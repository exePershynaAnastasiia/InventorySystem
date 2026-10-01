using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance;

    public float sensX = 0.1f;
    public float sensY = 0.1f;

    public Transform orientation;

    public bool updatingRotation = true;

    private float xRotation;
    private float yRotation;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!updatingRotation)
            return;

        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        yRotation += mouseDelta.x * sensX;
        xRotation -= mouseDelta.y * sensY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation =
            Quaternion.Euler(xRotation, yRotation, 0f);

        orientation.rotation =
            Quaternion.Euler(0f, yRotation, 0f);
    }
}