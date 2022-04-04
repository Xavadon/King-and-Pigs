using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInBox : MonoBehaviour
{
    [SerializeField] private GameObject[] _boxParts;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject _enemy;

    private void Awake()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Box"), LayerMask.NameToLayer("Player"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Box"), LayerMask.NameToLayer("Enemy"), true);
    }
    public void Death()
    {
        GetComponent<Animator>().SetTrigger("Hit");
    }

    private void DeathTrigger()
    {
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            GameObject boxPart = Instantiate(_boxParts[i], _spawnPoints[i].position, Quaternion.identity);
            Vector2 direction = _spawnPoints[i].position - transform.position;
            boxPart.GetComponent<Rigidbody2D>().AddForce(direction.normalized * 2, ForceMode2D.Impulse);
        }

        SpawnEnemy();
        Destroy(gameObject);
    }

    private void SpawnEnemy()
    {
        GameObject enemy = Instantiate(_enemy, transform.position, Quaternion.identity);
        enemy.GetComponent<ObjectHP>().Hit(0);
    }
}
