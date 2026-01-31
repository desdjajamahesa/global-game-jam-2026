using UnityEngine;

public class DynamicObstacle : MonoBehaviour
{
    [Header("Movement Config")]
    [SerializeField] private float moveDistance = 1f; // Distance to move per push
    [SerializeField] private float moveSpeed = 5f; // Speed of the lerp movement
    [SerializeField] private float pushThreshold = 0.1f;
    
    private Rigidbody rb;
    private int playerLayer;
    
    private bool isMoving = false;
    private bool playerTouching = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float moveProgress = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerLayer = LayerMask.NameToLayer("Player");
        
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        rb.isKinematic = true; // Use kinematic for smooth lerp movement
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            moveProgress += Time.fixedDeltaTime * moveSpeed;
            
            // Smoothly move to target position
            rb.MovePosition(Vector3.Lerp(startPosition, targetPosition, moveProgress));
            
            // Check if reached target
            if (moveProgress >= 1f)
            {
                rb.MovePosition(targetPosition); // Snap to exact position
                isMoving = false;
                moveProgress = 0f;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == playerLayer)
        {
            if (!playerTouching && !isMoving)
            {
                playerTouching = true;
                StartPush(collision);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == playerLayer)
        {
            playerTouching = false;
        }
    }

    private void StartPush(Collision collision)
    {
        Vector3 direction = transform.position - collision.transform.position;
        direction.y = 0;
        
        if (direction.magnitude > pushThreshold)
        {
            // Determine push direction (lock to strongest axis)
            Vector3 moveDirection;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            {
                // Move on X axis only
                moveDirection = new Vector3(Mathf.Sign(direction.x), 0, 0);
            }
            else
            {
                // Move on Z axis only
                moveDirection = new Vector3(0, 0, Mathf.Sign(direction.z));
            }
            
            // Set up movement
            startPosition = transform.position;
            targetPosition = startPosition + moveDirection * moveDistance;
            isMoving = true;
            moveProgress = 0f;
        }
    }
}
