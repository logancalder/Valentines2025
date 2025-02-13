using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float smoothSpeed = 5f;

    private float fixedZ; // Store the initial Z position

    void Start()
    {
        fixedZ = transform.position.z; // Keep the camera's original Z position
    }

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 targetPosition = player.position + offset;
            targetPosition.z = fixedZ; // Lock Z position

            // Snap to pixel grid (adjust 16 if your game has a different PPU)
            float pixelsPerUnit = 16f;
            targetPosition.x = Mathf.Round(targetPosition.x * pixelsPerUnit) / pixelsPerUnit;
            targetPosition.y = Mathf.Round(targetPosition.y * pixelsPerUnit) / pixelsPerUnit;

            transform.position = targetPosition;
        }
    }

}
