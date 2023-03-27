using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOrb : PooledObject
{
    [SerializeField] private float _speed = 2f;
    [SerializeField] private int _damage = 10;
    private Rigidbody _rb;

    private void Awake() => _rb = GetComponent<Rigidbody>();

    private void FixedUpdate()
    {
        _rb.MovePosition(transform.position + transform.forward * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();

        if (player != null)
        {
            player.ApplyDamage(_damage, transform.position);
        }

        ObjectPooler.Instance.RequestFromPool(ObjectPooler.PoolIdentifier.E_Impact, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
