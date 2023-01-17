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

    public AttackingCharacterState(PlayerController owner)
    {
        m_Owner = owner;
    }

    public override void OnEnd()
    {
        
    }

    public override void OnFixedUpdate()
    {
        
    }

    public override void OnStart()
    {
        Debug.Log("Sono in attacking");
    }

    public override void OnUpdate()
    {
        //Introdurre cooldown per attacco?

        //_damager.Attack();
    }
}

