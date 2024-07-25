using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public MovingObject movingObject
    {
        get { return m_MovingObject; }
    }

    [SerializeField]
    MovingObject m_MovingObject;

    [SerializeField]
    float m_Damage = 1f;

    [SerializeField]
    ParticleSystem m_HitParticleEffect;

    [SerializeField]
    AudioSource m_ShotAudioSource;

    float m_BulletLifespan = 0f;

    BulletManager m_BulletManager;
    PlayerController m_Player;
    Coroutine m_CoroutineHandle;

    public void InitializeBullet(BulletManager bulletManager, PlayerController player, float bulletSpeed)
    {
        m_MovingObject.SetSpeed(bulletSpeed);
        m_BulletManager = bulletManager;
        m_Player = player;
    }

    public void KillBullet()
    {
        StopCoroutine(m_CoroutineHandle);
        m_MovingObject.StopMovement();
        gameObject.SetActive(false);

        m_BulletManager.RequeueBullet(this);
    }

    public void ActivateBullet(Transform spawnPoint, float speed, float lifespan)
    {
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.transform.rotation;
        m_BulletLifespan = lifespan;
        gameObject.SetActive(true);
        m_MovingObject.StartMovement(speed);

        m_CoroutineHandle = StartCoroutine(Activate());
    }

    IEnumerator Activate()
    {
        //m_ShotAudioSource.Play();

        yield return new WaitForSeconds(m_BulletLifespan);

        KillBullet();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null || player == m_Player || player.IsDead || m_Player.IsDead)
        {
            return;
        }

        if (player.ApplyDamage(m_Damage))
        {
            GameManager.Instance.AddScore(m_Player);
        }

        StartCoroutine(HandleBulletHit());
    }

    IEnumerator HandleBulletHit()
    {
        m_HitParticleEffect.Play();

        yield return new WaitForSecondsRealtime(0.1f);

        KillBullet();
    }
}
