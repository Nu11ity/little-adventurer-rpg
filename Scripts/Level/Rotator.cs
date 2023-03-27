using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float _speed = 80f;

    void Update()
    {
        transform.Rotate(new Vector3(0f, _speed * Time.deltaTime, 0f), Space.World);
    }
}
