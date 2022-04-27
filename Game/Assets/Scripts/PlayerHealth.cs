using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public float currHealth = 100;
    public float newHealth = 100;
    public float dmgAmount = 10;
    public Transform healthIndicator;
    float originalScaleX;
    public float speed = 1;
    public Color ghostColor;
    Color origColor;
    Renderer renderComp;
    // Start is called before the first frame update
    void Start()
    {
        originalScaleX = healthIndicator.localScale.x;
        renderComp = GetComponent<Renderer>();
        origColor = renderComp.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        currHealth = Mathf.Lerp(currHealth, newHealth, Time.deltaTime * speed);
        Vector3 currScale = healthIndicator.localScale;
        healthIndicator.localScale = new Vector3( originalScaleX*currHealth/maxHealth, currScale.y, currScale.z );
        renderComp.material.color = Color.Lerp( origColor, ghostColor, (maxHealth-currHealth)/maxHealth );

    }

    void OnCollisionEnter2D( Collision2D other ){
        if ( other.collider.tag != "enemy" ) return;
        if ( currHealth - dmgAmount >= 0)
            newHealth -= dmgAmount;
        else{
            GetComponent<SceneChanger>().GameOver();
        }
    }
}
