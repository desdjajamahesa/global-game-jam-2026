using System.Collections.Generic;
using UnityEngine;

public class VisualController : MonoBehaviour
{
    private List<MeshRenderer> staticObsRenderers = new List<MeshRenderer>();
    private int staticObsLayer;

    private void OnEnable() 
    {
        PlayerStateManager.onAwakeActive += ShowStaticObs;
        PlayerStateManager.onAstralActive += HideStaticObs;
    }

    private void OnDisable() 
    {
        PlayerStateManager.onAwakeActive -= ShowStaticObs;
        PlayerStateManager.onAstralActive -= HideStaticObs;
    }

    void Start()
    {
        staticObsLayer = LayerMask.NameToLayer("StaticObs");
        FindAllStaticObsObjects();
    }

    private void FindAllStaticObsObjects()
    {
        staticObsRenderers.Clear();
        
        // Find all GameObjects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            // Check if object is on StaticObs layer
            if (obj.layer == staticObsLayer)
            {
                MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    staticObsRenderers.Add(renderer);
                }
            }
        }
    }

    private void HideStaticObs()
    {
        foreach (MeshRenderer renderer in staticObsRenderers)
        {
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }
    }

    private void ShowStaticObs()
    {
        foreach (MeshRenderer renderer in staticObsRenderers)
        {
            if (renderer != null)
            {
                renderer.enabled = true;
            }
        }
    }
}
