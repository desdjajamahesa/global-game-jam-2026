using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Behavior Configs")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxShadowDistance = 10f; // Maximum distance shadow can be from player

    [Header("Component Configs")]
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Rigidbody shadowRb;
    [SerializeField] private GameObject shadowBody;

    [Header("Component Reference")]
    [SerializeField] private PlayerStateManager playerStateManager;

    private Vector3 lastMoveDirection = Vector3.forward;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        MovementControl();
    }

#region InputArea 
    public void MovementControl()
    {
        float moveX = 0;
        float moveZ = 0;

        if (Input.GetKey(KeyCode.W))
        {
            moveZ = moveSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveZ = -moveSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -moveSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = moveSpeed;
        }

        // Update player facing direction based on movement
        if (moveX != 0 || moveZ != 0)
        {
            lastMoveDirection = new Vector3(moveX, 0, moveZ).normalized;
        }

        // Move appropriate body based on state
        if (playerStateManager.IsAwakeState())
        {
            playerRb.velocity = new Vector3(moveX, playerRb.velocity.y, moveZ);
        }
        else if (playerStateManager.IsAstralState())
        {
            // Check distance and constrain shadow movement
            Vector3 playerPos = playerRb.transform.position;
            Vector3 shadowPos = shadowRb.transform.position;
            
            // Flatten to horizontal plane
            Vector3 playerPosFlat = new Vector3(playerPos.x, 0, playerPos.z);
            Vector3 shadowPosFlat = new Vector3(shadowPos.x, 0, shadowPos.z);
            
            float currentDistance = Vector3.Distance(playerPosFlat, shadowPosFlat);
            Vector3 desiredVelocity = new Vector3(moveX, 0, moveZ);
            
            if (currentDistance >= maxShadowDistance && desiredVelocity.magnitude > 0.01f)
            {
                // At or beyond max distance - project movement onto circle boundary
                Vector3 directionFromPlayer = (shadowPosFlat - playerPosFlat).normalized;
                
                // Project desired velocity onto the tangent (perpendicular to radius)
                // Remove the radial component to allow only tangential movement
                float radialComponent = Vector3.Dot(desiredVelocity, directionFromPlayer);
                
                if (radialComponent > 0)
                {
                    // Trying to move away - only allow tangential movement (slide along edge)
                    desiredVelocity -= directionFromPlayer * radialComponent;
                }
                // If moving towards center (radialComponent < 0), allow full movement
                
                shadowRb.velocity = new Vector3(desiredVelocity.x, shadowRb.velocity.y, desiredVelocity.z);
            }
            else
            {
                // Within radius - move freely
                shadowRb.velocity = new Vector3(moveX, shadowRb.velocity.y, moveZ);
            }
        }
    }

    public Vector3 GetFacingDirection()
    {
        return lastMoveDirection;
    }


#endregion

#region Behavior

#endregion

    private void OnDrawGizmos()
    {
        if (playerRb != null)
        {
            // Draw radius around player
            Gizmos.color = Color.cyan;
            DrawCircle(playerRb.transform.position, maxShadowDistance, 64);
            
            // Draw line between player and shadow when in astral state
            if (shadowBody != null && shadowBody.activeSelf)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(playerRb.transform.position, shadowBody.transform.position);
            }
        }
    }

    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}
