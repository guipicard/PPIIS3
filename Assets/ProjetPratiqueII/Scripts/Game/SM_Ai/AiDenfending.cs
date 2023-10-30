using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiDenfending : AiState
{
    public AiDenfending(AIStateMachine stateMachine) : base(stateMachine)
    {
        m_Animator.SetBool(running, false);
        m_Animator.SetInteger(moveState, 0);
        m_NavmeshAgent.destination = m_Transform.position;
    }

    public override void UpdateExecute()
    {
        _AiStateMachine.IncrementCD();
        m_PlayerDistance = Vector3.Distance(player.transform.position, m_Transform.position);
        
        if (m_PlayerDistance > m_attackDistance)
        {
            _AiStateMachine.SetState(new AiMoving(_AiStateMachine));
        }

        if (m_PlayerDistance < m_SafeDistance)
        {
            _AiStateMachine.SetState(new AiMoving(_AiStateMachine));
        }
        m_Transform.LookAt(player.transform.position);
    }

    public override void FixedUpdateExecute()
    {
        Vector3 playerPosition = player.transform.position;
        m_Transform.LookAt(playerPosition);
    }
}
