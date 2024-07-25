using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer m_Sprite;

    [SerializeField]
    Color m_DamageColor = Color.white;

    public BulletManager BulletMgr { get { return m_BulletManager; } }

    [SerializeField]
    Transform m_BulletSpawnPoint;

    [SerializeField]
    BulletManager m_BulletManager;

    public InputManager InputManager { get { return m_InputManager; } }

    [SerializeField]
    InputManager m_InputManager;

    [SerializeField]
    MovingObject m_MovingObject;

    [SerializeField]
    float m_BoostChargeTime = 5f;

    [SerializeField]
    float m_MaxHealth = 5f;

    [SerializeField]
    Slider m_HealthSlider;

    [SerializeField]
    Slider m_BoostSlider;

    [SerializeField]
    ParticleSystem m_FlameParticle;

    [SerializeField]
    ParticleSystem m_DeadParticleEffect;

    [SerializeField]
    AudioSource m_BoostAudioSource;

    [SerializeField]
    AudioSource m_HitAudioSource;

    [SerializeField]
    AudioClip m_HitAudioClip;

    [SerializeField]
    AudioClip m_DestroyedAudioClip;

    float m_BoostCharge = 0f;

    float m_CurrentHealth;

    public int PlayerID { get { return m_PlayerId; } }

    int m_PlayerId;

    public bool IsDead { get { return m_bDead; } }

    bool m_bDead;

    void Start()
    {
        if (m_BulletManager != null)
        {
            m_BulletManager.SetOwner(this);
        }

        if (m_InputManager != null)
        {
            m_InputManager.SetOwner(this);
        }

        if (m_MovingObject != null)
        {
            m_MovingObject.OnScreenLoop += ScreenLoopEvent;
        }

        m_FlameParticle.Pause();
        m_bDead = true;
    }

    void Update()
    {
        if (m_bDead)
        {
            return;
        }

        if (!m_MovingObject.bBoost && m_BoostCharge < 1f)
        {
            m_BoostCharge += Time.deltaTime / m_BoostChargeTime;
            m_BoostSlider.value = m_BoostCharge;
        }
    }

    public void SetPlayerID(int id)
    {
        m_PlayerId = id;
    }

    public void InitializePlayer()
    {
        StopAllCoroutines();
        m_BulletManager.KillAllBullets();

        m_HealthSlider.gameObject.SetActive(true);
        m_BoostSlider.gameObject.SetActive(true);

        m_CurrentHealth = m_MaxHealth;
        m_HealthSlider.value = m_CurrentHealth / m_MaxHealth;
        m_BoostCharge = 0f;
        m_BoostSlider.value = m_BoostCharge;
        
        m_FlameParticle.gameObject.SetActive(true);
        m_FlameParticle.Play();
        m_DeadParticleEffect.gameObject.SetActive(false);
        
        m_Sprite.enabled = true;
        m_bDead = false;

        m_MovingObject.StartMovement();
    }

    public void DeactivatePlayer()
    {
        m_MovingObject.StopMovement();
        m_FlameParticle.gameObject.SetActive(false);
        m_HealthSlider.gameObject.SetActive(false);
        m_BoostSlider.gameObject.SetActive(false);
        m_bDead = true;
    }

    IEnumerator SpendEnergyBar()
    {
        while (m_MovingObject.bBoost)
        {
            m_BoostCharge -= Time.deltaTime / m_MovingObject.BoostDuration;
            m_BoostSlider.value = m_BoostCharge;
            yield return null;
        }
    }

    public void Turn(ETurnDirection Direction)
    {
        m_MovingObject.Turn(Direction);
        m_MovingObject.Break();
    }

    public void Boost()
    {
        if (!m_MovingObject.bBoost && m_BoostCharge >= 1f)
        {
            m_MovingObject.ApplyBoost();
            m_BoostAudioSource.Play();
            Vector3 Target = transform.position + m_MovingObject.BoostDuration * m_MovingObject.Velocity;
            GameManager.Instance.CameraFXManager.CameraZoomIn(Target, .9f, m_MovingObject.BoostDuration, 1f);
            StartCoroutine(SpendEnergyBar());
        }
    }

    public void Fire()
    {
        m_BulletManager.FireBullet(m_BulletSpawnPoint);
        StartCoroutine(HandleFireEffect());
    }

    IEnumerator HandleFireEffect()
    {
        m_BulletSpawnPoint.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        m_BulletSpawnPoint.gameObject.SetActive(false);
    }

    public bool ApplyDamage(float Damage)
    {
        m_CurrentHealth -= Damage;
        m_HealthSlider.value = m_CurrentHealth / m_MaxHealth;
        StartCoroutine(HandleDamageEffect());

        if (m_CurrentHealth <= 0f)
        {
            DeathSequence();
        }

        return m_bDead;
    }

    IEnumerator HandleDamageEffect()
    {
        m_HitAudioSource.PlayOneShot(m_HitAudioClip);
        GameManager.Instance.CameraFXManager.ShakeCamera();

        m_Sprite.color = m_DamageColor;
        Time.timeScale = 0;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        m_Sprite.color = Color.white;
        Time.timeScale = 1;
    }

    void DeathSequence()
    {
        DeactivatePlayer();
        m_Sprite.enabled = false;
        m_HitAudioSource.PlayOneShot(m_DestroyedAudioClip);
        m_DeadParticleEffect.gameObject.SetActive(true);
        m_DeadParticleEffect.Play();
        GameManager.Instance.CameraFXManager.CameraZoomIn(transform.position, .8f, 2f, 0.5f);
        GameManager.Instance.GameOver();
    }

    void ScreenLoopEvent()
    {
        StartCoroutine(HandleScreenLoop());
    }

    IEnumerator HandleScreenLoop()
    {
        m_FlameParticle.Pause();
        yield return new WaitForEndOfFrame();
        m_FlameParticle.Play();
    }
}
