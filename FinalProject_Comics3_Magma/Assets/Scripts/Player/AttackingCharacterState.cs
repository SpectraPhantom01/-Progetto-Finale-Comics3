using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


// classe totalmente nuova, specifica per questo progetto
public class AttackingCharacterState : State
{
    private PlayerController m_Owner;

    private float _timeElapsed; //Timer temporaneo

    public AttackingCharacterState(PlayerController owner)
    {
        m_Owner = owner;
    }

    public override void OnEnd()
    {
        m_Owner.IsAttacking = false;
        m_Owner.CanMove = true;
    }

    public override void OnFixedUpdate()
    {
        
    }

    public override void OnStart()
    {
        //Debug.Log("Sono in attacking");
        m_Owner.IsAttacking = true;

        m_Owner.CanMove = false;
        m_Owner.Rigidbody.velocity = Vector3.zero;

        _timeElapsed = 0;

        //m_Owner.PlayerManager._trackEntry = m_Owner.PlayerManager.CurrentSkeleton.state.SetAnimation(0, "attacco", false); //TEST            
    }

    public override void OnUpdate()
    {
        _timeElapsed += Time.deltaTime;

        if (_timeElapsed >= 0.8)
        {
            m_Owner.StateMachine.SetState(EPlayerState.Idle);
        }
    }
}

