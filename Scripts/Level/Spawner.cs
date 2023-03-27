using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    public UnityEvent OnAllSpawnedCharacterEliminated;
    private List<SpawnPoint> _spawnPointList;
    private List<Character> _spawnedCharacters;
    private bool _hasSpawned;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        var spawnPointArray = transform.parent.GetComponentsInChildren<SpawnPoint>();
        _spawnPointList = new List<SpawnPoint>(spawnPointArray);
        _spawnedCharacters = new List<Character>();
    }

    private void Update()
    {
        if (!_hasSpawned || _spawnedCharacters.Count == 0)
            return;

        bool allSpawnedAreDead = true;

        foreach (Character c in _spawnedCharacters)
        {
            if (c.CurrentState != Character.CharacterState.Dead)
            {
                allSpawnedAreDead = false;
                break;
            }
        }

        if (allSpawnedAreDead)
        {
            if (OnAllSpawnedCharacterEliminated != null)
                OnAllSpawnedCharacterEliminated.Invoke();

            _spawnedCharacters.Clear();
        }
    }

    public void SpawnCharacters()
    {
        if (_hasSpawned)
            return;

        _hasSpawned = true;

        foreach (SpawnPoint point in _spawnPointList)
        {
            if (point.EnemyToSpawn != null)
            {
                GameObject spawnedGameObject = Instantiate(point.EnemyToSpawn, point.transform.position, point.transform.rotation);
                _spawnedCharacters.Add(spawnedGameObject.GetComponent<Character>());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SpawnCharacters();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _collider.bounds.size);
    }
}
