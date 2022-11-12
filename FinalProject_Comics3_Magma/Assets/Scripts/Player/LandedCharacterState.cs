using System;


// stato di Atterrato. Viene chiamata dallo stato di caduta (falling) e indica il momento in cui il character atterra dopo una caduta (o salto che sia)
public class LandedCharacterState : State
{
    private CharacterMovement m_Owner;

    public LandedCharacterState(CharacterMovement owner)
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

    // OLD CODE FROM FINAL PROJECT 2

    //private PlayerMovementManager m_Owner;
    //private float m_TimePassed;

    //public LandedPlayerState(PlayerMovementManager owner)
    //{
    //    m_Owner = owner;
    //}
    //public override void MyOnCollisionEnter2D(Collision2D collision)
    //{
    //}

    //public override void OnEnd()
    //{
    //}

    //public override void OnFixedUpdate()
    //{
    //}

    //public override void OnStart()
    //{
    //    m_Owner.Rigidbody.velocity = Vector2.zero;
    //    m_TimePassed = 0;
    //    m_Owner.Skeleton.loop = false;
    //    m_Owner.Skeleton.AnimationName = "SaltoTermineDue";
    //}

    //public override void OnUpdate()
    //{
    //    if (m_Owner.InputDirection.magnitude > 0)
    //    {
    //        m_Owner.StateMachine.SetState(EPlayerState.Walking);
    //        return;
    //    }
    //    if (m_Owner.IsJumping)
    //    {
    //        m_Owner.StateMachine.SetState(EPlayerState.Jumping);
    //        return;
    //    }
    //    if (!m_Owner.IsGrounded)
    //    {
    //        m_Owner.StateMachine.SetState(EPlayerState.Landing);
    //    }
    //    m_TimePassed += Time.deltaTime;
    //    if (m_TimePassed >= 1f)
    //    {
    //        m_Owner.StateMachine.SetState(EPlayerState.Walking);
    //    }
    //}
}

