using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMining : PlayerState
{
    private float m_Elapsed;
    public PlayerMining(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        m_Animator.SetBool(Running, false);
        if (!m_Mining) m_Animator.SetTrigger(MineAnim);
        m_Mining = true;
        m_Elapsed = 0.0f;
        m_CurrentVelocity = Vector3.zero;
        m_Transform.LookAt(m_TargetCrystal.transform.position);
    }

    public override void UpdateExecute()
    {
        m_Elapsed += Time.deltaTime;
        if (m_Elapsed >= 0.25)
        {
            MineCrystal();
        }
    }

    public override void FixedUpdateExecute()
    {
    }
    
    private void MineCrystal()
    {
        if (m_TargetCrystal.activeSelf)
        {
            m_TargetCrystal.GetComponent<CrystalEvents>().GetMined();
            m_TargetCrystal = null;
            m_Mining = false;
            _StateMachine.SetState(new PlayerIdle(_StateMachine));
        }
    }
}
