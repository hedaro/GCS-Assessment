using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindConditionManager : MonoBehaviour
{
    [SerializeField]
    float m_MinWindStrength = 0f;

    [SerializeField]
    float m_MaxWindStrength = 1f;

    [SerializeField]
    float m_DirectionChangeChance = 0.5f;

    [SerializeField]
    float m_DirectionChangeTime = 20f;

    Vector3 m_Force;

    bool m_Blowing = false;

    private void Start()
    {
    }

    void FixedUpdate()
    {
        if (m_Blowing)
        {
            foreach (PlayerInfo p in GameManager.Instance.PlayersInfo)
            {
                foreach (Bullet b in p.Player.BulletMgr.BulletList)
                {
                    if (b.gameObject.activeInHierarchy)
                    {
                        b.transform.position += m_Force * Time.deltaTime;
                    }
                }
            }
        }        
    }

    IEnumerator ChangeForce()
    {
        while (m_Blowing)
        {
            if (Random.Range(0, 1) <= m_DirectionChangeChance)
            {
                int Direction = Random.Range(0, 4);
                switch (Direction)
                {
                    case 0:
                        m_Force = Vector3.up;
                        break;
                    case 1:
                        m_Force = Vector3.up * -1f;
                        break;
                    case 2:
                        m_Force = Vector3.right;
                        break;
                    case 3:
                        m_Force = Vector3.right * -1f;
                        break;
                }

                m_Force *= Random.Range(m_MinWindStrength, m_MaxWindStrength);

                yield return new WaitForSeconds(m_DirectionChangeTime);
            }
        }
    }

    public void StartWind()
    {
        m_Blowing = true;
        StartCoroutine(ChangeForce());
    }

    public void StopWind()
    {
        StopAllCoroutines();
        m_Blowing = false;
    }
}
