using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITimeBehaviour
{
    public IEnumerator Timer();
    public void CancelTimer();
    public void InitializeTimer();
}
