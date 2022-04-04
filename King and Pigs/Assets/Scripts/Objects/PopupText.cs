using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupText : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(DestroyObject());
    }
    private void Update()
    {
        transform.Translate(transform.up * 3 * Time.deltaTime);
    }

    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
    }
}
