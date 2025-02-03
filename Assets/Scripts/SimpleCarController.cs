using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SimpleCarController : MonoBehaviour
{
    [SerializeField] private Rigidbody _carRigidbody;
    [SerializeField] private MeshCollider _meshCollider;
    
    public int maxSpeed = 175;
    public int maxReverseSpeed = 55;
    public int accelerationMultiplier = 7;
    public int maxSteeringAngle = 30;
    public float steeringSpeed = 0.3f;
    public int brakeForce = 450;
    public int decelerationMultiplier = 2;
    public Vector3 bodyMassCenter = new Vector3(0, -0.5f, 0);

    public WheelCollider frontLeftCollider, frontRightCollider, rearLeftCollider, rearRightCollider;
    public GameObject frontLeftMesh, frontRightMesh, rearLeftMesh, rearRightMesh;

    private float steeringAxis, throttleAxis, localVelocityX, localVelocityZ;

    public void Initialize()
    {
        _carRigidbody.isKinematic = false;
        _carRigidbody.centerOfMass = bodyMassCenter;
        _meshCollider.enabled = true;
    }

    private void OnEnable()
    {
        EventManager.Subscribe(GameEvents.OnFinishGame, OnGameFinish);
    }

    private void OnDisable()
    {
        EventManager.Subscribe(GameEvents.OnFinishGame, OnGameFinish);
    }

    void Update()
    {
        localVelocityX = transform.InverseTransformDirection(_carRigidbody.velocity).x;
        localVelocityZ = transform.InverseTransformDirection(_carRigidbody.velocity).z;

        if (Input.GetKey(KeyCode.W)) GoForward();
        else if (Input.GetKey(KeyCode.S)) GoReverse();
        else ThrottleOff();

        if (Input.GetKey(KeyCode.A)) TurnLeft();
        else if (Input.GetKey(KeyCode.D)) TurnRight();
        else ResetSteeringAngle();

        AnimateWheelMeshes();
    }

    void GoForward() => ApplyThrottle(1);
    void GoReverse() => ApplyThrottle(-1);
    void ThrottleOff() => ApplyThrottle(0);

    void TurnLeft() => SetSteering(-1);
    void TurnRight() => SetSteering(1);
    void ResetSteeringAngle() => SetSteering(0);

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
            if (Mathf.Abs(_carRigidbody.velocity.magnitude * 3.6f) < (direction > 0 ? maxSpeed : maxReverseSpeed))
            {
                rearLeftCollider.motorTorque = rearRightCollider.motorTorque = torque;
            }
        }
    }

    void Brakes()
    {
        rearLeftCollider.brakeTorque = rearRightCollider.brakeTorque = brakeForce * 0.3f;
    }

    void SetSteering(float direction)
    {
        steeringAxis = Mathf.Lerp(steeringAxis, direction, Time.deltaTime * steeringSpeed * 5);
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
}
