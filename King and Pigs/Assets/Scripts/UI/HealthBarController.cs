using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    private GameObject _player;
    [SerializeField] private GameObject[] _hearts;
    private float _currentHp;
    private void Update()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            SetCurrentHp();
        }
    }

    private void SetCurrentHp()
    {
        _currentHp = _player.GetComponent<ObjectHP>().CurrentHp;

        if (_currentHp == 3) foreach (GameObject _heart in _hearts) _heart.SetActive(true);
        if (_currentHp == 2)
        {
            _hearts[0].SetActive(true);
            _hearts[1].SetActive(true);
            _hearts[2].GetComponent<Animator>().SetTrigger("Destroy");
        }
        if (_currentHp == 1)
        {
            _hearts[0].SetActive(true);
            _hearts[1].GetComponent<Animator>().SetTrigger("Destroy");
            _hearts[2].SetActive(false);
        }
        if (_currentHp == 0)
        {
            _hearts[0].GetComponent<Animator>().SetTrigger("Destroy");
            _hearts[1].SetActive(false);
            _hearts[2].SetActive(false);
        }
    }
}
