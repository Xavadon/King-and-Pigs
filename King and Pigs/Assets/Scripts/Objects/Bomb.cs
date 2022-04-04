using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private Animator _anim;
    [SerializeField] LayerMask _enemyLayer;
    private bool _exploded;

    private void Awake()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyObjectile"), LayerMask.NameToLayer("Box"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyObjectile"), LayerMask.NameToLayer("Item"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyObjectile"), LayerMask.NameToLayer("EnemyObjectile"), true);
    }

    private void Start()
    {
        _anim = GetComponent<Animator>();
        StartCoroutine(Explode(Random.Range(2.1f, 2.6f)));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            StartCoroutine(Explode(1f));
        }
    }

    private IEnumerator Explode(float _time)
    {
        yield return new WaitForSeconds(_time);
        if (!_exploded)
        {
            _exploded = true;
            _anim.SetTrigger("Explode");
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            GetComponent<Rigidbody2D>().gravityScale = 0;
            GetComponent<Collider2D>().enabled = false;
        }
    }

    private void DestroyAnimationTrigger()
    {
        Destroy(gameObject);
    }
    private void SetDamageAnimationTrigger()
    {
        Collider2D[] _enemies = Physics2D.OverlapCircleAll(transform.position, 0.875f, _enemyLayer);
        foreach(Collider2D _enemy in _enemies)
        {
            Vector2 direction = _enemy.transform.position - transform.position;
            direction = new Vector2(direction.x, 1.5f);
            if (_enemy.GetComponent<ObjectHP>() != null) 
                _enemy.GetComponent<ObjectHP>().Hit(1);
            _enemy.GetComponent<Rigidbody2D>().AddForce(direction * 10, ForceMode2D.Impulse);
        }

    }
}
