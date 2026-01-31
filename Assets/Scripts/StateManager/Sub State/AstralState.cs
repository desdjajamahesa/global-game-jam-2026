using UnityEngine;

public class AstralState : BaseState
{

    public override void EnterState(PlayerStateManager manager)
    {
        
    }

    public override void UpdateState(PlayerStateManager manager)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            manager.SwitchToAwake();
        }
    }

    public override void ExitState(PlayerStateManager manager)
    {
        
    }

}


