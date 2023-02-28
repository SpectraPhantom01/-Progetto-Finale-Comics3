using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashingCharacterState : State
{
    private PlayerController m_Owner;

    private float _dashingPower;
    private float _dashingTime;
    private float _dashingCooldown; //Non utilizzato, coroutine in PlayerController

    private float _timeElapsed;
    Vector2 _direction;

    private Vector2 _position;

    public DashingCharacterState(PlayerController owner)
    {
        m_Owner = owner;
    }

    public override void OnStart()
    {
        //Debug.Log("Sono in dashing");

        _timeElapsed = 0;

        _dashingPower = m_Owner.dashingPower;
        _dashingTime = m_Owner.dashingTime;
        _dashingCooldown = m_Owner.dashingCooldown;

        _direction = m_Owner.Direction.normalized;

        m_Owner.CanDash = false;
        m_Owner.IsDashing = true;

        _position = m_Owner.transform.position;
    }

    public override void OnEnd() //Da interrompere il dash quando va contro un muro
    {

        m_Owner.IsDashing = false;
        m_Owner.StartCoroutine(m_Owner.DashCooldownRoutine());

        //Debug.Log(_position);
        //Debug.Log(m_Owner.transform.position);

        Debug.Log(Vector2.Distance(_position,(Vector2)m_Owner.transform.position));
    }

    public override void OnFixedUpdate() 
    {
        m_Owner.Rigidbody.velocity = _direction * _dashingPower;
    }

    public override void OnUpdate() 
    {       
        DashingTimer();
    }

    private void DashingTimer()
    {
        _timeElapsed += Time.deltaTime;

        if (_timeElapsed > _dashingTime)
        {
            m_Owner.StateMachine.SetState(EPlayerState.Idle);
        }
    }
}
