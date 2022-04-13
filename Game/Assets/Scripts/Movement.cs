using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Transform cam;
    public float moveSpeed = 10, jumpStrength  = 10;
    public float maxVel = 10;
    public Animator anim;
    Rigidbody2D rb2D;
    bool grounded = true;
    // Start is called before the first frame update
    void Start()
    {
        rb2D = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetKey( KeyCode.A ) ){
            rb2D.AddForce( new Vector2( -moveSpeed, 0 ));
            transform.localScale = new Vector3( -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z );
        }else if ( Input.GetKey( KeyCode.D ) ){
            rb2D.AddForce( new Vector2( moveSpeed, 0 ));
            transform.localScale = new Vector3( Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z );
        }
        Vector2 currVel = rb2D.velocity;
        rb2D.velocity = ( currVel.magnitude > maxVel ) ? Vector2.Lerp( currVel.normalized * maxVel, currVel, Time.deltaTime ) : currVel;

        anim.SetFloat( "Speed", Mathf.Abs( rb2D.velocity.magnitude ) );

        if ( Input.GetKeyDown( KeyCode.Space ) && grounded ){
            rb2D.AddForce( new Vector2( 0, jumpStrength ) );
            grounded = false;
        }

    }

    void FixedUpdate(){
        cam.position = new Vector3( this.transform.position.x, cam.position.y, cam.position.z );
    }

    private void OnCollisionEnter2D( Collision2D other ) {
        grounded = true;
    }
}
