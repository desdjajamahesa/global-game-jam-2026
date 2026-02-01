using UnityEngine;
using System;

enum Objectives
{
    Player,
    Shadow
}

public class MainObjective : MonoBehaviour
{
    [SerializeField] private Objectives objectives;
    public static Action onPlayerGoalsComplete;
    public static Action onShadowGoalsComplete;
    
    private int playerLayer;
    private int shadowLayer;
    [SerializeField] private ParticleSystem splashParticle;
    [SerializeField] private int splashEmitCount = 30;
    private bool correctObjectInTrigger = false;
    private bool isCompleted = false;

    void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        shadowLayer = LayerMask.NameToLayer("Shadow");

        // Auto-assign particle system if not set in inspector
        if (splashParticle == null)
            splashParticle = GetComponentInChildren<ParticleSystem>();

        // Ensure particle does not play automatically on Awake/Start
        if (splashParticle != null)
        {
            var main = splashParticle.main;
            main.playOnAwake = false;
            splashParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    // Public method to get objective type for auto-detection
    public string GetObjectiveType()
    {
        return objectives.ToString();
    }

    void Update()
    {
        // Check for E key press while correct object is in trigger and not already completed
        if (correctObjectInTrigger && !isCompleted && Input.GetKeyDown(KeyCode.E))
        {
            CompleteObjective();
        }
    }

    void CompleteObjective()
    {
        isCompleted = true;

        // Emit a one-shot splash burst if available
        if (splashParticle != null)
        {
            splashParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            splashParticle.Clear();
            splashParticle.Emit(Mathf.Max(1, splashEmitCount));
        }
        
        if (objectives == Objectives.Player)
        {
            Debug.Log("Player objective completed!");
            onPlayerGoalsComplete?.Invoke();
        }
        else if (objectives == Objectives.Shadow)
        {
            Debug.Log("Shadow objective completed!");
            onShadowGoalsComplete?.Invoke();
        }
        
        // Optional: Disable or visually change the objective after completion
        // gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) 
    {
        // Check if Player objective and player entered
        if (objectives == Objectives.Player && other.gameObject.layer == playerLayer)
        {
            correctObjectInTrigger = true;
        }
        
        // Check if Shadow objective and shadow entered
        if (objectives == Objectives.Shadow && other.gameObject.layer == shadowLayer)
        {
            correctObjectInTrigger = true;

        }
    }

    private void OnTriggerExit(Collider other) 
    {
        // Reset flag when object leaves
        if ((objectives == Objectives.Player && other.gameObject.layer == playerLayer) ||
            (objectives == Objectives.Shadow && other.gameObject.layer == shadowLayer))
        {
            correctObjectInTrigger = false;
        }
    }
}
