using UnityEngine;

public class CameraFlow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // Finds the position the camera should move toward
        Vector3 desiredPosition = target.position + offset;

        // Smoothly moves the camera toward the player
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Applies the new camera position
        transform.position = smoothedPosition;
    }
}
