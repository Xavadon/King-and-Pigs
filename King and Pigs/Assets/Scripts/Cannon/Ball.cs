using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float _force;
    private void Awake()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyObjectile"), LayerMask.NameToLayer("EnemyObjectile"), true);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            Destroy(gameObject);
        }
        if (collision.TryGetComponent<ObjectHP>(out ObjectHP hp))
        {
            Destroy(gameObject);
            collision.GetComponent<ObjectHP>().Hit(1);
            collision.GetComponent<Rigidbody2D>().AddForce(new Vector2(_force, 0), ForceMode2D.Impulse);
        }
    }
}
