using UnityEngine;

public class Goals : MonoBehaviour
{
    [SerializeField] private bool isPlayerObjFinished;
    [SerializeField] private bool isShadowObjFinished;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void ObjectivesCheck()
    {
        if (isPlayerObjFinished == true && isShadowObjFinished == true)
        {
            Debug.Log("All objectives are complete");
        }
    }

    void OpenGoals()
    {
        Debug.Log("Next stage are available");
    }
}
