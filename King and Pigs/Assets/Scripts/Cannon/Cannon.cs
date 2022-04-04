using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cannon : MonoBehaviour
{
    [SerializeField] private Transform _ballSpawnpoint;
    [SerializeField] private GameObject _ball;
    [SerializeField] private float _shootForce;
    [SerializeField] private float _cooldownTime;
    [SerializeField] private UnityEvent ShootSound;

    private Animator _anim;
    private int _direction;

    private bool _isTargetNear;
    private bool _canShoot = true;

    public bool CanShoot;

    private void Start()
    {
        if(_shootForce > 0) _direction = -1;
        if(_shootForce < 0) _direction = 1;

        transform.localScale = new Vector3(_direction, 1, 1);
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        CanShoot = _isTargetNear && _canShoot;
    }

    public void Shoot()
    {
        if (_canShoot)
        {
            StartCoroutine(ShootCooldown());

            GameObject Ball = Instantiate(_ball, _ballSpawnpoint.position, Quaternion.identity);
            Ball.GetComponent<Rigidbody2D>().AddForce(_shootForce * Vector2.right, ForceMode2D.Impulse);
            Ball.transform.localScale = new Vector3(_direction, 1, 1);
            Ball.GetComponent<Ball>()._force = _shootForce;

            _anim.SetTrigger("Shoot");

            ShootSound.Invoke();
        }
    }

    private IEnumerator ShootCooldown()
    {
        _canShoot = false;
        yield return new WaitForSeconds(_cooldownTime);
        _canShoot = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player") _isTargetNear = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player") _isTargetNear = false;
    }
}
