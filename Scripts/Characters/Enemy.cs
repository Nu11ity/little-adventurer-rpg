using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private Transform _targetPlayer;
    [SerializeField] private GameObject _itemToDrop;

    private void Awake()
    {
        base.Initialize();

        _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _targetPlayer = GameObject.FindWithTag("Player").transform;
        _navMeshAgent.speed = _moveSpeed;
        SwitchStateTo(CharacterState.Spawn);
    }

    private void CalculateMovement()
    {
        if (Vector3.Distance(_targetPlayer.position, transform.position) >= _navMeshAgent.stoppingDistance)
        {
            _navMeshAgent.SetDestination(_targetPlayer.position);
            _animator.SetFloat("Speed", 0.2f);
        }
        else
        {
            _navMeshAgent.SetDestination(transform.position);
            _animator.SetFloat("Speed", 0f);

            SwitchStateTo(CharacterState.Attacking);
        }
    }

    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case CharacterState.Normal:
                CalculateMovement();
                break;

            case CharacterState.Attacking:
                break;

            case CharacterState.Dead:
                return;

            case CharacterState.BeingHit:
                break;

            case CharacterState.Slide:
                _movementVelocity = transform.forward * _slideSpeed * Time.deltaTime;
                break;

            case CharacterState.Spawn:
                _currentSpawnTime -= Time.deltaTime;
                if (_currentSpawnTime <= 0)
                {
                    SwitchStateTo(CharacterState.Normal);
                }
                break;
        }

        if (_impactOnCharacter.magnitude > 0.2f)
        {
            _movementVelocity = _impactOnCharacter * Time.deltaTime;
        }
        _impactOnCharacter = Vector3.Lerp(_impactOnCharacter, Vector3.zero, Time.deltaTime * 5);

        if (CurrentState != CharacterState.Normal)
        {
            _controller.Move(_movementVelocity);
            _movementVelocity = Vector3.zero;
        }
    }

    public override void SwitchStateTo(CharacterState newState)
    {
        //Exiting state
        switch (CurrentState)
        {
            case CharacterState.Normal:
                break;

            case CharacterState.Attacking:
                if (_damageCaster != null)
                    DisableDamageCaster();
                break;

            case CharacterState.Dead:
                return;

            case CharacterState.BeingHit:
                break;

            case CharacterState.Slide:
                break;

            case CharacterState.Spawn:
                _isInvincible = false;
                break;
        }

        //Entering state
        switch (newState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                Quaternion newRotation = Quaternion.LookRotation(_targetPlayer.position - transform.position);
                transform.rotation = newRotation;
                _animator.SetTrigger("Attack");
                break;

            case CharacterState.Dead:
                _controller.enabled = false;
                _animator.SetTrigger("Dead");
                StartCoroutine(MaterialDissolve());
                SkinnedMeshRenderer mesh = GetComponentInChildren<SkinnedMeshRenderer>();
                mesh.gameObject.layer = 0;
                break;

            case CharacterState.BeingHit:
                _animator.SetTrigger("BeingHit");
                break;

            case CharacterState.Slide:
                _animator.SetTrigger("Slide");
                break;

            case CharacterState.Spawn:
                _isInvincible = true;
                _currentSpawnTime = _spawnDuration;
                StartCoroutine(MaterialAppear());
                break;
        }
        CurrentState = newState;
        Debug.Log("Switching to " + CurrentState);
    }

    public override void ApplyDamage(int damage, Vector3 attackerPos = default)
    {
        base.ApplyDamage(damage, attackerPos);

        GetComponent<EnemyVFXManager>().PlayBeingHitVFX(attackerPos);
        StartCoroutine(MaterialBlink());
        AddImpact(attackerPos, 2.5f);
    }

    public void DropItem()
    {
        if (_itemToDrop != null)
            Instantiate(_itemToDrop, transform.position, Quaternion.identity);
    }

    protected override void OnMaterialDissolveOver()
    {
        base.OnMaterialDissolveOver();
        DropItem();
    }

    public void RotateToTarget()
    {
        if (CurrentState != CharacterState.Dead)
        {
            transform.LookAt(_targetPlayer, Vector3.up);
        }
    }
    public void SlideAnimationEnds() => SwitchStateTo(CharacterState.Normal);
    public void AttackAnimationEnds() => SwitchStateTo(CharacterState.Normal);
    public void BeingHitAnimationEnds() => SwitchStateTo(CharacterState.Normal);
}
