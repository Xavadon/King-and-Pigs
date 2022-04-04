using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _speed;
    [SerializeField] private float _damage;

    [SerializeField] private float _attackDistance;
    [SerializeField] private float _chaseDistance;
    [SerializeField] private float _cooldownTime;
    [SerializeField] private Animator _anim;

    [Header("Attack")]
    [SerializeField] private GameObject _attackPoint1;
    [SerializeField] private GameObject _attackPoint2;
    [SerializeField] private LayerMask _playerLayer;
    private GameObject _target;
    private bool _notCooldown = true;

    [Header("Bomb")]
    [SerializeField] private GameObject _bomb;
    [SerializeField] private Transform _bombSpawnpoint;
    private bool _canShoot = true;

    [Header("Logic")]
    [SerializeField] private Transform _stayPoint;

    [Header("Sound")]
    [SerializeField] private AudioClip[] _attackClips;
    private AudioSource _audioSource;

    private Rigidbody2D _rb;

    private bool _gotDamage => GetComponent<ObjectHP>().GotDamage;
    private bool _isDead => GetComponent<ObjectHP>().IsDead;
    void Start()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"), true);
        _audioSource = GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!_isDead)
        {
            SetAnimator();
            if (_target != null)
            {
                Flip(_target.transform);
                Chase();
            }
            if (_target == null && _stayPoint != null) 
            {
                DefendPoint();
            }
        }
    }

    private void DefendPoint()
    {
        if (transform.position.x != _stayPoint.position.x)
        {
            GoToStayPoint();
            Flip(_stayPoint);
        }
        if (transform.position.x == _stayPoint.position.x)
        {
            _anim.SetFloat("XVelocity", 0);
        }
    }

    private void GoToStayPoint()
    {
        Vector2 targetPos = new Vector2(_stayPoint.transform.position.x, transform.position.y);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * 0.5f * Time.deltaTime);

        _anim.SetFloat("XVelocity", 1);
    }

    private void Chase()
    {
        Vector2 _pos = transform.position;
        Vector2 _targetPos = _target.transform.position;

        float _distance = Vector2.Distance(_pos, _targetPos);
        if (!_gotDamage)
        {
            if (_distance > _chaseDistance && _canShoot) ThrowBomb();
            if (_distance < _chaseDistance && _distance > _attackDistance) Move();
            if (_distance < _attackDistance && _notCooldown) Attack();
        }
    }

    private void Move()
    {
        Vector2 targetPosition = _target.transform.position - transform.position;
        targetPosition = new Vector2(targetPosition.x, 0);
        _rb.velocity = targetPosition.normalized * _speed;
    }

    private void ThrowBomb()
    {
        StartCoroutine(ShootCooldown(_cooldownTime));

        _anim.SetTrigger("ThrowBomb");
    }

    private void ThrowBombAnimationTrigger()
    {
        Vector2 direction;
        GameObject bomb = Instantiate(_bomb, _bombSpawnpoint.position, Quaternion.identity);
        if (_target != null)
        {
            direction = _target.transform.position - transform.position;
            direction = new Vector2(direction.x * 1.5f, direction.y + 3);
        }
        else
        {
            direction = _bombSpawnpoint.position - transform.position;
            direction = new Vector2(direction.x * 80.5f, direction.y + 3);
        }
        
        bomb.GetComponent<Rigidbody2D>().AddForce(direction, ForceMode2D.Impulse);
    }

    private IEnumerator ShootCooldown(float _time)
    {
        _canShoot = false;
        yield return new WaitForSeconds(_time);
        _canShoot = true;
    }

    private void Attack()
    {
        StartCoroutine(AttackCooldown(_cooldownTime));

        _anim.SetTrigger("Attack");
        AttackSound();
    }

    private void AttackAnimationTrigger()
    {
        Collider2D[] _enemies = Physics2D.OverlapAreaAll(_attackPoint1.transform.position, _attackPoint2.transform.position, _playerLayer);
        foreach (Collider2D _enemy in _enemies)
        {
            Vector2 direction = _enemy.transform.position - transform.position;
            direction = new Vector2(direction.x, 1.5f);
            if (_enemy.GetComponent<Rigidbody2D>() != null) _enemy.GetComponent<Rigidbody2D>().AddForce(direction * 5, ForceMode2D.Impulse);
            if (_enemy.GetComponent<ObjectHP>() != null) _enemy.GetComponent<ObjectHP>().Hit(_damage);
        }
    }

    private void AttackSound()
    {
        _audioSource.clip = _attackClips[Random.Range(0, _attackClips.Length)];
        _audioSource.Play();
    }

    private IEnumerator AttackCooldown(float _time)
    {
        _notCooldown = false;
        yield return new WaitForSeconds(_time);
        _notCooldown = true;
    }

    private void Flip(Transform _target)
    {
        bool _isTargetLeft = _target.transform.position.x > transform.position.x;
        if (_isTargetLeft) transform.localScale = new Vector2(-1, 1);
        if (!_isTargetLeft) transform.localScale = new Vector2(1, 1);
    }

    private void SetAnimator()
    {
        _anim.SetFloat("XVelocity", Mathf.Abs(_rb.velocity.x));
        _anim.SetFloat("YVelocity", _rb.velocity.y);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _target = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _target = null;
        }
    }
}