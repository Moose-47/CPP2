using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.HighDefinition;

public class CameraController : MonoBehaviour
{

    private Transform player;
    public Transform cameraTransform;

    [Header("Rotation Settings")]
    public float sensitivityX = 10f;
    public float sensitivityY = 10f;
    public float minY = -5f;
    public float maxY = 60f;

    [Header("Camera Positioning")]
    public float distanceFromPlayer = 5f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    [Header("Lock On Settings")]
    public float lockOnSmoothSpeed = 5f;


    private Transform lockOnTarget;

    private bool isLockedOn = false;   
    private bool isPanningToPlayer = false;

    private Quaternion targetPanRotation = Quaternion.identity;

    private void OnEnable()
    {
        GameManager.Instance.OnPlayerSpawned += OnPlayerSpawnedCallback;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnPlayerSpawned -= OnPlayerSpawnedCallback;
    }
    private void OnPlayerSpawnedCallback(Player _player)
    {
        player = _player.transform;
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
        }

    }
    void Update()
    {
        if (player == null) return;

        transform.position = player.position;

        if (!isLockedOn && !isPanningToPlayer) 
        { 
            float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

            rotationY += mouseX;
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, minY, maxY);

            //Rotate around the player
            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        }
        else if (lockOnTarget != null)
        {
            Vector3 directionToTarget = (lockOnTarget.position - player.position).normalized;
            directionToTarget.y = 0f;

            if (directionToTarget.sqrMagnitude > 0f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnSmoothSpeed * Time.deltaTime);
            }
            
        }

        if (isPanningToPlayer)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetPanRotation, lockOnSmoothSpeed * 500f * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, targetPanRotation) < 0.1f)
            {
                isPanningToPlayer = false;

                transform.rotation = targetPanRotation;

                Vector3 angles = transform.rotation.eulerAngles;
                rotationY = angles.y;
            }
        }

        Vector3 offset = -transform.forward * distanceFromPlayer + Vector3.up * 1.5f;
        cameraTransform.position = transform.position + offset;
        cameraTransform.LookAt(player.position + Vector3.up * 1.5f);
    }

    public void ToggleLockOn(Transform target)
    {
        isLockedOn = (target != null);
        lockOnTarget = target;

        if (!isLockedOn)
        {
            Vector3 angles = transform.rotation.eulerAngles;
            rotationY = angles.y;
            rotationX = angles.x;
        }
    }

    public void PanToPlayerDirection()
    {
        if (player == null) return;

        isPanningToPlayer = true;

        // Get the desired Y rotation to match the player's forward direction
        Vector3 playerForward = player.forward;
        playerForward.y = 0;

        if (playerForward.sqrMagnitude > 0.01f)
        {
            // Only rotate around Y-axis (horizontal plane)
            float targetY = Quaternion.LookRotation(playerForward).eulerAngles.y;
            targetPanRotation = Quaternion.Euler(rotationX, targetY, 0f);
        }
    }
}
