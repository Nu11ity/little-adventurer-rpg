using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyVFXManager : MonoBehaviour
{
    [SerializeField] private VisualEffect _footStep;
    [SerializeField] private VisualEffect _attackVFX;
    [SerializeField] private ParticleSystem _beingHitVFX;

    public void BurstFootStep() => _footStep.SendEvent("OnPlay");
    public void PlayAttackVFX() => _attackVFX.SendEvent("OnPlay");
    public void PlayBeingHitVFX(Vector3 attackerPos)
    {
        Vector3 forceForward = transform.position - attackerPos;
        forceForward.Normalize();
        forceForward.y = 0;
        _beingHitVFX.transform.rotation = Quaternion.LookRotation(forceForward);
        _beingHitVFX.Play();
        // (!!)Change this method to use object pooling 
        Vector3 splashPos = transform.position;
        splashPos.y += 2f;
        ObjectPooler.Instance.RequestFromPool(ObjectPooler.PoolIdentifier.E_Blood, splashPos, Quaternion.identity);
    }
}
