using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField]
    Bullet m_BulletPrefab;

    [SerializeField]
    int m_MaxBullets = 5;

    [SerializeField]
    float m_BulletLifespan = 2f;

    [SerializeField]
    float m_FireRate = 2f;

    [SerializeField]
    float m_BulletSpeed = 2f;

    int m_FiredBullets = 0;

    public float BulletsFiredRatio { get { return (float) m_FiredBullets / m_MaxBullets; } }

    PlayerController m_PlayerOwner;

    public List<Bullet> BulletList { get { return m_BulletList; } }

    List<Bullet> m_BulletList;
    Queue<Bullet> m_BulletQueue;

    float m_FireTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        m_BulletQueue = new Queue<Bullet>(m_MaxBullets);
        m_BulletList = new List<Bullet>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_FireTimer > 0f)
        {
            m_FireTimer -= Time.deltaTime;
        }   
    }
    
    public void SetOwner(PlayerController player)
    {
        m_PlayerOwner = player;
    }

    bool CanFire()
    {
        if (m_FireTimer > 0f || m_FiredBullets >= m_MaxBullets)
        {
            return false;
        }

        return true;
    }

    public bool FireBullet(Transform SpawnPoint)
    {
        if (CanFire())
        {
            Bullet NextBullet;
            if (m_BulletQueue.Count > 0)
            {
                NextBullet = m_BulletQueue.Dequeue();
            }
            else
            {
                NextBullet = SpawnBullet();
                if (NextBullet == null)
                {
                    return false;
                }
            }

            NextBullet.ActivateBullet(SpawnPoint, m_BulletSpeed, m_BulletLifespan);
            m_FiredBullets++;
            m_FireTimer = 1f / m_FireRate;
            return true;
        }

        return false;
    }

    Bullet SpawnBullet()
    {
        if (m_BulletList.Count >= m_MaxBullets)
        {
            return null;
        }

        Bullet NewBullet = Instantiate(m_BulletPrefab);
        NewBullet.InitializeBullet(this, m_PlayerOwner, m_BulletSpeed);
        m_BulletList.Add(NewBullet);

        return NewBullet;
    }

    public void RequeueBullet(Bullet bullet)
    {
        m_BulletQueue.Enqueue(bullet);
        m_FiredBullets--;
    }

    public void KillAllBullets()
    {
        m_BulletQueue?.Clear();
        foreach (Bullet bullet in m_BulletList)
        {
            bullet.KillBullet();
            RequeueBullet(bullet);
        }
    }
}
