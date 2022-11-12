using System;


// stato di salto. Qui ci sono le funzioni per l'accelerazione verso l'alto. Da capire se dovrà essere implementata oppure no,
// dipende da cosa scelgono i designer se mettere un "salto" per una qualche funzionalità oppure no
public class JumpingCharacterState : State
{
    private CharacterMovement m_Owner;

    public JumpingCharacterState(CharacterMovement owner)
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
    //private float m_TimePassed = 0;
    //private float m_JumpHeigth;
    //public JumpingPlayerState(PlayerMovementManager owner)
    //{
    //    m_Owner = owner;
    //}

    //public override void MyOnCollisionEnter2D(Collision2D collision)
    //{

    //}

    //public override void OnEnd()
    //{
    //    m_TimePassed = 0;

    //}

    //public override void OnFixedUpdate()
    //{
    //    m_Owner.InputDirection = new Vector2(m_Owner.InputDirection.x, 0);
    //    m_Owner.Movement();
    //    Jump();
    //}

    //public override void OnStart()
    //{
    //    m_TimePassed = 0;
    //    m_Owner.Skeleton.loop = false;
    //    m_Owner.Skeleton.AnimationName = "SaltoApice";
    //    m_JumpHeigth = m_Owner.JumpHeight;
    //}

    //public override void OnUpdate()
    //{
    //    if (!m_Owner.IsJumping && !m_Owner.IsGrounded)
    //    {
    //        m_Owner.StateMachine.SetState(EPlayerState.Landing);
    //    }

    //    m_TimePassed += Time.deltaTime;
    //    if (m_TimePassed >= m_Owner.TimerJumpButtonIsPressed)
    //    {
    //        m_TimePassed = 0;
    //        m_Owner.IsJumping = false;
    //    }

    //    if (!m_Owner.ForwardCheckOfWall(Vector3.up, 1f))
    //    {
    //        m_Owner.StateMachine.SetState(EPlayerState.Landing);
    //    }
    //}

    //private void Jump()
    //{
    //    m_Owner.Rigidbody.velocity = new Vector2(m_Owner.Rigidbody.velocity.x, Vector2.up.y * m_JumpHeigth * Time.fixedDeltaTime);
    //    m_JumpHeigth += Time.fixedDeltaTime * m_Owner.JumpDecelerator;
    //}

}

