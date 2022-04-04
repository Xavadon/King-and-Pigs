using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _damage;
    [SerializeField] private float _jumpCooldownTime;
    
    private Rigidbody2D _rb;

    private bool _canJump => Input.GetButton("Jump") && _onGround && !_jumpCooldown && Input.GetAxisRaw("Vertical") != -1;
    private bool _onGround;
    private bool _notGround;
    private bool _jumpCooldown;

    private bool _jumpDown => Input.GetAxisRaw("Vertical") == -1 && Input.GetButtonDown("Jump");
    private bool _sitDown => Input.GetAxisRaw("Vertical") == -1;

    private bool _canAttack => Input.GetButtonDown("Fire1") && _notCooldown;
    private bool _notCooldown = true;
    private bool _isDead => GetComponent<ObjectHP>().IsDead;
    private bool _gotDamage => GetComponent<ObjectHP>().GotDamage;

    [SerializeField] private float _cooldownTime;

    [Header("Animation")]
    [SerializeField] private Animator _anim;

    [Header("Attack")]
    [SerializeField] private GameObject _attackPoint1;
    [SerializeField] private GameObject _attackPoint2;
    [SerializeField] private LayerMask _enemyLayer;

    [Header("Attack Sound")]
    [SerializeField] private AudioClip[] _attackClips;
    private AudioSource _audioSource;

    [Header("Ground Raycast")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundRaycastLength;
    [SerializeField] private Vector3 _groundRaycastOffset;


    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!_isDead && !_gotDamage)
        {
            Move();
            Flip();
            if (_canAttack) Attack();
            SetAnimator();
            SetJumpVelocity();
            if (_jumpDown) PlatformJumpDown();
        }
    }
    private void FixedUpdate()
    {
        CheckGround();
        if (_canJump &&(!_isDead && !_gotDamage)) Jump();
    }

    private void Move()
    {
        Vector2 _movement = new Vector2(GetInput().x * _speed, _rb.velocity.y);
        _rb.velocity = _movement;
    }

    private void SetJumpVelocity()
    {
        if(_rb.velocity.y > 0)
        {
            if (Input.GetButton("Jump"))
            {
                _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y);
            }
            else _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.5f);
        }
        else _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y);
    }

    private void Jump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, 0);
        _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        StartCoroutine(JumpCooldown());

        _anim.SetTrigger("Jump");
    }

    private void PlatformJumpDown()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Platform"), true);
        Invoke("IgnoreLayerOff", 0.25f);

        _anim.SetTrigger("JumpDown");
    }

    private void IgnoreLayerOff()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Platform"), false);

    }

    private IEnumerator JumpCooldown()
    {
        _jumpCooldown = true;
        yield return new WaitForSeconds(_jumpCooldownTime);
        _jumpCooldown = false;
    }

    private void Attack()
    {
        Collider2D[] _enemies = Physics2D.OverlapAreaAll(_attackPoint1.transform.position, _attackPoint2.transform.position, _enemyLayer);
        foreach (Collider2D _enemy in _enemies)
        {
            if(_enemy.tag == "CannonBall")
            {
                ChangeCannonBallDirection(_enemy);
            }
            else if(_enemy.tag == "Bomb")
            {
                SetAttackImpactToBomb(_enemy);
            }
            else if(_enemy.tag == "Box")
            {
                if(_enemy.GetComponent<Box>()) _enemy.GetComponent<Box>().Death();
                if (_enemy.GetComponent<EnemyInBox>()) _enemy.GetComponent<EnemyInBox>().Death();
            }
            else
            {
                SetAttackImpact(_enemy);
                _enemy.GetComponent<ObjectHP>().Hit(_damage);
            }
        }
        StartCoroutine(AttackCooldown(_cooldownTime));

        _anim.SetTrigger("Attack");
        AttackSound();
    }

    private void ChangeCannonBallDirection(Collider2D _enemy)
    {
        Rigidbody2D _enemyRb = _enemy.GetComponent<Rigidbody2D>();
        _enemyRb.velocity = new Vector2(_enemyRb.velocity.x * -1, _enemyRb.velocity.y);
        _enemy.GetComponent<Ball>()._force *= -1;
    }

    private void SetAttackImpactToBomb(Collider2D _enemy)
    {
        Vector2 direction = (_enemy.transform.position - transform.position).normalized;
        direction = new Vector2(direction.x, 0.3f);
        _enemy.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        _enemy.GetComponent<Rigidbody2D>().AddForce(direction * 8, ForceMode2D.Impulse);
    }

    private void SetAttackImpact(Collider2D _enemy)
    {
        Vector2 direction = _enemy.transform.position - transform.position;
        direction = new Vector2(direction.x, 1.5f);
        _enemy.GetComponent<Rigidbody2D>().AddForce(direction * 10, ForceMode2D.Impulse);
    }

    private void AttackSound()
    {
        _audioSource.clip = _attackClips[Random.Range(0, _attackClips.Length)];
        _audioSource.Play();
    }

    private void Flip()
    {
        if(GetInput().x > 0)
        transform.localScale = new Vector2(1, 1);
        if(GetInput().x < 0)
        transform.localScale = new Vector2(-1, 1);
    }

    private IEnumerator AttackCooldown(float _time)
    {
        _notCooldown = false;
        yield return new WaitForSeconds(_time);
        _notCooldown = true;
    }

    private Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), 0);
    }

    private void CheckGround()
    {
        bool _checkGround = Physics2D.Raycast(transform.position + _groundRaycastOffset, Vector2.down, _groundRaycastLength, _groundLayer) ||
        Physics2D.Raycast(transform.position - _groundRaycastOffset, Vector2.down, _groundRaycastLength, _groundLayer);

        if (_checkGround)
        {
            _onGround = true;
            _notGround = false;
        }
        else if(!_notGround) Invoke("SetGroundFalse", 0.1f);

        _anim.SetBool("OnGround", _onGround);
    }

    private void SetGroundFalse()
    {
        _notGround = true;
        _onGround = false;
    }

    private void SetAnimator()
    {
        _anim.SetFloat("XVelocity", Mathf.Abs(_rb.velocity.x));
        _anim.SetFloat("YVelocity", _rb.velocity.y);
        _anim.SetBool("SitDown", _sitDown);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + _groundRaycastOffset, transform.position + _groundRaycastOffset + Vector3.down * _groundRaycastLength);
        Gizmos.DrawLine(transform.position - _groundRaycastOffset, transform.position - _groundRaycastOffset + Vector3.down * _groundRaycastLength);
    }
}
