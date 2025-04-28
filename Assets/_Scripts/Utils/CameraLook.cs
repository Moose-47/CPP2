using UnityEngine;

public class CameraLook : MonoBehaviour
{

    public Transform player;
    public Transform cameraTransform; 
    public float sensitivityX = 10f;
    public float sensitivityY = 10f;
    public float minY = -5f;
    public float maxY = 60f;
    public float distanceFromPlayer = 5f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        if (player != null) 
        { 
            float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

            rotationY += mouseX;
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, minY, maxY);

            //Rotate around the player
            transform.position = player.position;
            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);

            //Set the camera's position behind the pivot point
            cameraTransform.position = transform.position - transform.forward * distanceFromPlayer;
            cameraTransform.LookAt(player.position + Vector3.up * 1.5f);
        }
    }
}
