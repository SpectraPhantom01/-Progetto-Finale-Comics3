using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashingCharacterState : State
{
    private PlayerController m_Owner;

    private float _dashingPower;
    private float _dashingTime;
    private float _dashingCooldown;

    public DashingCharacterState(PlayerController owner)
    {
        m_Owner = owner;
    }

    public override void OnStart()
    {
        Debug.Log("Sono in dashing");

        _dashingPower = m_Owner.dashingPower;
        _dashingTime = m_Owner.dashingTime;
        _dashingCooldown = m_Owner.dashingCooldown;

        m_Owner.CanDash = false;
        m_Owner.IsDashing = true;
    }

    public override void OnEnd()
    {
        m_Owner.CanDash = true;
        m_Owner.IsDashing = false;
    }

    public override void OnFixedUpdate() //Introdurre timer, non si possono usare le coroutine
    {
        m_Owner.Rigidbody.velocity = m_Owner.Direction.normalized * _dashingPower;
    }

    public override void OnUpdate()
    {
        throw new System.NotImplementedException();
    }
}
