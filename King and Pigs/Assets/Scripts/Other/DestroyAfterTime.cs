using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float _time;
    private void Start()
    {
        Invoke("DestroyThis", _time);
    }
    
    private void DestroyThis()
    {
        Destroy(gameObject);
    }
}
