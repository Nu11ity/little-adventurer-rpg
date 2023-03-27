using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("State Data")]
    public CharacterState CurrentState;
    public enum CharacterState
    {
        Normal, Attacking, Dead, BeingHit, Slide, Spawn
    }

    [Header("Locomotion Data")]
    [SerializeField] protected float _moveSpeed = 5f;
    [SerializeField] protected float _gravity = -9.8f;
    protected CharacterController _controller;
    protected Vector3 _movementVelocity;
    protected float _verticalVelocity;

    [Header("Spawn Data")]
    [SerializeField] protected float _spawnDuration = 2f;
    protected float _currentSpawnTime;
    private MaterialPropertyBlock _materialPropertyBlock;
    private SkinnedMeshRenderer _skinnedMeshRenderer;

    [Header("Combat Data")]
    [SerializeField] protected bool _isInvincible;
    [SerializeField] private float _invincibleDuration = 2f;
    [SerializeField] protected float _slideSpeed = 9f;
    protected Vector3 _impactOnCharacter;

    // general components
    private Health _health;
    protected DamageCaster _damageCaster;
    protected Animator _animator;

    protected void Initialize()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _damageCaster = GetComponentInChildren<DamageCaster>();

        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);
    }
    public virtual void SwitchStateTo(CharacterState newState) { }

    #region Combat Methods
    public void EnableDamageCaster() => _damageCaster.EnableDamageCaster();
    public void DisableDamageCaster() => _damageCaster.DisableDamageCaster();
    public virtual void ApplyDamage(int damage, Vector3 attackerPos = new Vector3())
    {
        if (_isInvincible)
            return;

        if (_health != null)
        {
            _health.ApplyDamage(damage);
        }
    }
    protected virtual void AddHealth(int health)
    {
        _health.AddHealth(health);
    }
    protected IEnumerator DelayCancelInvincible()
    {
        yield return new WaitForSeconds(_invincibleDuration);
        _isInvincible = false;
    }

    protected void AddImpact(Vector3 attackerPos, float force)
    {
        Vector3 impactDir = transform.position - attackerPos;
        impactDir.Normalize();
        impactDir.y = 0;
        _impactOnCharacter = impactDir * force;
    }
    #endregion

    #region Material Behavior
    protected IEnumerator MaterialBlink()
    {
        _materialPropertyBlock.SetFloat("_blink", 0.4f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        yield return new WaitForSeconds(0.2f);

        _materialPropertyBlock.SetFloat("_blink", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
    protected virtual void OnMaterialDissolveOver()
    {
        Debug.Log("Dissolve step ended, do something?");
    }
    protected IEnumerator MaterialDissolve()
    {
        yield return new WaitForSeconds(2);

        float dissolveTimeDuration = 2f;
        float currentDissolveTime = 0;
        float dissolveHeight_start = 20f;
        float dissolveHeight_target = -10f;
        float dissolveHeight;

        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHeight = Mathf.Lerp(dissolveHeight_start, dissolveHeight_target, currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHeight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }
        OnMaterialDissolveOver();
    }
    protected IEnumerator MaterialAppear()
    {
        float dissolveTimeDuration = _spawnDuration;
        float currentDissolveTime = 0;
        float dissolveHeight_start = -10f;
        float dissolveHeight_target = 20f;
        float dissolveHeight;

        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHeight = Mathf.Lerp(dissolveHeight_start, dissolveHeight_target, currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHeight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }

        _materialPropertyBlock.SetFloat("_enableDissolve", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
    #endregion
}
