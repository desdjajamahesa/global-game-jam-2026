using UnityEngine;
using DG.Tweening;
using System;

enum State
{
    Awake,
    Astral
}

public class PlayerStateManager : MonoBehaviour
{
    private BaseState currentState;

    [Header("Component Reference")]
    [SerializeField] public GameObject playerBody;
    [SerializeField] public Rigidbody playerRb;
    [SerializeField] public GameObject shadowBody;
    [SerializeField] public Rigidbody shadowRb;
    [SerializeField] private PlayerController playerController;

    [Header("Shadow Config")]
    [SerializeField] private float slideDistance = 5f;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private bool shadowPassThroughWalls = false;
    [SerializeField] private LayerMask wallLayers; // Layers that should block shadow spawn

    public static Action onAwakeActive;
    public static Action onAstralActive;

    public AwakeState awakeState = new AwakeState();
    public AstralState astralState = new AstralState();

    private bool isTransitioning = false;
    private bool controlShadow = false; // Tracks which body is actively controlled
    private int originalShadowLayer;

    void Start()
    {
        shadowBody.SetActive(false);
        currentState = awakeState;
        currentState.EnterState(this);
        
        // Store the original shadow layer
        originalShadowLayer = shadowBody.layer;
    }

    void Update()
    {
        if (!isTransitioning)
        {
            currentState.UpdateState(this);
        }
    }
    
#region Input
    
#endregion

#region State Switch
    public void SwitchToAstral()
    {
        onAstralActive?.Invoke();
        if (isTransitioning) return;
        isTransitioning = true;
        

        currentState.ExitState(this);
        currentState = astralState;
        
        // Enable shadow body
        shadowBody.transform.position = playerBody.transform.position;
        shadowBody.SetActive(true);
        
        // Immediately switch control to shadow
        controlShadow = true;
        
        // Calculate slide direction
        Vector3 slideDirection = -playerController.GetFacingDirection();
        Vector3 startPos = shadowBody.transform.position;
        Vector3 targetPos = startPos + slideDirection * slideDistance;
        
        // Check for obstacles and adjust target position
        if (!shadowPassThroughWalls)
        {
            RaycastHit hit;
            float checkDistance = slideDistance + 0.5f; // Add small buffer
            
            if (Physics.Raycast(startPos, slideDirection, out hit, checkDistance, wallLayers))
            {
                // Hit a wall, stop before it
                targetPos = hit.point + (-slideDirection * 0.2f); // Stop 0.2 units before wall
            }
        }
        else
        {
            // Optionally disable collisions during slide if passing through walls
            if (shadowRb != null)
            {
                shadowRb.detectCollisions = false;
            }
        }
        
        // Use Rigidbody.DOMove for physics-based movement
        if (shadowRb != null)
        {
            shadowRb.DOMove(targetPos, slideDuration).SetEase(Ease.OutQuad).OnComplete(() => {
                // Re-enable collisions after slide if they were disabled
                if (shadowPassThroughWalls)
                {
                    shadowRb.detectCollisions = true;
                }
                isTransitioning = false;
                currentState.EnterState(this);
            });
        }
        else
        {
            shadowBody.transform.DOMove(targetPos, slideDuration).SetEase(Ease.OutQuad).OnComplete(() => {
                isTransitioning = false;
                currentState.EnterState(this);
            });
        }
    }

    public void SwitchToAwake()
    {
        onAwakeActive?.Invoke();
        if (isTransitioning) return;
        isTransitioning = true;

        currentState.ExitState(this);
        currentState = awakeState;
        
        // Disable collisions during return slide
        if (shadowPassThroughWalls && shadowRb != null)
        {
            shadowRb.detectCollisions = false;
        }
        
        // Slide shadow back to player
        if (shadowRb != null)
        {
            shadowRb.DOMove(playerBody.transform.position, slideDuration).SetEase(Ease.InQuad).OnComplete(() => {
                shadowBody.SetActive(false);
                controlShadow = false;
                isTransitioning = false;
                
                // Re-enable collisions when shadow is deactivated
                if (shadowPassThroughWalls)
                {
                    shadowRb.detectCollisions = true;
                }
                
                currentState.EnterState(this);
            });
        }
        else
        {
            shadowBody.transform.DOMove(playerBody.transform.position, slideDuration).SetEase(Ease.InQuad).OnComplete(() => {
                shadowBody.SetActive(false);
                controlShadow = false;
                isTransitioning = false;
                currentState.EnterState(this);
            });
        }
    }

    public bool IsAwakeState()
    {
        return !controlShadow;
    }

    public bool IsAstralState()
    {
        return controlShadow;
    }
#endregion

}
