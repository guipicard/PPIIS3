using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBallBehaviour : MonoBehaviour
{
    [SerializeField] private float m_Speed;
    private Vector3 m_TargetPos;
    private Vector3 m_InitialPos;
    // Start is called before the first frame update
    void Start()
    {
        m_TargetPos = Vector3.zero;
        m_InitialPos = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {

        if (m_TargetPos != Vector3.zero && m_InitialPos != Vector3.zero)
        {
            Vector3 direction = m_TargetPos - m_InitialPos;
            direction.Normalize();
            transform.position = Vector3.Lerp(m_InitialPos, m_TargetPos, m_Speed * Time.deltaTime);
            //direction * m_Speed * Time.deltaTime; 
            if (transform.position == m_TargetPos)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetTarget(Vector3 _targetPos)
    {
        m_TargetPos = _targetPos;
    }

    public void SetInitialPos(Vector3 _initialPos)
    {
        m_InitialPos = _initialPos;
    }
}
