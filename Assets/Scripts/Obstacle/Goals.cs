using UnityEngine;

public class Goals : MonoBehaviour
{
    [SerializeField] private bool isPlayerObjFinished;
    [SerializeField] private bool isShadowObjFinished;

    void OnEnable()
    {
        MainObjective.onPlayerGoalsComplete += PlayerFinished;
        MainObjective.onShadowGoalsComplete += ShadowFinished;    
    }

    void OnDisable()
    {
        MainObjective.onPlayerGoalsComplete += PlayerFinished;
        MainObjective.onShadowGoalsComplete += ShadowFinished;
    }
    
    void ObjectivesCheck()
    {
        Debug.Log("Checking Objectives Status");
        if (isPlayerObjFinished == true && isShadowObjFinished == true)
        {
            OpenGoals();
            Debug.Log("All objectives are complete");
        }
    }

    void PlayerFinished()
    {
        isPlayerObjFinished = true;
        ObjectivesCheck();
    }

    void ShadowFinished()
    {
        isShadowObjFinished = true;
        ObjectivesCheck();
    }

    void OpenGoals()
    {
        Debug.Log("Next stage are available");
    }
}
