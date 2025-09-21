using UnityEngine;

public class EggPlayerController : MonoBehaviour
{
    public float minJumpForce = 5f;
    public float maxJumpForce = 20f;
    public float maxChargeTime = 1.5f;
    public float repulsionForce = 5f;

    [Header("Efeitos")]
    public AudioSource jumpSoundSource;
    public AudioClip jumpSoundClip;
    public ParticleSystem jumpVFX;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    public LineRenderer jumpIndicator;
    public int trajectoryPoints = 30;
    public float trajectoryExtensionFactor = 1.5f;

    private Rigidbody rb;
    private float jumpChargeTime;
    private bool isChargingJump = false;
    private int direction = 1; 
    private bool wasGrounded = false;
    private float gravity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("sem RB");
        }
        gravity = Physics.gravity.y;
    }

    void Update()
    {
        bool isCurrentlyGrounded = IsGrounded();

        if (isCurrentlyGrounded && !wasGrounded)
        {
            rb.linearVelocity = Vector3.zero;
        }

        if (isCurrentlyGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isChargingJump = true;
                jumpChargeTime = 0f;
                if (jumpIndicator != null)
                {
                    jumpIndicator.enabled = true;
                }
            }

            if (isChargingJump && Input.GetKey(KeyCode.Space))
            {
                jumpChargeTime += Time.deltaTime;
                jumpChargeTime = Mathf.Min(jumpChargeTime, maxChargeTime);

                if (jumpIndicator != null)
                {
                    Vector3 startPos = transform.position;
                    float chargeRatio = jumpChargeTime / maxChargeTime;
                    float currentJumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargeRatio);

                    Vector3 initialVelocity = new Vector3(direction * currentJumpForce, currentJumpForce, 0);

                    Vector3[] points = new Vector3[trajectoryPoints];
                    jumpIndicator.positionCount = trajectoryPoints;

                    for (int i = 0; i < trajectoryPoints; i++)
                    {
                        float timeStep = (float)i / trajectoryPoints;
                        float t = timeStep * (currentJumpForce / Mathf.Abs(gravity)) * trajectoryExtensionFactor;

                        float x = initialVelocity.x * t;
                        float y = initialVelocity.y * t + 0.5f * gravity * t * t;
                        float z = 0;

                        points[i] = startPos + new Vector3(x, y, z);
                    }
                    jumpIndicator.SetPositions(points);
                }
            }

            if (Input.GetKeyUp(KeyCode.Space) && isChargingJump)
            {
                if (jumpIndicator != null)
                {
                    jumpIndicator.enabled = false;
                }

                float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, jumpChargeTime / maxChargeTime);
                Vector3 jumpDirection = new Vector3(direction, 1, 0);
                rb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);

                isChargingJump = false;
                if (jumpSoundSource != null && jumpSoundClip != null)
                {
                    jumpSoundSource.PlayOneShot(jumpSoundClip);
                }

                if (jumpVFX != null)
                {
                    jumpVFX.Play();
                }

                isChargingJump = false;
            }

        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = -1;
            transform.rotation = Quaternion.Euler(0, 270, 0);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = 1;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        wasGrounded = isCurrentlyGrounded;
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            return false;
        }
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector3 repelDirection = -collision.contacts[0].normal;
            rb.AddForce(repelDirection * repulsionForce, ForceMode.Impulse);
        }
    }
}