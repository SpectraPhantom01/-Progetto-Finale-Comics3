using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


// stato idle
public class IdleCharacterState : State
{
    private PlayerController m_Owner;

    public IdleCharacterState(PlayerController owner)
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
        Debug.Log("Sono in idle");
    }

    public override void OnUpdate()
    {
        //Cambio di stato? ==> Gestito alla fine dal GameManager (tecnicamente)
    }
}

