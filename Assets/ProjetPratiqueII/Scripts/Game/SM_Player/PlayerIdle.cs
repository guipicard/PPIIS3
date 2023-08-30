using UnityEngine;

public class PlayerIdle : PlayerState
{
    public PlayerIdle(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        m_Animator.SetBool(Running, false);
        m_Animator.SetBool(MineAnim, false);
        m_CurrentVelocity = Vector3.zero;
        m_Direction = Vector3.zero;
    }
    
    public override void UpdateExecute()
    {
        
    }
    
    public override void FixedUpdateExecute()
    {
        
    }
}
