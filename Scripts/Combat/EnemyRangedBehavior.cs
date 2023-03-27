using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedBehavior : MonoBehaviour
{
    [SerializeField] private Transform _shootingPoint;
    private Enemy _enemy;

    private void Awake() => _enemy = GetComponent<Enemy>();
    private void Update() => _enemy.RotateToTarget();
    public void ShootTheDamageOrb()
    {
        ObjectPooler.Instance.RequestFromPool(ObjectPooler.PoolIdentifier.E_Projectile, _shootingPoint.position, Quaternion.LookRotation(_shootingPoint.forward));
    }
}
