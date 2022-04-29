using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScaleHandler : MonoBehaviour
{
    public Vector2[] possibleLocations = { new Vector2( 0, 0 ) };
    public float waitTime = 100;
    float originalTime;
    Color originalColor;
    // Start is called before the first frame update
    void Start()
    {
        int spawnLoc = Random.Range( 0, possibleLocations.Length );
        transform.localPosition = new Vector3( possibleLocations[spawnLoc].x, possibleLocations[spawnLoc].y, 0 );
        originalTime = waitTime;
        originalColor = GetComponent<Renderer>().material.color;
        Physics2D.IgnoreLayerCollision( 13, 11, true );
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Renderer>().material.color = new Color( originalColor.r, originalColor.g, originalColor.b, ( originalTime - waitTime )/originalTime );
        float deltaTime = Time.deltaTime;
        if ( waitTime - deltaTime >= 0 ) waitTime -= deltaTime;
        if ( waitTime < 10 ) GetComponent<Rigidbody2D>().simulated = true;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if ( waitTime > 10 ) return;
        if ( other.collider.tag != "Player" ) return;

        SceneManager.LoadScene("wonGame");

    }
}
