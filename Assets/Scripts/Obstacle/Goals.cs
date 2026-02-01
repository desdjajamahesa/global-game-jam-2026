using UnityEngine;

public class Goals : MonoBehaviour
{
    [Header("Objective Requirements")]
    [SerializeField] private int requiredPlayerObjectives = 1;
    [SerializeField] private int requiredShadowObjectives = 1;
    [SerializeField] private bool autoDetectObjectives = true;

    [Header("Current Progress")]
    [SerializeField] private int completedPlayerObjectives = 0;
    [SerializeField] private int completedShadowObjectives = 0;

    void Start()
    {
        // Auto-detect objectives in the scene if enabled
        if (autoDetectObjectives)
        {
            MainObjective[] allObjectives = FindObjectsOfType<MainObjective>();
            requiredPlayerObjectives = 0;
            requiredShadowObjectives = 0;

            foreach (MainObjective obj in allObjectives)
            {
                if (obj.GetObjectiveType() == "Player")
                {
                    requiredPlayerObjectives++;
                }
                else if (obj.GetObjectiveType() == "Shadow")
                {
                    requiredShadowObjectives++;
                }
            }

            Debug.Log($"Auto-detected {requiredPlayerObjectives} Player objectives and {requiredShadowObjectives} Shadow objectives");
        }
    }

    void OnEnable()
    {
        MainObjective.onPlayerGoalsComplete += PlayerFinished;
        MainObjective.onShadowGoalsComplete += ShadowFinished;    
    }

    void OnDisable()
    {
        MainObjective.onPlayerGoalsComplete -= PlayerFinished;
        MainObjective.onShadowGoalsComplete -= ShadowFinished;
    }
    
    void ObjectivesCheck()
    {
        Debug.Log($"Objectives Status - Player: {completedPlayerObjectives}/{requiredPlayerObjectives}, Shadow: {completedShadowObjectives}/{requiredShadowObjectives}");
        
        if (completedPlayerObjectives >= requiredPlayerObjectives && 
            completedShadowObjectives >= requiredShadowObjectives)
        {
            OpenGoals();
            Debug.Log("All objectives are complete!");
        }
    }

    void PlayerFinished()
    {
        completedPlayerObjectives++;
        Debug.Log($"Player objective completed! ({completedPlayerObjectives}/{requiredPlayerObjectives})");
        ObjectivesCheck();
    }

    void ShadowFinished()
    {
        completedShadowObjectives++;
        Debug.Log($"Shadow objective completed! ({completedShadowObjectives}/{requiredShadowObjectives})");
        ObjectivesCheck();
    }

    void OpenGoals()
    {
        Debug.Log("Next stage is available!");
        // Add your level completion logic here
    }

    // Public methods to get progress
    public float GetPlayerProgress()
    {
        if (requiredPlayerObjectives == 0) return 1f;
        return (float)completedPlayerObjectives / requiredPlayerObjectives;
    }

    public float GetShadowProgress()
    {
        if (requiredShadowObjectives == 0) return 1f;
        return (float)completedShadowObjectives / requiredShadowObjectives;
    }

    public float GetTotalProgress()
    {
        int totalRequired = requiredPlayerObjectives + requiredShadowObjectives;
        if (totalRequired == 0) return 1f;
        int totalCompleted = completedPlayerObjectives + completedShadowObjectives;
        return (float)totalCompleted / totalRequired;
    }
}
