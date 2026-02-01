using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image skillParam;
    [SerializeField] private TextMeshProUGUI playerObjectiveText;
    [SerializeField] private TextMeshProUGUI shadowObjectiveText;

    [SerializeField] private Goals goals;

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
        if (goals != null)
        {
            if (playerObjectiveText != null)
                playerObjectiveText.text = string.Format("{0}/{1}", goals.CompletedPlayerObjectives, goals.RequiredPlayerObjectives);

            if (shadowObjectiveText != null)
                shadowObjectiveText.text = string.Format("{0}/{1}", goals.CompletedShadowObjectives, goals.RequiredShadowObjectives);
        }
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
