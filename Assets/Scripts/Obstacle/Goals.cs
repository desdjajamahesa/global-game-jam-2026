using UnityEngine;
using UnityEngine.SceneManagement;

public class Goals : MonoBehaviour
{
    [Header("Objective Requirements")]
    [SerializeField] private int requiredPlayerObjectives = 1;
    [SerializeField] private int requiredShadowObjectives = 1;
    [SerializeField] private bool autoDetectObjectives = true;

    [Header("Current Progress")]
    [SerializeField] private int completedPlayerObjectives = 0;
    [SerializeField] private int completedShadowObjectives = 0;

    [Header("Door and Scene Settings")]
    [SerializeField] private string doorChildName = "TargetedDoor";
    [SerializeField] private Vector3 doorOpenPosition = new Vector3(0, 6f, 0);
    [SerializeField] private float doorMoveDuration = 1f;
    [SerializeField] private bool enableSceneTransition = true;
    [SerializeField] private string nextSceneName = "";

    private GameObject targetedDoor;
    private bool doorOpened = false;
    private bool canTransition = false;

    void Start()
    {
        // Find the door from children
        Transform doorTransform = transform.Find(doorChildName);
        if (doorTransform != null)
        {
            targetedDoor = doorTransform.gameObject;
            Debug.Log($"Found targeted door: {doorChildName}");
        }
        else
        {
            Debug.LogWarning($"Child GameObject '{doorChildName}' not found!");
        }

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
        
        if (!doorOpened && targetedDoor != null)
        {
            // Move the door to the open position
            StartCoroutine(MoveDoorToPosition(targetedDoor.transform, doorOpenPosition, doorMoveDuration));
            doorOpened = true;
            Debug.Log($"Door moving to position {doorOpenPosition}");
        }

        // Enable scene transition
        if (enableSceneTransition)
        {
            canTransition = true;
            Debug.Log("Scene transition enabled - enter the trigger to load next scene");
        }
    }

    private System.Collections.IEnumerator MoveDoorToPosition(Transform doorTransform, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = doorTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            doorTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        doorTransform.localPosition = targetPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if can transition and if player or shadow entered
        if (canTransition && enableSceneTransition)
        {
            // Check if it's the player or shadow (adjust layer check as needed)
            if (other.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("Player") || 
                other.gameObject.layer == LayerMask.NameToLayer("Shadow"))
            {
                LoadNextScene();
            }
        }
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log($"Loading next scene: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next scene name is not set!");
        }
    }

    // Public methods to get progress
    // Public getters for UI access
    public int RequiredPlayerObjectives => requiredPlayerObjectives;
    public int RequiredShadowObjectives => requiredShadowObjectives;
    public int CompletedPlayerObjectives => completedPlayerObjectives;
    public int CompletedShadowObjectives => completedShadowObjectives;

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
