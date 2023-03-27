using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public GameObject prefab;
    public int size;
    public bool canGrow;
    public Transform poolParent;
    public List<GameObject> pooledObjects = new List<GameObject>();
}
