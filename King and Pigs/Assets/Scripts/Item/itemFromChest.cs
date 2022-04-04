using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemFromChest : MonoBehaviour
{
    private Collider2D _collider;
    private void Awake()
    {

        _collider = GetComponent<Collider2D>();
        _collider.enabled = false;
        StartCoroutine(EnableCollider());
    }

    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.4f);
        _collider.enabled = true;
    }
}
