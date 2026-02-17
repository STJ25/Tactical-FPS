using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensX = 200f;
    public float sensY = 200f;

    [Header("Smoothing")]
    public float smoothTime = 0.05f; // lower = snappier, higher = smoother

    public Transform orientation;

    private float xRotation, yRotation;
    private float targetXRotation, targetYRotation;
    private float xVelocity, yVelocity; // for SmoothDamp

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- Get raw mouse input ---
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY * Time.deltaTime;

        targetYRotation += mouseX;
        targetXRotation -= mouseY;
        targetXRotation = Mathf.Clamp(targetXRotation, -90f, 90f);

        // --- Smooth damp rotation ---
        xRotation = Mathf.SmoothDamp(xRotation, targetXRotation, ref xVelocity, smoothTime);
        yRotation = Mathf.SmoothDamp(yRotation, targetYRotation, ref yVelocity, smoothTime);

        // --- Apply rotation ---
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
