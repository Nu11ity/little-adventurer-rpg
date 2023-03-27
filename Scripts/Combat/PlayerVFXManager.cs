using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerVFXManager : MonoBehaviour
{
    [SerializeField] private VisualEffect _footStep;
    [SerializeField] private ParticleSystem _blade01;
    [SerializeField] private ParticleSystem _blade02;
    [SerializeField] private ParticleSystem _blade03;
    [SerializeField] private VisualEffect _slash;
    [SerializeField] private VisualEffect _heal;
    [SerializeField] private ParticleSystem _collect;

    public void PlayBlade01() => _blade01.Play();
    public void PlayBlade02() => _blade02.Play();
    public void PlayBlade03() => _blade03.Play();
    public void PlayHealVFX() => _heal.Play();
    public void PlayCollectVFX() => _collect.Play();
    public void Update_FootStep(bool state)
    {
        if (state)
            _footStep.Play();
        else
            _footStep.Stop();
    }
    public void PlaySlash(Vector3 pos)
    {
        _slash.transform.position = pos;
        _slash.Play();
    }
    public void StopBlade()
    {
        _blade01.Simulate(0);
        _blade01.Stop();

        _blade02.Simulate(0);
        _blade02.Stop();

        _blade03.Simulate(0);
        _blade03.Stop();
    }
}
