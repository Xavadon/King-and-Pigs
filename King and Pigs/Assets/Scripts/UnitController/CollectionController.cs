using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionController : MonoBehaviour
{
    [SerializeField] private GameObject _popupText;
    private int _score;

    [SerializeField] private Text[] _scoreText;

    private void Update()
    {
        for (int i = 0; i < _scoreText.Length; i++)
        {
            _scoreText[i].text = _score.ToString();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {   
        if(collision.tag == "Diamond" && collision.GetComponent<ItemDestroy>()._canDestroy)
        {
            SetComponents(collision);
            _score += 100;
            SpawnPopupText();
        }
        if(collision.tag == "Heart" && collision.GetComponent<ItemDestroy>()._canDestroy)
        {
            SetComponents(collision);
            GetComponentInParent<ObjectHP>().Heal();
        }
    }

    private void SpawnPopupText()
    {
        Instantiate(_popupText, transform.position, Quaternion.identity);
    }

    private void SetComponents(Collider2D collision)
    {
        collision.GetComponent<Animator>().SetTrigger("PickUp");
        collision.GetComponent<ItemDestroy>().PlaySound();
        collision.GetComponent<Rigidbody2D>().gravityScale = 0;
        collision.GetComponent<Collider2D>().enabled = false;
    }
}
