using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;

    private void Awake()
    {
        Time.timeScale = 0;
    }
    public void StartGame()
    {
        _pauseMenu.SetActive(true);
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
