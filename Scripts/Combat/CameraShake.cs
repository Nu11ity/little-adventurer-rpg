using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }
    [SerializeField] private float _shakeIntensity = 1f;
    [SerializeField] private float _shakeTime = 0.2f;
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin _cbmcp;
    private float _timer;

    private void Awake()
    {
        if (CameraShake.Instance == null)
        {
            CameraShake.Instance = this;
        }
        else if (CameraShake.Instance != this)
        {
            Destroy(this);
        }

        _cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        _cbmcp = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera()
    {
        _cbmcp.m_AmplitudeGain = _shakeIntensity;
        _timer = _shakeTime;
    }

    private void StopShake()
    {
        _cbmcp.m_AmplitudeGain = 0;
        _timer = 0;
    }

    private void Update()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0)
            {
                StopShake();
            }
        }
    }
}
