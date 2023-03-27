using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWeapons : MonoBehaviour
{
    [SerializeField] private List<GameObject> _weapons;

    public void DropSwords()
    {
        for (int i = 0; i < _weapons.Count; i++)
        {
            _weapons[i].AddComponent<Rigidbody>();
            _weapons[i].AddComponent<BoxCollider>();
            _weapons[i].transform.parent = null;
        }
    }
}
