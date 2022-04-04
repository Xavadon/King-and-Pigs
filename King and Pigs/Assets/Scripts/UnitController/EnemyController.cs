using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _speed;
    [SerializeField] private float _damage;

    [SerializeField] private float _attackDistance;
    [SerializeField] private float _cooldownTime;
    [SerializeField] private Animator _anim;

    [Header("Attack")]
    [SerializeField] private GameObject _attackPoint1;
    [SerializeField] private GameObject _attackPoint2;
    [SerializeField] private LayerMask _playerLayer;

    [Header("Patrol")]
    [SerializeField] private bool _isPatrolUnit;
    [SerializeField] private GameObject[] _patrolPoints;
    private int _currentPoint;

    [Header("Cannon")]
    [SerializeField] private bool _isCannonUnit;
    [SerializeField] private GameObject _cannon;
    [SerializeField] private Transform _cannonStayPoint;

    [Header("Sound")]
    [SerializeField] private AudioClip[] _attackClips;
    private AudioSource _audioSource;

    private Rigidbody2D _rb;
    private GameObject _target;
    private bool _notCooldown = true;

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
                Flip(_target);
                Chase();

                _anim.SetBool("IsCannonUnit", false);                
            }
            if (_target == null && _isPatrolUnit) 
                Patrol();
            if (_target == null && _isCannonUnit)
            {
                Flip(_cannon);
                DefendPoint();
            }           
        }
    }

    private void Patrol()
    {
        GameObject target = _patrolPoints[_currentPoint];
        Vector2 targetPos = new Vector2(target.transform.position.x, transform.position.y);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * 0.5f * Time.deltaTime);

        Flip(target);

        if (transform.position.x == target.transform.position.x)
        {
            _currentPoint++;
            if (_currentPoint >= _patrolPoints.Length) _currentPoint = 0;
        }

        _anim.SetFloat("XVelocity", 1);
    }

    private void DefendPoint()
    {
        bool _canShoot = _cannon.GetComponent<Cannon>().CanShoot;
        if ((transform.position.x + 0.2f > _cannonStayPoint.position.x) && (transform.position.x - 0.2f < _cannonStayPoint.position.x))
        {
            if (_canShoot) _anim.SetTrigger("Shoot");

            _anim.SetBool("IsCannonUnit", true);
        }
        else
        {
            GoToStayPoint(_cannonStayPoint);
        }
    }

    private void ShootAnimationTrigger()
    {
        _cannon.GetComponent<Cannon>().Shoot();
    }

    private void GoToStayPoint(Transform staypoint)
    {
        if (transform.position.x != staypoint.position.x)
        {
            Vector2 targetPos = new Vector2(staypoint.transform.position.x, transform.position.y);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * 0.5f * Time.deltaTime);

            _anim.SetFloat("XVelocity", 1);
        }
    }

    private void Chase()
    {
        Vector2 _pos = new Vector2(transform.position.x,0);
        Vector2 _targetPos = new Vector2(_target.transform.position.x, 0);

        float _distance = Vector2.Distance(_pos, _targetPos);
        if (!_gotDamage)
        {
            if (_distance > _attackDistance) Move();
            if (_distance < _attackDistance && _notCooldown) Attack();
        }
    }

    private void Move()
    {
        Vector2 targetPosition = _target.transform.position - transform.position;
        targetPosition = new Vector2(targetPosition.x, 0);
        _rb.velocity = targetPosition.normalized * _speed;
    }

    private void Attack()
    {
        StartCoroutine(AttackCooldown(_cooldownTime));

        _anim.SetTrigger("Attack");
        AttackSound();
    }

    private void AttackSound()
    {
        _audioSource.clip = _attackClips[Random.Range(0, _attackClips.Length)];
        _audioSource.Play();
    }

    private void AttackAnimationTrigger()
    {
        Collider2D[] _enemies = Physics2D.OverlapAreaAll(_attackPoint1.transform.position, _attackPoint2.transform.position, _playerLayer);
        foreach (Collider2D _enemy in _enemies)
        {
            if (_enemy.GetComponent<Rigidbody2D>() != null)
            {
                Vector2 direction = _enemy.transform.position - transform.position;
                direction = new Vector2(direction.x, 1.5f);
                _enemy.GetComponent<Rigidbody2D>().AddForce(direction * 5, ForceMode2D.Impulse);
                _enemy.GetComponent<ObjectHP>().Hit(_damage);
            }
        }
    }

    private IEnumerator AttackCooldown(float _time)
    {
        _notCooldown = false;
        yield return new WaitForSeconds(_time);
        _notCooldown = true;
    }

    private void Flip(GameObject _target)
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