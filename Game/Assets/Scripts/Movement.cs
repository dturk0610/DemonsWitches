using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public Transform cam;
    public float moveSpeed = 10, jumpStrength  = 10;
    float sprintVal = 1;
    public float maxVel = 10;
    public Animator anim;
    Rigidbody2D rb2D;
    public bool grounded = true;
    Vector2 moveDir = new Vector2( 0, 0 );
    // Start is called before the first frame update
    void Start()
    {
        rb2D = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        rb2D.velocity = new Vector2( moveDir.x*moveSpeed*sprintVal, rb2D.velocity.y );
        float dirXAbs = Mathf.Abs(moveDir.x);
        if ( dirXAbs > 0 )
            transform.localScale = new Vector3( moveDir.x/dirXAbs*Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z );

        anim.SetFloat( "Speed", Mathf.Abs( rb2D.velocity.magnitude ) );
    }

    void FixedUpdate(){
        cam.position = new Vector3( this.transform.position.x, cam.position.y, cam.position.z );
    }
    public void Move(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }
    
    public void Jump(InputAction.CallbackContext context)
    {
        if ( !grounded ) return;
        rb2D.velocity = new Vector2( rb2D.velocity.x, jumpStrength );
        grounded = false;
    }

    public void Sprint(InputAction.CallbackContext context){
        float isSprinting = context.ReadValue<float>();
        if ( context.phase == InputActionPhase.Performed) isSprinting = 0;
        sprintVal = 2 / ( 2 - isSprinting);
    }

    private void OnCollisionEnter2D( Collision2D other ) {
        if ( other.gameObject.tag == "ground"){
            grounded = true;
        }
    }
}
