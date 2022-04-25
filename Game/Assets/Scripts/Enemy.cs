using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb2D;
    EnemyAIState currState;
    public float viewRad = 3;
    public GameObject ground;
    public GameObject leftStair;
    public GameObject rightStair;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        UnityEvent<EnemyAIState> changeStateEvent = new UnityEvent<EnemyAIState>();
        changeStateEvent.AddListener( ChangeEnemyState );
        currState = new IdleState( this.transform, rb2D, anim, viewRad, changeStateEvent, ground, leftStair, rightStair );
    }

    // Update is called once per frame
    void Update()
    {
        currState.Update();
    }

    public void ChangeEnemyState( EnemyAIState newState ){
        currState = newState;
    }

    private void OnCollisionEnter2D( Collision2D other ) {
        currState.OnCollisionEnter2D( other );
    }
    
    private void OnTriggerEnter2D( Collider2D other ) {
        currState.OnTriggerEnter2D( other );
    }
    private void OnTriggerExit2D( Collider2D other ) {
        currState.OnTriggerExit2D( other );
    }
}
