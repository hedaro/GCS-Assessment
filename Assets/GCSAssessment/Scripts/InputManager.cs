using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    KeyCode m_TurnLeftKey;

    [SerializeField]
    KeyCode m_TurnRightKey;

    [SerializeField]
    KeyCode m_BoostKey;

    [SerializeField]
    KeyCode m_FireKey;

    PlayerController m_Player;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Player.IsDead)
        {
            return;
        }

        HandleInput();
    }

    public void SetOwner(PlayerController player)
    {
        m_Player = player;
    }

    void HandleInput()
    {
        if (Input.GetKey(m_TurnLeftKey))
        {
            m_Player.Turn(ETurnDirection.LEFT);
        }
        else if (Input.GetKey(m_TurnRightKey))
        {
            m_Player.Turn(ETurnDirection.RIGHT);
        }

        if (Input.GetKeyDown(m_BoostKey))
        {
            m_Player.Boost();
        }

        if (Input.GetKey(m_FireKey))
        {
            m_Player.Fire();
        }
    }
}
