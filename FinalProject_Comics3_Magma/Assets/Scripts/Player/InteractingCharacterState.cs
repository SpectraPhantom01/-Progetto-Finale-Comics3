using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InteractingCharacterState : State
{
    private CharacterMovement m_Owner;

    public InteractingCharacterState(CharacterMovement owner)
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

