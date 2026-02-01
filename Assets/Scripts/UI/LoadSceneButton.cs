using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] private string sceneName = "TestAwe";

    // Call this from a UI Button OnClick to load the configured scene
    public void LoadTestAwe()
    {
        if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);
    }

    // Optional: generic loader if you want to pass a string via the Button
    public void LoadSceneByName(string name)
    {
        if (!string.IsNullOrEmpty(name))
            SceneManager.LoadScene(name);
    }
}
