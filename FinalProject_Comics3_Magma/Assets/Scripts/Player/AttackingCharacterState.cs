using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// classe totalmente nuova, specifica per questo progetto
public class AttackingCharacterState : State
{
    private CharacterMovement m_Owner;

    public AttackingCharacterState(CharacterMovement owner)
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

