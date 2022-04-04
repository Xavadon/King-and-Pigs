using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    void Update()
    {
        Vector2 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerPos = new Vector3(_target.position.x, _target.position.y, -10);
        transform.position = Vector3.Lerp(playerPos, cameraPos, 0.15f);
    }
}
