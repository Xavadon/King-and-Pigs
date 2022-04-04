using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] private GameObject[] _items;
    [SerializeField] private GameObject[] _boxParts;
    [SerializeField] private Transform[] _spawnPoints;

    private void Awake()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Box"), LayerMask.NameToLayer("Player"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Box"), LayerMask.NameToLayer("Enemy"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Box"), LayerMask.NameToLayer("Item"), true);
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

        DropItems();
        Destroy(gameObject);
    }

    private void DropItems()
    {
        for (int i = 0; i < _items.Length; i++)
        {
            GameObject item = Instantiate(_items[i], transform.position, Quaternion.identity);
            Vector3 randomDir = new Vector3(Random.Range(-1.5f, 1.5f), 3);
            item.GetComponent<Rigidbody2D>().AddForce(randomDir * Random.Range(1.5f, 1.7f), ForceMode2D.Impulse);
        }
    }
}
