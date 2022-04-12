using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Transform cam;
    public float moveSpeed = 10, jumpStrength  = 10;
    public float maxVel = 10;
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
        }else if ( Input.GetKey( KeyCode.D ) ){
            rb2D.AddForce( new Vector2( moveSpeed, 0 ));
        }
        Vector2 currVel = rb2D.velocity;
        rb2D.velocity = ( currVel.magnitude > maxVel ) ? currVel.normalized * maxVel : currVel;

        if ( Input.GetKeyDown( KeyCode.Space ) && grounded ){
            Debug.Log("uhhh");
            rb2D.AddForce( new Vector2( 0, jumpStrength ) );
            grounded = false;
        }

        cam.position = new Vector3( this.transform.position.x, cam.position.y, cam.position.z );
    }

    private void OnCollisionEnter2D( Collision2D other ) {
        Debug.Log("grounded");
        grounded = true;
    }
}
