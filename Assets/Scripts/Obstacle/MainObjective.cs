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
    private bool correctObjectInTrigger = false;

    void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        shadowLayer = LayerMask.NameToLayer("Shadow");
    }

    void Update()
    {
        // Check for E key press while correct object is in trigger
        if (correctObjectInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (objectives == Objectives.Player)
            {
                onPlayerGoalsComplete?.Invoke();
            }
            if (objectives == Objectives.Shadow)
            {
                onShadowGoalsComplete?.Invoke();
            }
        }
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
