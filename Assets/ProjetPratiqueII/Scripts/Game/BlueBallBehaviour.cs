using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBallBehaviour : MonoBehaviour
{
    [SerializeField] private float m_Speed;
    private GameObject m_Target;
    private Vector3 m_InitialPos;
    private float m_Time;
    private float m_Elapsed;
    private float m_TravelElapsed;

    // Start is called before the first frame update
    void Start()
    {
        m_Elapsed = 0.0f;
        m_TravelElapsed = 0.0f;
        transform.LookAt(m_Target.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Elapsed <= m_Time / m_Speed)
        {
            m_Elapsed += Time.deltaTime;
        }

        if (m_Elapsed > m_Time / m_Speed)
        {
            Vector3 targetPos = m_Target.transform.position;
            Vector3 direction = targetPos - m_InitialPos;
            direction.Normalize();
            transform.position = Vector3.Lerp(m_InitialPos, targetPos, m_TravelElapsed * m_Speed);
            //direction * m_Speed * Time.deltaTime; 
            if (!m_Target.activeSelf)
            {
                Destroy(gameObject);
            }

            if (transform.position == targetPos)
            {
                Destroy(gameObject);
                if (m_Target.activeSelf) m_Target.GetComponent<CrystalEvents>().GetMined();
            }

            m_TravelElapsed += Time.deltaTime;
        }
    }


    public void SetTarget(GameObject _target)
    {
        m_Target = _target;
    }

    public void SetInitialPos(Vector3 _initialPos)
    {
        m_InitialPos = _initialPos;
    }

    public void SetTimer(float _time)
    {
        m_Time = _time;
    }
}