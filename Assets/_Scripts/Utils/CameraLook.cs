using UnityEngine;

public class CameraLook : MonoBehaviour
{

    public float sensitivityX = 10f;
    public float sensitivityY = 10f;
    public Transform player;

    private float rotationX = 0f;

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -10f, 15f);

        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        player.Rotate(Vector3.up * mouseX);
    }
}
