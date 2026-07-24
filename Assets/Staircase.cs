using UnityEngine;
using Unity.Cinemachine; 

public class Staircase : MonoBehaviour
{
    [Header("Destination")]
    [SerializeField] private StairPoint targetStairPoint;

    [Header("Caméra Cinemachine")]
    [SerializeField] private CinemachineCamera virtualCamera;

    private static float nextAllowedTeleportTime = 0f;
    private static float cooldownDelay = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Time.time >= nextAllowedTeleportTime)
        {
            UseStaircase(collision.transform);
        }
    }

    public void UseStaircase(Transform playerTransform)
    {
        if (targetStairPoint == null || playerTransform == null) return;

        nextAllowedTeleportTime = Time.time + cooldownDelay;

        Vector3 targetPlayerPos = targetStairPoint.transform.position;
        targetPlayerPos.z = 0f;

        Vector3 deltaPosition = targetPlayerPos - playerTransform.position;

        if (playerTransform.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = new Vector2(targetPlayerPos.x, targetPlayerPos.y);
        }
        playerTransform.position = targetPlayerPos;

        if (virtualCamera != null)
        {
            Vector3 camPosition = new Vector3(targetPlayerPos.x, targetPlayerPos.y, -10f);

            virtualCamera.transform.position = camPosition;

            virtualCamera.OnTargetObjectWarped(playerTransform, deltaPosition);
            virtualCamera.ForceCameraPosition(camPosition, Quaternion.identity);
        }
    }
}