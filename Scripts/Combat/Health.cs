using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    public float CurrentHealthPercentage
    {
        get
        {
            return (float)_currentHealth / (float)_maxHealth;
        }
    }
    private Character _character;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _character = GetComponent<Character>();
    }

    public void ApplyDamage(int damage)
    {
        _currentHealth -= damage;
        Debug.Log(gameObject.name + "took damage: " + damage);
        Debug.Log(gameObject.name + " currentHealth: " + _currentHealth);

        CameraShake.Instance.ShakeCamera();

        CheckHealth();
    }

    private void CheckHealth()
    {
        if (_currentHealth <= 0)
        {
            _character.SwitchStateTo(Character.CharacterState.Dead);
        }
    }

    public void AddHealth(int health)
    {
        _currentHealth += health;

        if (_currentHealth > _maxHealth)
            _currentHealth = _maxHealth;
    }
}
