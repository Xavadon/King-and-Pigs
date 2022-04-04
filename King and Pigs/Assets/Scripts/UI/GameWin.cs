using System.Collections;
using UnityEngine;

public class GameWin : MonoBehaviour
{
    [SerializeField] private GameObject _winPanel;
    private bool _isGameWin;
    private void Update()
    {
        if (GetComponent<ObjectHP>().IsDead && !_isGameWin) StartCoroutine(GameWinCor());
    }

    private IEnumerator GameWinCor()
    {
        _isGameWin = true;
        yield return new WaitForSeconds(5);
        _winPanel.SetActive(true);
        Time.timeScale = 0;
    }
}
