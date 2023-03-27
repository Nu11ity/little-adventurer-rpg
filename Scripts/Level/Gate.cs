using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] private GameObject _gateVisual;
    [SerializeField] private float _openDuration = 2f;
    [SerializeField] private float _openTargetY = -2;
    private Collider _gateCollider;

    private void Awake()
    {
        _gateCollider = GetComponent<Collider>();
    }

    IEnumerator OpenGateAnimation()
    {
        float currentOpenDuration = 0;
        Vector3 startPos = _gateVisual.transform.position;
        Vector3 targetPos = startPos + Vector3.up * _openTargetY;

        while (currentOpenDuration < _openDuration)
        {
            currentOpenDuration += Time.deltaTime;
            _gateVisual.transform.position = Vector3.Lerp(startPos, targetPos, currentOpenDuration / _openDuration);

            yield return null;
        }

        _gateCollider.enabled = false;
    }

    public void Open()
    {
        StartCoroutine(OpenGateAnimation());
    }
}
