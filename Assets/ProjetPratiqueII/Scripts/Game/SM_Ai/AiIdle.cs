using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiIdle : AiState
{
    public AiIdle(AIStateMachine stateMachine) : base(stateMachine)
    {
        m_NavmeshAgent.destination = m_Transform.position;
        m_Animator.SetBool(running, false);
        m_Animator.SetInteger(moveState, 0);
        m_Animator.SetBool(combat, false);
    }

    public override void UpdateExecute()
    {
        m_PlayerDistance = Vector3.Distance(player.transform.position, m_Transform.position);
        if (m_PlayerDistance < m_TriggerDistance)
        {
            _AiStateMachine.SetState(new AiMoving(_AiStateMachine));
        }
    }

    public override void FixedUpdateExecute()
    {
    }
}
