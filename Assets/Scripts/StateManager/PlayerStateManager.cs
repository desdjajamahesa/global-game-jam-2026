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

    public static Action onAwakeActive;
    public static Action onAstralActive;

    public AwakeState awakeState = new AwakeState();
    public AstralState astralState = new AstralState();

    private bool isTransitioning = false;
    private bool controlShadow = false; // Tracks which body is actively controlled

    void Start()
    {
        shadowBody.SetActive(false);
        currentState = awakeState;
        currentState.EnterState(this);
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
        
        // Slide shadow in opposite direction of player's facing (behind the player)
        Vector3 slideDirection = -playerController.GetFacingDirection();
        Vector3 targetPos = shadowBody.transform.position + slideDirection * slideDistance;
        
        shadowBody.transform.DOMove(targetPos, slideDuration).OnComplete(() => {
            isTransitioning = false;
            currentState.EnterState(this);
        });
    }

    public void SwitchToAwake()
    {
        onAwakeActive?.Invoke();
        if (isTransitioning) return;
        isTransitioning = true;

        currentState.ExitState(this);
        currentState = awakeState;
        
        // Slide shadow back to player
        shadowBody.transform.DOMove(playerBody.transform.position, slideDuration).OnComplete(() => {
            shadowBody.SetActive(false);
            controlShadow = false; // Switch control back to player after shadow vanishes
            isTransitioning = false;
            currentState.EnterState(this);
        });
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
