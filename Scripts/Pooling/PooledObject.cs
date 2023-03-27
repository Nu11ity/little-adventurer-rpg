using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour, ITimeBehaviour
{
    [SerializeField] private float _duration;
    private Coroutine _timerCoroutine;

    private void OnDisable() => CancelTimer();
    private void OnEnable() => InitializeTimer();
    public void InitializeTimer() => _timerCoroutine = StartCoroutine(Timer());
    public void CancelTimer()
    {
        if (_timerCoroutine != null) StopCoroutine(_timerCoroutine);
    }
    public IEnumerator Timer()
    {
        float timer = _duration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        if (gameObject.activeInHierarchy)
            gameObject.SetActive(false);
    }
}
