using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
        throw new NotImplementedException();
    }

    public override void OnFixedUpdate()
    {
        throw new NotImplementedException();
    }

    public override void OnStart()
    {
        throw new NotImplementedException();
    }

    public override void OnUpdate()
    {
        throw new NotImplementedException();
    }
}

