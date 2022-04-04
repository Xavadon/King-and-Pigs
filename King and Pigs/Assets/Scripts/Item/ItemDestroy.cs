using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemDestroy : MonoBehaviour
{
    public bool _canDestroy;
    [SerializeField] private GameObject _pickUpSound;

    private void Awake()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Item"), LayerMask.NameToLayer("Item"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Item"), LayerMask.NameToLayer("Player"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Item"), LayerMask.NameToLayer("Enemy"), true);
        StartCoroutine(SetDestroyTrue());
    }
    private void DestroyOnAnimationTrigger()
    {
        Destroy(gameObject);
    }
    private IEnumerator SetDestroyTrue()
    {
        yield return new WaitForSeconds(0.7f);
        _canDestroy = true;
    }
    public void PlaySound()
    {
        GameObject sound = Instantiate(_pickUpSound, transform.position, Quaternion.identity);
        sound.GetComponent<AudioSource>().pitch = Random.Range(0.95f, 1.05f);
    }
}
