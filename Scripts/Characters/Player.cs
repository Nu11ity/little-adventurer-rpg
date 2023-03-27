using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    // slide forward on attacking
    [SerializeField] private float _attackSlideDuration = 0.4f;
    [SerializeField] private float _attackSlideSpeed = 0.06f;
    private float _attackStartTime;
    private float _attackAnimationDuration;

    //general
    public int Coin { get; private set; }
    private PlayerInputMap _inputMap;
    private PlayerVFXManager _vfxManager;

    private void Awake()
    {
        base.Initialize();

        _inputMap = GetComponent<PlayerInputMap>();
        _vfxManager = GetComponent<PlayerVFXManager>();
    }

    private void CalculateMovement()
    {
        if (_inputMap.Action01 && _controller.isGrounded)
        {
            SwitchStateTo(CharacterState.Attacking);
            return;
        }
        else if (_inputMap.Dodge && _controller.isGrounded)
        {
            SwitchStateTo(CharacterState.Slide);
            return;
        }
        //
        _movementVelocity.Set(_inputMap.MoveData.x, 0f, _inputMap.MoveData.y);
        _movementVelocity.Normalize();
        _movementVelocity = Quaternion.Euler(0, -45f, 0) * _movementVelocity;

        _animator.SetFloat("Speed", _movementVelocity.magnitude);

        _movementVelocity *= _moveSpeed * Time.deltaTime;

        if (_movementVelocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_movementVelocity);

        _animator.SetBool("AirBorne", !_controller.isGrounded);
    }

    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case CharacterState.Normal:
                CalculateMovement();
                break;

            case CharacterState.Attacking:
                if (Time.time < _attackStartTime + _attackSlideDuration)
                {
                    float timePassed = Time.time - _attackStartTime;
                    float lerpTime = timePassed / _attackSlideDuration;
                    _movementVelocity = Vector3.Lerp(transform.forward * _attackSlideSpeed, Vector3.zero, lerpTime);
                }

                if (_inputMap.Action01 && _controller.isGrounded)
                {
                    string currentClipName = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                    _attackAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                    if (currentClipName != "LittleAdventurerAndie_ATTACK_03" &&
                        _attackAnimationDuration > 0.5f && _attackAnimationDuration < 0.7f)
                    {
                        //_inputMap.Action01 = false;
                        SwitchStateTo(CharacterState.Attacking);

                        //CalculatePlayerMovement();
                    }
                }
                break;

            case CharacterState.Dead:
                return;

            case CharacterState.BeingHit:
                break;

            case CharacterState.Slide:
                _movementVelocity = transform.forward * _slideSpeed * Time.deltaTime;
                break;

            case CharacterState.Spawn:/*
                _currentSpawnTime -= Time.deltaTime;
                if (_currentSpawnTime <= 0)
                {
                    SwitchStateTo(CharacterState.Normal);
                }*/
                break;
        }

        if (_impactOnCharacter.magnitude > 0.2f)
        {
            _movementVelocity = _impactOnCharacter * Time.deltaTime;
        }
        _impactOnCharacter = Vector3.Lerp(_impactOnCharacter, Vector3.zero, Time.deltaTime * 5);

        if (!_controller.isGrounded)
            _verticalVelocity = _gravity;
        else
            _verticalVelocity = _gravity * 0.3f;

        _movementVelocity += _verticalVelocity * Vector3.up * Time.deltaTime;

        _controller.Move(_movementVelocity);
        _movementVelocity = Vector3.zero;
    }

    public override void SwitchStateTo(CharacterState newState)
    {
        _inputMap.ClearCache();

        //Exiting state
        switch (CurrentState)
        {
            case CharacterState.Normal:
                break;

            case CharacterState.Attacking:
                if (_damageCaster != null)
                    DisableDamageCaster();
                _vfxManager.StopBlade();
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
                _animator.SetTrigger("Attack");
                _attackStartTime = Time.time;
                // ! breaks controller aiming
                RotateToCursor();
                break;

            case CharacterState.Dead:
                _controller.enabled = false;
                _animator.SetTrigger("Dead");
                StartCoroutine(MaterialDissolve());
                break;

            case CharacterState.BeingHit:
                _animator.SetTrigger("BeingHit");
                _isInvincible = true;
                StartCoroutine(DelayCancelInvincible());
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

    private void RotateToCursor()
    {
        // ! temporary way of controller vs pc testing
        if (Input.GetJoystickNames().Length > 0)
        {
            if (_inputMap.DirectionData.magnitude > 0.4f)
            {
                transform.rotation = Quaternion.LookRotation(_inputMap.TouchpadDirection, Vector3.up);
            }
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitResult;

        if (Physics.Raycast(ray, out hitResult, 1000, 1 << LayerMask.NameToLayer("CursorTest")))
        {
            Vector3 cursorPos = hitResult.point;
            transform.rotation = Quaternion.LookRotation(cursorPos - transform.position, Vector3.up);
        }
    }

    public override void ApplyDamage(int damage, Vector3 attackerPos = default)
    {
        base.ApplyDamage(damage, attackerPos);

        StartCoroutine(MaterialBlink());
        SwitchStateTo(CharacterState.BeingHit);
        AddImpact(attackerPos, 10f);
    }

    public void PickUpItem(PickUp item)
    {
        switch (item.Type)
        {
            case PickUp.PickUpType.Heal:
                AddHealth(item.Value);
                break;
            case PickUp.PickUpType.Coin:
                AddCoin(item.Value);
                break;
        }
    }

    protected override void AddHealth(int health)
    {
        base.AddHealth(health);
        _vfxManager.PlayHealVFX();
    }

    private void AddCoin(int coin)
    {
        Coin += coin;
        _vfxManager.PlayCollectVFX();
    }
    public void SlideAnimationEnds() => SwitchStateTo(CharacterState.Normal);
    public void AttackAnimationEnds() => SwitchStateTo(CharacterState.Normal);
    public void BeingHitAnimationEnds() => SwitchStateTo(CharacterState.Normal);
}
