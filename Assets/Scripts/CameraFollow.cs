using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; 
    [SerializeField] private Vector3 offset = new Vector3(0, 4, -11);
    [SerializeField] private float smoothTime = .2f; 
    
    private Vector3 velocity = Vector3.zero;

    private bool isFollowing = true; // Kamera takip ediyor mu?

    private void OnEnable()
    {
        EventManager.Subscribe(GameEvents.OnFinishGame, BreakCameraFollow);
        EventManager.Subscribe(GameEvents.OnStartGame, StartCameraFollow);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(GameEvents.OnFinishGame, BreakCameraFollow);
        EventManager.Unsubscribe(GameEvents.OnStartGame, StartCameraFollow);
    }

    private void BreakCameraFollow()
    {
        isFollowing = false; 
    }
    private void StartCameraFollow()
    {
        isFollowing = true; 
    }

    void LateUpdate()
    {
        if (target == null || !isFollowing) return; 

        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        Vector3 lookDirection = target.position - transform.position;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }
}