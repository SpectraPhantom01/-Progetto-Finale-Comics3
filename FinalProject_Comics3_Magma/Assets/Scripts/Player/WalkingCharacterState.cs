using UnityEngine;

// stato di walk. prende la direzione dall'owner (cioè dall'input)
public class WalkingCharacterState : State
{
    private PlayerController m_Owner;

    public WalkingCharacterState(PlayerController owner)
    {
        m_Owner = owner;
    }

    public override void OnStart()
    {
        //Debug.Log("Sono in walking");
    }

    public override void OnUpdate()
    {
        if (m_Owner.Direction.magnitude == 0)
        {
            m_Owner.StateMachine.SetState(EPlayerState.Idle);
        }
    }

    public override void OnFixedUpdate()
    {
        
    }

    public override void OnEnd()
    {
        //Debug.Log(m_Owner.StateMachine.CurrentState);
    }


    //Da spostare qui maybe?
    //private void MoveDirection()
    //{
    //    if (Direction.magnitude < 0.01f)
    //    {
    //        value = Mathf.MoveTowards(value, 0, deceleration * Time.fixedDeltaTime);

    //        rb.velocity = lastDirection * value;

    //        _stateMachine.SetState(PlayerStates.Idle);
    //    }
    //    else
    //    {
    //        //lastDirection = normalizedDirection;

    //        value += acceleration * Time.fixedDeltaTime;
    //        value = Mathf.Clamp(value, -maxSpeed, maxSpeed);

    //        rb.velocity = Direction.normalized * value;
    //    }

    //    AttackPointRotation();
    //}










    // OLD CODE FROM FINAL PROJECT 2

    //public override void MyOnCollisionEnter2D(Collision2D collision)
    //{

    //}

    //public override void OnEnd()
    //{
    //    m_TimePassed = 0;
    //}

    //public override void OnFixedUpdate()
    //{
    //    if (Mathf.Abs(m_Owner.InputDirection.magnitude) > 0)
    //    {
    //        m_Owner.InputDirection = new Vector2(m_Owner.InputDirection.x, 0);
    //        m_Owner.Movement();
    //    }
    //    if (m_OnDeceleration)
    //    {
    //        if (!m_Owner.DeadCoroutinePlaying)
    //            Decelerate();
    //    }
    //}

    //private void Decelerate()
    //{
    //    if (m_Owner.Rigidbody.velocity.x > 0)
    //    {
    //        m_Owner.Rigidbody.velocity -= new Vector2(m_Owner.InertiaDecelerator * Time.fixedDeltaTime, 0f);
    //        if (m_Owner.Rigidbody.velocity.x < 0)
    //        {
    //            m_Owner.Rigidbody.velocity = Vector3.zero;
    //            m_OnDeceleration = false;
    //        }
    //    }
    //    else if (m_Owner.Rigidbody.velocity.x < 0)
    //    {
    //        m_Owner.Rigidbody.velocity += new Vector2(m_Owner.InertiaDecelerator * Time.fixedDeltaTime, 0f);
    //        if (m_Owner.Rigidbody.velocity.x > 0)
    //        {
    //            m_Owner.Rigidbody.velocity = Vector3.zero;
    //            m_OnDeceleration = false;
    //        }
    //    }
    //}

    //public override void OnStart()
    //{
    //    m_TimePassed = 0;
    //    m_Owner.Skeleton.loop = true;
    //}



    //public override void OnUpdate()
    //{
    //    m_Owner.InputDirection = new Vector2(m_Owner.InputDirection.x, 0);



    //    if (m_Owner.IsJumping)
    //    {
    //        m_Owner.StateMachine.SetState(EPlayerState.Jumping);
    //        return;
    //    }
    //    if (!m_Owner.IsGrounded)
    //    {
    //        m_Owner.StateMachine.SetState(EPlayerState.Landing);
    //        return;
    //    }
    //    if (m_Owner.InputDirection.magnitude == 0)
    //    {
    //        m_TimePassed += Time.deltaTime;
    //        if (m_TimePassed >= m_Owner.InertiaTime)
    //        {
    //            m_OnDeceleration = true;
    //            m_TimePassed = 0;
    //        }
    //    }

    //    if (Mathf.Abs(m_Owner.Rigidbody.velocity.x) > 0)
    //    {
    //        m_Owner.Skeleton.AnimationName = "Camminata";
    //    }
    //    else
    //    {
    //        m_Owner.Skeleton.AnimationName = "Idol";
    //    }
    //}


}
