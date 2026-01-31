using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image skillParam;

    private void OnEnable() 
    {
        PlayerStateManager.onAwakeActive += UIAwakeActive;
        PlayerStateManager.onAstralActive += UIAstralActive;
    }

    private void OnDisable() 
    {
        PlayerStateManager.onAwakeActive -= UIAwakeActive;
        PlayerStateManager.onAstralActive -= UIAstralActive;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UIAwakeActive()
    {
        skillParam.color = Color.yellow;
    }

    public void UIAstralActive()
    {
        skillParam.color = Color.black;
    }
}
