using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ETurnDirection
{
    LEFT = 1,
    RIGHT = -1
}

public class MovingObject : MonoBehaviour
{
    [SerializeField]
    float m_MaxSpeed = 1f;

    [SerializeField]
    float m_TurnSpeed = 10f;

    [SerializeField]
    float m_BreakSpeedModifier = 0.5f;

    [SerializeField]
    float m_Acceleration = 0.1f;

    [SerializeField]
    float m_BoostSpeedModifier = 2f;

    [SerializeField]
    float m_BoostDuration = 1f;

    public event Action OnScreenLoop;

    public float BoostDuration
    {
        get { return m_BoostDuration; }
    }

    public Vector3 Velocity {  get { return m_Velocity; } }

    Vector3 m_Velocity = Vector3.zero;

    bool m_Moving = false;

    float m_SpeedMultiplier = 1f;

    bool m_Boost = false;


    public bool bBoost { get { return m_Boost; } }

    void Start()
    {
    }

    void FixedUpdate()
    {
        if (m_Moving && m_MaxSpeed != 0)
        {
            m_Velocity += transform.up * (m_Acceleration / m_MaxSpeed) * Time.deltaTime;
            
            if (m_Velocity.magnitude > 1)
            {
                m_Velocity.Normalize();
            }
        }

        MoveForward();
        AdjustPositionInMap();

        if (m_SpeedMultiplier < 1f)
        {
            m_SpeedMultiplier += m_SpeedMultiplier * (m_Acceleration / m_MaxSpeed) * Time.deltaTime;
            m_SpeedMultiplier = Mathf.Clamp(m_SpeedMultiplier, 0f, 1f);
        }
    }

    public void StopMovement()
    {
        m_Velocity = Vector3.zero;
        m_Moving = false;
    }

    public void StartMovement(float StartingSpeed = 0f)
    {
        m_Moving = true;
        m_Velocity = transform.up * StartingSpeed;
    }

    public void SetSpeed(float speed)
    {
        m_MaxSpeed = speed;
    }

    void MoveForward()
    {
        transform.position += m_Velocity * m_MaxSpeed * m_SpeedMultiplier * Time.deltaTime;
    }

    public void Turn(ETurnDirection Direction)
    {
        if (m_Moving && !m_Boost)
        {
            transform.eulerAngles += (float)Direction * transform.forward * m_TurnSpeed * Time.deltaTime * Mathf.PI / 2f;
        }
    }

    public void Break()
    {
        if (!m_Boost)
        {
            m_SpeedMultiplier = m_BreakSpeedModifier;
        }
    }

    public void ApplyBoost() 
    {
        if (!m_Boost)
        {
            StartCoroutine(HandleBoost());
        }
    }

    IEnumerator HandleBoost()
    {
        m_Boost = true;
        m_Velocity = transform.up * m_MaxSpeed;
        m_SpeedMultiplier = m_BoostSpeedModifier;

        yield return new WaitForSeconds(m_BoostDuration);

        m_Boost = false;
        m_SpeedMultiplier = 1f;
    }

    void AdjustPositionInMap()
    {
        Bounds cameraBounds = GameManager.Instance.CameraBounds;

        if (transform.position.y < cameraBounds.center.y - cameraBounds.extents.y)
        {
            transform.position += Vector3.up * cameraBounds.size.y;
            if (OnScreenLoop != null)
            {
                OnScreenLoop();
            }
        }
        else if(transform.position.y > cameraBounds.center.y + cameraBounds.extents.y)
        {
            transform.position -= Vector3.up * cameraBounds.size.y;
            if (OnScreenLoop != null)
            {
                OnScreenLoop();
            }
        }

        if (transform.position.x < cameraBounds.center.x - cameraBounds.extents.x)
        {
            transform.position += Vector3.right * cameraBounds.size.x;
            if (OnScreenLoop != null)
            {
                OnScreenLoop();
            }
        }
        else if (transform.position.x > cameraBounds.center.x + cameraBounds.extents.x)
        {
            transform.position -= Vector3.right * cameraBounds.size.x;
            if (OnScreenLoop != null)
            {
                OnScreenLoop();
            }
        }
    }
}
