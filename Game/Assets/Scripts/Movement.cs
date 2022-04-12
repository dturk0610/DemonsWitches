using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Transform cam;
    public float moveSpeed = 10;
    public float maxVel = 10;
    Rigidbody2D rigidbody2D;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetKey(KeyCode.A) ){
            rigidbody2D.AddForce( new Vector2( -moveSpeed, 0 ));
        }else if ( Input.GetKey(KeyCode.D) ){
            rigidbody2D.AddForce( new Vector2( moveSpeed, 0 ));
        }
        Vector2 currVel = rigidbody2D.velocity;
        rigidbody2D.velocity = ( currVel.magnitude > maxVel ) ? currVel.normalized * maxVel : currVel;
        cam.position = new Vector3( this.transform.position.x, cam.position.y, cam.position.z );
    }
}
