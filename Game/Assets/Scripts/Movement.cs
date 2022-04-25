using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public Transform cam;
    public float moveSpeed = 10, jumpStrength  = 10, camHeight = 5;
    float sprintVal = 1;
    public float maxVel = 10;
    public Animator anim;
    Rigidbody2D rb2D;
    public bool grounded = true;
    Vector2 moveDir = new Vector2( 0, 0 );
    bool inStairsTrigger = false;
    GameObject currGround;
    string stairTriggerTag = "";
    bool stairInteract = false, canHide = false;

    public float hurtCoolDownStartVal = 2;
    public float hurtCoolDown = 2;
    bool hit = false, isCrouching = false;
    SpriteRenderer thisRenderer;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        rb2D = GetComponent<Rigidbody2D>();
        thisRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update(){

        rb2D.velocity = new Vector2( moveDir.x*moveSpeed*sprintVal, rb2D.velocity.y );
        float dirXAbs = Mathf.Abs(moveDir.x);
        if ( dirXAbs > 0 ){
            transform.localScale = new Vector3( moveDir.x/dirXAbs*Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z );
            if ( isCrouching ) EndCrouch();
        }
        if ( stairInteract && inStairsTrigger && grounded 
            && stairTriggerTag == "StairToFloor" ){
            Physics2D.IgnoreLayerCollision( 8, 9, true );
        } else if ( stairInteract && inStairsTrigger && grounded 
            && stairTriggerTag == "FloorToStair" ){
            Physics2D.IgnoreLayerCollision( 8, 10, true );
        }   

        anim.SetFloat( "Speed", Mathf.Abs( rb2D.velocity.magnitude ) );

        if ( hit ){
            hurtCoolDown -= Time.deltaTime;
            if ( hurtCoolDown <= 0 ){
                hurtCoolDown = hurtCoolDownStartVal;
                hit = false;
                Physics2D.IgnoreLayerCollision( 8, 11, false );
            }
        }
    }

    void FixedUpdate(){
        cam.position = new Vector3( this.transform.position.x, this.transform.position.y + camHeight, cam.position.z );
    }
    public void Move( InputAction.CallbackContext context )
    {
        moveDir = context.ReadValue<Vector2>();
    }

    public void StairInteract( InputAction.CallbackContext context ){
        switch ( context.phase ){
            case InputActionPhase.Started: stairInteract = true; break;
            case InputActionPhase.Canceled: stairInteract = false; break;

        }
    }
    
    public void Crouch( InputAction.CallbackContext context ){
        switch ( context.phase ){
            case InputActionPhase.Started: 
                if ( canHide && !isCrouching ) BeginCrouch(); 
                break;
            case InputActionPhase.Canceled:
                if ( isCrouching ) EndCrouch();
                break;
        }
    }

    void BeginCrouch(){
        anim.SetBool( "NeedCrouch", true );
        thisRenderer.sortingOrder = 2;
        Physics2D.IgnoreLayerCollision( 8, 11, true );
        isCrouching = true;
    }

    void EndCrouch(){
        anim.SetBool( "NeedCrouch", false );
        thisRenderer.sortingOrder = 4;
        Physics2D.IgnoreLayerCollision( 8, 11, false );
        isCrouching = false;
    }

    public void Jump( InputAction.CallbackContext context )
    {
        if ( !grounded || context.phase == InputActionPhase.Canceled ) return;
        rb2D.velocity = new Vector2( rb2D.velocity.x, jumpStrength );
        grounded = false;
    }

    public void Sprint( InputAction.CallbackContext context ){
        float isSprinting = context.ReadValue<float>();
        if ( context.phase == InputActionPhase.Performed ) isSprinting = 0;
        sprintVal = 2 / ( 2 - isSprinting);
    }

    private void OnCollisionEnter2D( Collision2D other ) {
        switch( other.gameObject.tag ){
            case "ground":
            case "stairs":
                currGround = other.gameObject;
                grounded = true;
                break;
            case "enemy":
                hit = true;
                Physics2D.IgnoreLayerCollision( 8, 11, true );
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        switch( other.gameObject.tag ){
            case "FloorToStair":
            case "StairToFloor":
                inStairsTrigger = true;
                stairTriggerTag = other.tag;
                break;
            case "hideLoc":
                canHide = true;
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        switch( other.gameObject.tag ){
            case "FloorToStair":
            case "StairToFloor":
                inStairsTrigger = false;
                Physics2D.IgnoreLayerCollision( 8, 9, false );
                Physics2D.IgnoreLayerCollision( 8, 10, false );
                break;
            case "hideLoc":
                canHide = false;
                break;
        }
    }
}
