using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputMap : MonoBehaviour
{
    public PlayerInputs playerInputs;
    public Vector2 MoveData { get; private set; }
    public Vector2 DirectionData { get; private set; }
    public bool Dodge { get; private set; }
    public bool Action01 { get; private set; }

    private void Awake() => playerInputs = new PlayerInputs();
    private void OnEnable() => playerInputs.Enable();
    private void OnDisable()
    {
        playerInputs.Disable();
        ClearCache();
    }
    public void ClearCache()
    {
        Action01 = false;
        Dodge = false;
        MoveData = Vector2.zero;
    }
    private void Update()
    {
        if (Time.timeScale != 0)
        {
            if (!Action01)
            {
                Action01 = playerInputs.combat.action01.triggered;
            }
            if (!Dodge)
            {
                Dodge = playerInputs.combat.dodge.triggered;
            }
        }
        MoveData = playerInputs.locomotion.move.ReadValue<Vector2>();
        DirectionData = playerInputs.locomotion.direction.ReadValue<Vector2>();
    }

    public Vector3 TouchpadDirection
    {
        get
        {
            Vector3 forward = new Vector3(DirectionData.x, 0, DirectionData.y).normalized;
            forward = Quaternion.Euler(0, -45f, 0) * forward;
            return forward.normalized;
        }
    }
}
