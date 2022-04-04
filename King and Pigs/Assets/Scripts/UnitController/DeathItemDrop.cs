using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathItemDrop : MonoBehaviour
{
    [SerializeField] private GameObject[] _items;
    private bool _itemsDropped;

    private void Update()
    {
        if (GetComponent<ObjectHP>().IsDead == true && !_itemsDropped) DropItems();
    }

    private void DropItems()
    {
        _itemsDropped = true;
        for (int i = 0; i < _items.Length; i++)
        {
            GameObject item = Instantiate(_items[i], transform.position, Quaternion.identity);
            Vector3 randomDir = new Vector3(Random.Range(-1.5f, 1.5f), 3);
            item.GetComponent<Rigidbody2D>().AddForce(randomDir * Random.Range(1.5f, 1.7f), ForceMode2D.Impulse);
        }
    }
}
