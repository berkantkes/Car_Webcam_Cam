using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SimpleCarController : MonoBehaviour
{
    [SerializeField] private Rigidbody _carRigidbody;
    [SerializeField] private MeshCollider _meshCollider;
    [SerializeField] private Vector3 startPosition = new Vector3(-2f, 0f, 27.25f);
    [SerializeField] private Vector3 startRotation = new Vector3(0f, 270f, 0f);
    [SerializeField] private AudioSource _carEngineSound; // This variable stores the sound of the car engine.

    private UiController _uiController;
    private int maxSpeed = 650;
    private int maxReverseSpeed = 55;
    private int accelerationMultiplier = 16;
    private int maxSteeringAngle = 45;
    private float steeringSpeed = 0.6f;
    private int brakeForce = 500;
    private int decelerationMultiplier = 2;
    private float naturalDrag = .5f; //  // 🔹 Yan kaymayı önlemek için stabilizasyon kuvvetiaz kesildiğinde daha hızlı yavaşlasın
    public float stabilizationForce = 5f; // 
    float initialCarEngineSoundPitch = .75f; // Used to store the initial pitch of the car engine sound.

    private Vector3 bodyMassCenter = new Vector3(0, 0, 0);

    public WheelCollider frontLeftCollider, frontRightCollider, rearLeftCollider, rearRightCollider;
    public GameObject frontLeftMesh, frontRightMesh, rearLeftMesh, rearRightMesh;

    private float steeringAxis, throttleAxis, localVelocityX, localVelocityZ;
    private int startLineTouchCount = 0;
    private Transform _startTransform;
    private float _carSpeed = 0;

    public void Initialize(UiController uiController)
    {
        _uiController = uiController;
    }
    
    private void OnEnable()
    {
        EventManager.Subscribe(GameEvents.OnStartGame, SetStartSettings);
        EventManager.Subscribe(GameEvents.OnFinishGame, OnGameFinish);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(GameEvents.OnStartGame, SetStartSettings);
        EventManager.Unsubscribe(GameEvents.OnFinishGame, OnGameFinish);
    }
    
    private void SetStartSettings()
    {
        Debug.Log("SetStartSettings");
        _carRigidbody.centerOfMass = bodyMassCenter;
        _meshCollider.enabled = true;
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(startRotation);
        _carRigidbody.isKinematic = false;
        startLineTouchCount = 0;

        AdjustWheelFriction();
    }

    void Update()
    {
        float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(_carRigidbody.velocity.magnitude) / 25f);
        _carEngineSound.pitch = engineSoundPitch;
        _carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
        localVelocityX = transform.InverseTransformDirection(_carRigidbody.velocity).x;
        localVelocityZ = transform.InverseTransformDirection(_carRigidbody.velocity).z;

        // 🔹 `Vertical` ve `Horizontal` ile giriş al
        float verticalInput = Input.GetAxis("Vertical");   // W - S veya Yukarı - Aşağı ok tuşları
        float horizontalInput = Input.GetAxis("Horizontal"); // A - D veya Sol - Sağ ok tuşları

        ApplyThrottle(verticalInput);
        SetSteering(horizontalInput);

        ApplyStabilization(); // 🔹 Drift sırasında aracı dengele

        AnimateWheelMeshes();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StartLine"))
        {
            startLineTouchCount++;
            if (startLineTouchCount == 4)
            {
                EventManager.Execute(GameEvents.OnFinishGame);
                Debug.Log("Game finished!");
                
                _carRigidbody.isKinematic = true;
                _uiController.FinishGame();
            }
        }
    }

    // void GoForward() => ApplyThrottle(1);
    // void GoReverse() => ApplyThrottle(-1);
    // void ThrottleOff() => ApplyThrottle(0);
    //
    // void TurnLeft() => SetSteering(-1);
    // void TurnRight() => SetSteering(1);
    // void ResetSteeringAngle() => SetSteering(0);

    void ApplyThrottle(float direction)
    {
        throttleAxis = Mathf.Lerp(throttleAxis, direction, Time.deltaTime * 10);
        float torque = accelerationMultiplier * 50f * throttleAxis;

        if ((direction > 0 && localVelocityZ < -0.5f) || (direction < 0 && localVelocityZ > 0.5f))
        {
            Brakes();
        }
        else
        {
            rearLeftCollider.brakeTorque = rearRightCollider.brakeTorque = 0;

            if (direction == 0)
            {
                rearLeftCollider.motorTorque = rearRightCollider.motorTorque = 0;
                _carRigidbody.drag = naturalDrag;
            }
            else
            {
                _carRigidbody.drag = 0;

                if (Mathf.Abs(_carRigidbody.velocity.magnitude * 3.6f) < (direction > 0 ? maxSpeed : maxReverseSpeed))
                {
                    rearLeftCollider.motorTorque = rearRightCollider.motorTorque = torque;
                }
            }
        }
    }

    void Brakes()
    {
        rearLeftCollider.brakeTorque = rearRightCollider.brakeTorque = brakeForce;
    }

    void SetSteering(float direction)
    {
        steeringAxis = Mathf.Lerp(steeringAxis, direction, Time.deltaTime * steeringSpeed * 15);
        float angle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = frontRightCollider.steerAngle = angle;
    }

    void AnimateWheelMeshes()
    {
        UpdateWheelMesh(frontLeftCollider, frontLeftMesh);
        UpdateWheelMesh(frontRightCollider, frontRightMesh);
        UpdateWheelMesh(rearLeftCollider, rearLeftMesh);
        UpdateWheelMesh(rearRightCollider, rearRightMesh);
    }

    void UpdateWheelMesh(WheelCollider collider, GameObject mesh)
    {
        collider.GetWorldPose(out Vector3 position, out Quaternion rotation);
        mesh.transform.position = position;
        mesh.transform.rotation = rotation;
    }

    private void OnGameFinish()
    {
        _meshCollider.enabled = false;
    }

    
    private void AdjustWheelFriction()
    {
        SetFriction(frontLeftCollider, 1.5f, 2.5f);
        SetFriction(frontRightCollider, 1.5f, 2.5f);
        SetFriction(rearLeftCollider, 2f, 3.5f);
        SetFriction(rearRightCollider, 2f, 3.5f);
    }

    private void SetFriction(WheelCollider wheel, float forwardStiffness, float sidewaysStiffness)
    {
        WheelFrictionCurve forwardFriction = wheel.forwardFriction;
        forwardFriction.stiffness = forwardStiffness;
        wheel.forwardFriction = forwardFriction;

        WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
        sidewaysFriction.stiffness = sidewaysStiffness;
        wheel.sidewaysFriction = sidewaysFriction;
    }

    
    private void ApplyStabilization()
    {
        if (Mathf.Abs(localVelocityX) > 2f) // Yan kayma algılanıyorsa
        {
            _carRigidbody.AddForce(-transform.right * localVelocityX * stabilizationForce, ForceMode.Acceleration);
        }
    }
}
