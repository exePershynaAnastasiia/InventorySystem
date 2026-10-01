using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Camera Position")]
    public Vector3 offset = new Vector3(1.2f, 1.8f, -3f);

    [Header("Mouse Look")]
    public float sensitivity = 0.15f;
    public float minPitch = -30f;
    public float maxPitch = 60f;

    private float yaw;
    private float pitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        yaw += mouseDelta.x * sensitivity;
        pitch -= mouseDelta.y * sensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        transform.position = target.position + rotation * offset;

        transform.LookAt(target.position + Vector3.up * 1.2f);
    }
}