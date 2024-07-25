using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAIBehavior
{
    BALANCED,
    DEFENSIVE,
    AGGRESSIVE
}

public class AIController : MonoBehaviour
{
    [SerializeField]
    PlayerController m_Player;

    [SerializeField]
    float m_MinDistanceToEvade = 2f;

    [SerializeField]
    float m_MaxDistanceToEvade = 5f;

    [SerializeField]
    float m_MinAngleToEvade = 10f;

    [SerializeField]
    float m_MinDistanceToAttack = 5f;

    [SerializeField]
    float m_MaxAngleToAttack = 10f;

    [SerializeField]
    float m_MinDistanceToFollow = 2f;

    [SerializeField]
    float m_MaxDistanceToFollow = 5f;

    [SerializeField]
    float m_MinAngleToFollow = 10f;

    [SerializeField]
    float m_MaxAngleToFollow = 135f;

    [SerializeField]
    float m_NoiseFactor = 0.1f;

    EAIBehavior m_Behavior;

    PlayerController m_EnemyPlayer;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void InitializeAI()
    {
        m_EnemyPlayer = GameManager.Instance.GetOtherPlayer(m_Player.PlayerID);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Player.IsDead)
        {
            return;
        }

        float DistanceToEnemy = Vector3.Distance(transform.position, m_EnemyPlayer.transform.position);

        Vector3 DirectionToEnemy = m_EnemyPlayer.transform.position - transform.position;
        float SideDotProduct = Vector3.Dot(DirectionToEnemy, transform.right);
        float DirectionDotProduct = Vector3.Dot(transform.up, m_EnemyPlayer.transform.up);

        float AngleToEnemy = Vector3.Angle(transform.up, DirectionToEnemy) * (1 + Random.Range(-m_NoiseFactor, m_NoiseFactor));
        float AngleFromEnemy = Vector3.Angle(m_EnemyPlayer.transform.up, -DirectionToEnemy) * (1 + Random.Range(-m_NoiseFactor, m_NoiseFactor));

        switch (m_Behavior)
        {
            case EAIBehavior.BALANCED:
                BalancedBehavior(DistanceToEnemy, AngleFromEnemy, AngleFromEnemy, DirectionDotProduct, SideDotProduct);
                break;
            case EAIBehavior.AGGRESSIVE:
                AggressiveBehavior(DistanceToEnemy, AngleToEnemy, AngleFromEnemy, DirectionDotProduct, SideDotProduct);
                break;
            case EAIBehavior.DEFENSIVE:
                DefensiveBehavior(DistanceToEnemy, AngleToEnemy, AngleFromEnemy, DirectionDotProduct, SideDotProduct);
                break;
        }

        if (DistanceToEnemy < m_MinDistanceToAttack && AngleToEnemy < m_MaxAngleToAttack)
        {
            m_Player.Fire();
        }
    }

    public void SetBehavior(EAIBehavior behavior)
    {
        m_Behavior = behavior;
    }

    void DefensiveBehavior(float DistanceToEnemy, float AngleToEnemy, float AngleFromEnemy, float DirectionDotProduct, float SideDotProduct)
    {
        if (AngleFromEnemy <= m_MinAngleToEvade && DistanceToEnemy <= m_MaxDistanceToEvade && DistanceToEnemy >= m_MinDistanceToEvade)
        {
            if (DirectionDotProduct < 0)
            {
                TurnFromEnemy(SideDotProduct);
            }
            else
            {
                m_Player.Boost();
            }
        }
        else if (AngleToEnemy >= m_MinAngleToFollow && AngleToEnemy <= m_MaxAngleToFollow)
        {
            if (AngleFromEnemy <= m_MinAngleToEvade && DistanceToEnemy <= m_MaxDistanceToEvade)
            {
                TurnFromEnemy(SideDotProduct);
            }
            else if (DistanceToEnemy >= m_MinDistanceToFollow && DistanceToEnemy <= m_MaxDistanceToFollow)
            {
                TurnToEnemy(SideDotProduct);
            } 
            else if (AngleFromEnemy <= m_MinAngleToEvade && DistanceToEnemy <= m_MinDistanceToEvade && DirectionDotProduct > 0)
            {
                m_Player.Boost();
            }
        }
    }

    void BalancedBehavior(float DistanceToEnemy, float AngleToEnemy, float AngleFromEnemy, float DirectionDotProduct, float SideDotProduct)
    {
        if (AngleFromEnemy <= m_MinAngleToEvade && DirectionDotProduct < 0)
        {
            if (DistanceToEnemy >= m_MaxDistanceToFollow)
            {
                m_Player.Boost();
            }
            else if (DistanceToEnemy <= m_MinDistanceToEvade)
            {
                TurnFromEnemy(SideDotProduct);
            }

        }
        else if (AngleToEnemy >= m_MinAngleToFollow && AngleToEnemy <= m_MaxAngleToFollow)
        {
            if (AngleFromEnemy <= m_MinAngleToEvade && DistanceToEnemy <= m_MinDistanceToEvade)
            {
                TurnFromEnemy(SideDotProduct);
            }
            else if (DistanceToEnemy >= m_MinDistanceToFollow && DistanceToEnemy <= m_MaxDistanceToFollow)
            {
                TurnToEnemy(SideDotProduct);
            }
            else if (AngleFromEnemy <= m_MinAngleToEvade && DistanceToEnemy <= m_MinDistanceToEvade && DirectionDotProduct > 0)
            {
                m_Player.Boost();
            }
        }
    }

    void AggressiveBehavior(float DistanceToEnemy, float AngleToEnemy, float AngleFromEnemy, float DirectionDotProduct, float SideDotProduct)
    {
        if (AngleToEnemy >= m_MinAngleToFollow)
        {
            if (DistanceToEnemy >= m_MinDistanceToFollow && DistanceToEnemy <= m_MaxDistanceToFollow)
            {
                TurnToEnemy(SideDotProduct);
            }
            else if (AngleFromEnemy <= m_MinAngleToEvade && DistanceToEnemy <= m_MinDistanceToEvade)
            {
                m_Player.Boost();
            }            
        }
        else if (AngleFromEnemy <= m_MinAngleToEvade && DirectionDotProduct < 0)
        {
            if (DistanceToEnemy >= m_MaxDistanceToFollow)
            {
                m_Player.Boost();
            }
            else if (DistanceToEnemy <= m_MinDistanceToEvade)
            {
                TurnFromEnemy(SideDotProduct);
            }
            
        }
    }

    void TurnToEnemy(float DotProduct)
    {
        if (DotProduct < 0f)
        {
            m_Player.Turn(ETurnDirection.LEFT);
        }
        else
        {
            m_Player.Turn(ETurnDirection.RIGHT);
        }
    }

    void TurnFromEnemy(float DotProduct)
    {
        if (DotProduct < 0f)
        {
            m_Player.Turn(ETurnDirection.RIGHT);
        }
        else
        {
            m_Player.Turn(ETurnDirection.LEFT);
        }
    }
}
