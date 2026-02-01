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

    [Header("Layer Detection")]
    [SerializeField] private LayerMask playerLightLayer;
    [SerializeField] private LayerMask shadowLightLayer;

    [Header("Debug Visualization")]
    [SerializeField] private bool showDebugVisualization = true;
    [SerializeField] private LineRenderer circleLineRenderer;
    [SerializeField] private LineRenderer connectionLineRenderer;
    [SerializeField] private int circleSegments = 64;

    private Vector3 lastMoveDirection = Vector3.forward;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        SetupDebugVisualization();
        SetupShadowTriggerDetection();
    }


    void Update()
    {
        MovementControl();
        UpdateDebugVisualization();
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
    private void SetupShadowTriggerDetection()
    {
        if (shadowBody != null)
        {
            // Add trigger detector to shadow body
            var shadowDetector = shadowBody.AddComponent<TriggerForwarder>();
            shadowDetector.Initialize(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if Player (Awake state) enters ShadowLight trigger
        if (playerStateManager.IsAwakeState() && IsInLayerMask(other.gameObject, shadowLightLayer))
        {
            HandlePlayerDeath();
        }
    }

    public void OnShadowTriggerEnter(Collider other)
    {
        // Check if Shadow (Astral state) enters PlayerLight trigger
        if (playerStateManager.IsAstralState() && IsInLayerMask(other.gameObject, playerLightLayer))
        {
            playerStateManager.SwitchToAwake();
        }
    }

    public void OnShadowTriggerStay(Collider other)
    {
        if (playerStateManager.IsAstralState() && IsInLayerMask(other.gameObject, playerLightLayer))
        {
            playerStateManager.SwitchToAwake();
        }
    }

    private void HandlePlayerDeath()
    {
        Debug.Log("Player died from ShadowLight!");
        // Add your death logic here (e.g., respawn, game over, etc.)
        // For now, just logging
    }

    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return ((1 << obj.layer) & layerMask) != 0;
    }

    // Inner class to forward shadow trigger events
    private class TriggerForwarder : MonoBehaviour
    {
        private PlayerController controller;

        public void Initialize(PlayerController ctrl)
        {
            controller = ctrl;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (controller != null)
            {
                controller.OnShadowTriggerEnter(other);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (controller != null)
            {
                controller.OnShadowTriggerStay(other);
            }
        }
    }
#endregion

#region Debug Visualization
    private void SetupDebugVisualization()
    {
        if (!showDebugVisualization) return;

        // Create circle LineRenderer if not assigned
        if (circleLineRenderer == null)
        {
            GameObject circleObj = new GameObject("ShadowRadiusCircle");
            circleObj.transform.SetParent(playerRb.transform);
            circleObj.transform.localPosition = Vector3.zero;
            circleLineRenderer = circleObj.AddComponent<LineRenderer>();
            circleLineRenderer.startWidth = 0.1f;
            circleLineRenderer.endWidth = 0.1f;
            circleLineRenderer.loop = true;
            circleLineRenderer.useWorldSpace = true;
            circleLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            circleLineRenderer.startColor = Color.cyan;
            circleLineRenderer.endColor = Color.cyan;
            circleLineRenderer.positionCount = circleSegments;
        }

        // Create connection LineRenderer if not assigned
        if (connectionLineRenderer == null)
        {
            GameObject connectionObj = new GameObject("PlayerShadowConnection");
            connectionObj.transform.SetParent(transform);
            connectionObj.transform.localPosition = Vector3.zero;
            connectionLineRenderer = connectionObj.AddComponent<LineRenderer>();
            connectionLineRenderer.startWidth = 0.05f;
            connectionLineRenderer.endWidth = 0.05f;
            connectionLineRenderer.useWorldSpace = true;
            connectionLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            connectionLineRenderer.startColor = Color.yellow;
            connectionLineRenderer.endColor = Color.yellow;
            connectionLineRenderer.positionCount = 2;
        }
    }

    private void UpdateDebugVisualization()
    {
        if (!showDebugVisualization) return;

        // Update circle around player
        if (circleLineRenderer != null && playerRb != null)
        {
            Vector3 center = playerRb.transform.position;
            float angleStep = 360f / circleSegments;

            for (int i = 0; i < circleSegments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 point = center + new Vector3(Mathf.Cos(angle) * maxShadowDistance, 0, Mathf.Sin(angle) * maxShadowDistance);
                circleLineRenderer.SetPosition(i, point);
            }
        }

        // Update connection line
        if (connectionLineRenderer != null && playerRb != null && shadowBody != null)
        {
            if (shadowBody.activeSelf && playerStateManager != null && playerStateManager.IsAstralState())
            {
                connectionLineRenderer.enabled = true;
                connectionLineRenderer.SetPosition(0, playerRb.transform.position);
                connectionLineRenderer.SetPosition(1, shadowBody.transform.position);
            }
            else
            {
                connectionLineRenderer.enabled = false;
            }
        }
    }
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
