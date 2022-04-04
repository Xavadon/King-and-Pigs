using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restarter : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
