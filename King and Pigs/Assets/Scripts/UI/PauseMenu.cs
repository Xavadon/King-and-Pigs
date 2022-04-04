using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    private bool _isMenuEnabled;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isMenuEnabled)
            {
                _isMenuEnabled = false;
                _pauseMenu.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                _isMenuEnabled = true;
                _pauseMenu.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }
}
