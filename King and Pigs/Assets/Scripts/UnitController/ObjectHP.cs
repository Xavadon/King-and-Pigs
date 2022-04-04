using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHP : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator _anim;

    [Header("Health")]
    [SerializeField] private float _currentHP;

    [Header("Hurt Sounds")]
    [SerializeField] private AudioClip[] _hurtClips;
    [SerializeField] private AudioClip[] _dieClips;
    private AudioSource _audioSource;
    private Collider2D _collider;
    private float _maxHP;

    [Header("GameOverUI")]
    [SerializeField] private GameObject _gameOverPanel;

    private bool _gotDamage;
    private bool _isDead;

    public bool GotDamage => _gotDamage;
    public bool IsDead => _isDead;

    public float CurrentHp => _currentHP;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _collider = GetComponent<Collider2D>();
        _maxHP = _currentHP;
    }
    private void Update()
    {
        if (_currentHP > _maxHP) _currentHP = _maxHP;

        _anim.SetBool("IsHit", _gotDamage);
    }

    public void Hit(float _damage)
    {
        _currentHP -= _damage;
        if (_currentHP <= 0)
        {
            Dead();
            DeadSound();
        }
        else
        {
            HurtSound();
            StartCoroutine(HitDamage());

            _anim.SetTrigger("Hit");
        }
    }

    public void Heal()
    {
        if (!_isDead)
        {
            _currentHP += 1;
            if (_currentHP > _maxHP) _currentHP = _maxHP;
        }
    }

    private void DeadSound()
    {
        _audioSource.clip = _dieClips[Random.Range(0, _dieClips.Length)];
        _audioSource.Play();
    }

    private void HurtSound()
    {
        _audioSource.clip = _hurtClips[Random.Range(0, _hurtClips.Length)];
        _audioSource.Play();
    }

    private IEnumerator HitDamage()
    {
        _gotDamage = true;
        yield return new WaitForSeconds(0.7f);
        _gotDamage = false;
    }

    private void Dead()
    {
        _isDead = true;
        _collider.enabled = false;
        if (_gameOverPanel != null) StartCoroutine(GameOver());

        _anim.SetTrigger("Dead");
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2);
        _gameOverPanel.SetActive(true);
    }
}
