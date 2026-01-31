using UnityEngine;

public class AwakeState : BaseState
{
    public override void EnterState(PlayerStateManager manager)
    {
        
    }

    public override void UpdateState(PlayerStateManager manager)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            manager.SwitchToAstral();
        }
    }
    
    public override void ExitState(PlayerStateManager manager)
    {
        
    }

}
