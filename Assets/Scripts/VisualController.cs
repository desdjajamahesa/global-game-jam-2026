using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VisualController : MonoBehaviour
{
    private List<MeshRenderer> staticObsRenderers = new List<MeshRenderer>();
    private int staticObsLayer;

    [Header("Pressure Plate Movement")]
    public float pressedPosY = 0f;
    public float releasedPosY = 0.1f;
    public float moveSpeed = 10f;
    private Vector3 targetLocalPos;

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
        targetLocalPos = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPos, Time.deltaTime * moveSpeed);
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

    public void SetPressedPos()
    {
        targetLocalPos = new Vector3(transform.localPosition.x, pressedPosY, transform.localPosition.z);
    }

    public void SetReleasedPos()
    {
        targetLocalPos = new Vector3(transform.localPosition.x, releasedPosY, transform.localPosition.z);
    }
}
