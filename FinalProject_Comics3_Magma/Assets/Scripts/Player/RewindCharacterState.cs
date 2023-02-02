using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindCharacterState : State
{
    private PlayerController m_Owner;

    //private float _timeElapsed;

    public RewindCharacterState(PlayerController owner)
    {
        m_Owner = owner;
    }

    public override void OnEnd()
    {
        //Manca qualcosa da gestire alla fine?
    }

    public override void OnFixedUpdate()
    {
        
    }

    public override void OnStart()
    {
        if (!m_Owner.GhostActive)
        {
            m_Owner.CreateGhost();
        }
        else
        {
            m_Owner.Rewind();
        }          
    }

    public override void OnUpdate()
    {
       //Aggiungere tempo?
    }

}
