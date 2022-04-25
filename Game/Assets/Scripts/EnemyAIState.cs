using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EnemyAIState
{

    protected Transform trans;
    protected Rigidbody2D rb2D;
    protected float viewRad;
    protected Animator anim;
    protected UnityEvent<EnemyAIState> stateChanger;
    protected GameObject ground;
    protected GameObject leftStairs;
    protected GameObject rightStairs;
    public EnemyAIState( Transform t, Rigidbody2D r, Animator a, float vRad, UnityEvent<EnemyAIState> changeStateEvent, GameObject gnd, GameObject ltStr, GameObject rtStr ){
        trans = t;
        rb2D = r;
        anim = a;
        viewRad = vRad;
        stateChanger = changeStateEvent;
        ground = gnd;
        leftStairs = ltStr;
        rightStairs = rtStr;
    }
    public abstract void Update();
    public virtual void OnTriggerEnter2D(Collider2D other) { return; }
    public virtual void OnTriggerExit2D(Collider2D other) { return; }
    
    public virtual void OnCollisionEnter2D( Collision2D other ) { return; }
}

public class IdleState : EnemyAIState{
    int frameCount = 0;
    public IdleState( Transform t, Rigidbody2D r, Animator a, float vRad, UnityEvent<EnemyAIState> changeStateEvent, GameObject gnd, GameObject ltStr, GameObject rtStr ) : base( t, r, a, vRad, changeStateEvent, gnd, ltStr, rtStr ){
        frameCount = 0;
    }
    public override void Update()
    {
        anim.SetFloat( "Speed", 0 );

        int layerMask = (1 << 8);
        float invXScale = 1/trans.localScale.x;
        RaycastHit2D hit = Physics2D.Raycast( new Vector2(trans.position.x, trans.position.y + 1 ), new Vector2( 1, 0 ), viewRad*trans.localScale.x, layerMask );
        if ( hit && hit.collider.tag == "Player" ){
            ChaseState chaseState = new ChaseState( hit.transform, trans, rb2D, anim, viewRad, stateChanger, ground, leftStairs, rightStairs );
            stateChanger.Invoke( chaseState );
        }

        if ( frameCount % 15 == 0 ){
            float rand = Random.Range( 0.0f, 1.0f );
            if ( rand > .75f ){
                WanderState wanderState = new WanderState( trans, rb2D, anim, viewRad, stateChanger, ground, leftStairs, rightStairs );
                stateChanger.Invoke(wanderState);
            }
        }
        

        frameCount = (frameCount + 1) % 60;
    }
}

public class WanderState : EnemyAIState{
    
    int frameCount = 0;
    bool walking = false;
    float walkTime = 0;
    float leftWeight = 1.0f;
    float rightWeight = 1.0f;
    Vector2 dirToMove = new Vector2( 0, 0 );
    float baseMoveSpeed = 12f;
    float shiftAmt = .2f;
    Collider2D thisCollider;
    public WanderState( Transform t, Rigidbody2D r, Animator a, float vRad, UnityEvent<EnemyAIState> changeStateEvent, GameObject gnd, GameObject ltStr, GameObject rtStr ) : base( t, r, a, vRad, changeStateEvent, gnd, ltStr, rtStr ){
        frameCount = 0;
        thisCollider = trans.GetComponent<Collider2D>();
    }
    public override void Update()
    {
        int layerMask = (1 << 8);
    
        float invXScale = 1/trans.localScale.x;
        RaycastHit2D hit = Physics2D.Raycast( new Vector2(trans.position.x, trans.position.y + 1 ), new Vector2( 1, 0 ), viewRad*trans.localScale.x, layerMask );
        if ( hit && hit.collider.tag == "Player" ){
            ChaseState chaseState = new ChaseState( hit.transform, trans, rb2D, anim, viewRad, stateChanger, ground, leftStairs, rightStairs );
            stateChanger.Invoke( chaseState );
            return;
        }

        if ( !walking && frameCount % 15 == 0 ){
            float rand = Random.Range( 0.0f, 1.0f );
            if ( rand > .75f ){
                walkTime = Random.Range( 0.0f, 5.0f );
                walking = true;
                float dirToWalk = Random.Range( -1.0f * leftWeight, 1.0f * rightWeight );
                if ( dirToWalk < 0 ){
                    leftWeight -= shiftAmt;
                    rightWeight += shiftAmt;
                }else if ( dirToWalk > 0 ){
                    leftWeight += shiftAmt;
                    rightWeight -= shiftAmt;
                }
                if ( dirToWalk != 0 ){
                    dirToMove = new Vector2( dirToWalk/Mathf.Abs(dirToWalk), 0 );
                }else if ( dirToMove.x == 0 ){
                    dirToMove = new Vector2( 1, 0 );
                }
            }
            if ( rand < .25f ){
                IdleState idleState = new IdleState( trans, rb2D, anim, viewRad, stateChanger, ground, leftStairs, rightStairs );
                stateChanger.Invoke(idleState);
                return;
            }
        }
        float absDirMoveX = Mathf.Abs(dirToMove.x);
        if ( absDirMoveX > 0 ){
            anim.SetFloat( "Speed", .5f );
        }else{
            anim.SetFloat( "Speed", 0f );
        }
        float faceDir = ( absDirMoveX == 0 ) ? 1 : dirToMove.x;
        trans.localScale = new Vector3( faceDir*Mathf.Abs(trans.localScale.x), trans.localScale.y, trans.localScale.z );
        rb2D.velocity = new Vector2( baseMoveSpeed*faceDir, rb2D.velocity.y );
        walkTime -= Time.deltaTime;
        if ( walkTime <= 0 ) walking = false;
    }
    public override void OnCollisionEnter2D( Collision2D other ) {
        switch( other.gameObject.tag ){
            case "ground": break;
            case "stairs": break;
        }
    }

    public override void OnTriggerEnter2D( Collider2D other ) {
        float rand = Random.Range( 0.0f, 1.0f );
        switch( other.gameObject.tag ){
            case "StairToFloor": 
                if (rand >= .5f ){
                    foreach ( Collider2D col in leftStairs.GetComponents<Collider2D>() )
                        Physics2D.IgnoreCollision( thisCollider, col, true );
                    foreach ( Collider2D col in rightStairs.GetComponents<Collider2D>() )
                        Physics2D.IgnoreCollision( thisCollider, col, true );
                }
                break;
            case "FloorToStair": 
                if (rand >= .5f ){ 
                    foreach ( Collider2D col in ground.GetComponents<Collider2D>() )
                        Physics2D.IgnoreCollision( thisCollider, col, true );
                }
                break;
        }
    }
    
    public override void OnTriggerExit2D( Collider2D other ) {
        switch( other.gameObject.tag ){
            case "StairToFloor":
            case "FloorToStair":
                foreach ( Collider2D col in leftStairs.GetComponents<Collider2D>() )
                    Physics2D.IgnoreCollision( thisCollider, col, false );
                foreach ( Collider2D col in rightStairs.GetComponents<Collider2D>() )
                    Physics2D.IgnoreCollision( thisCollider, col, false );
                foreach ( Collider2D col in ground.GetComponents<Collider2D>() )
                    Physics2D.IgnoreCollision( thisCollider, col, false );
                break;
        }
    }
}

public class ChaseState : EnemyAIState{
    Transform target;
    float baseMoveSpeed = 12f;
    Collider2D thisCollider;
    public ChaseState( Transform tar, Transform t, Rigidbody2D r, Animator a, float vRad, UnityEvent<EnemyAIState> changeStateEvent, GameObject gnd, GameObject ltStr, GameObject rtStr ) : base( t, r, a, vRad, changeStateEvent, gnd, ltStr, rtStr ){
        target = tar;
        thisCollider = trans.GetComponent<Collider2D>();
    }
    public override void Update()
    {
        Vector2 diff = target.position - trans.position;
        Vector2 dirToMove = diff.normalized;
        float faceDir = ( diff.x >= 0 ) ? 1 : -1;
        trans.localScale = new Vector3( faceDir*Mathf.Abs(trans.localScale.x), trans.localScale.y, trans.localScale.z );
        
        anim.SetFloat( "Speed", .5f );
        rb2D.velocity = new Vector2( baseMoveSpeed*faceDir, rb2D.velocity.y );
        
        if ( diff.magnitude > Mathf.Abs( viewRad*trans.localScale.x ) ){
            IdleState idleState = new IdleState( trans, rb2D, anim, viewRad, stateChanger, ground, leftStairs, rightStairs );
            stateChanger.Invoke(idleState);
       }
    }

    public override void OnCollisionEnter2D( Collision2D other ) {
        switch( other.gameObject.tag ){
            case "ground": break;
            case "stairs": break;
        }
    }

    public override void OnTriggerEnter2D( Collider2D other ) {
        float rand = Random.Range( 0.0f, 1.0f );
        switch( other.gameObject.tag ){
            case "StairToFloor": 
                if (rand >= .5f ){
                    foreach ( Collider2D col in leftStairs.GetComponents<Collider2D>() )
                        Physics2D.IgnoreCollision( thisCollider, col, true );
                    foreach ( Collider2D col in rightStairs.GetComponents<Collider2D>() )
                        Physics2D.IgnoreCollision( thisCollider, col, true );
                }
                break;
            case "FloorToStair": 
                if (rand >= .5f ){ 
                    foreach ( Collider2D col in ground.GetComponents<Collider2D>() )
                        Physics2D.IgnoreCollision( thisCollider, col, true );
                }
                break;
        }
    }

    public override void OnTriggerExit2D(Collider2D other)
    {
        switch( other.gameObject.tag ){
            case "StairToFloor":
            case "FloorToStair":
                foreach ( Collider2D col in leftStairs.GetComponents<Collider2D>() )
                    Physics2D.IgnoreCollision( thisCollider, col, false );
                foreach ( Collider2D col in rightStairs.GetComponents<Collider2D>() )
                    Physics2D.IgnoreCollision( thisCollider, col, false );
                foreach ( Collider2D col in ground.GetComponents<Collider2D>() )
                    Physics2D.IgnoreCollision( thisCollider, col, false );
                break;
        }
    }
}

public class RecklessState : EnemyAIState{
    public RecklessState( Transform t, Rigidbody2D r, Animator a, float vRad, UnityEvent<EnemyAIState> changeStateEvent, GameObject gnd, GameObject ltStr, GameObject rtStr ) : base( t, r, a, vRad, changeStateEvent, gnd, ltStr, rtStr ){
        
    }
    public override void Update() { }
}
