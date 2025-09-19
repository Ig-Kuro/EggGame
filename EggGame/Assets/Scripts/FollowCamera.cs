using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);
    private float initialX;
    private float initialZ;
    void Start()
    {
        initialX = transform.position.x;
        initialZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (player == null)
        {
            return;
        }
        float desiredY = player.position.y + offset.y;
        Vector3 smoothedPosition = Vector3.Lerp(
            new Vector3(initialX, transform.position.y, initialZ),
            new Vector3(initialX, desiredY, initialZ),
            followSpeed * Time.deltaTime
        );
        transform.position = smoothedPosition;
    }
}