using UnityEngine;

public class FreeCameraFollow : MonoBehaviour
{
    public Transform player;        // Player to follow
    public Vector3 offset = new Vector3(0, 2, -5);
    public float sensitivity = 3f;

    private float yaw = 0f;  // Horizontal rotation
    private float pitch = 0f; // Vertical rotation

    void Start()
    {
        // Lock cursor in middle but make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -89f, 89f); // prevent flipping

        // Rotate camera based on yaw + pitch
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // Camera position (smoothly follow player, no shaking)
        transform.position = player.position + rotation * offset;

        // Apply rotation
        transform.rotation = rotation;
    }
}
