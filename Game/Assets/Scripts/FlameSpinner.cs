using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameSpinner : MonoBehaviour
{
    public Transform flame1, flame2, flame3;
    public float xdiff = 2 * Mathf.PI * .3333333f;
    public float ydiff = .01f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float currTime = Time.time;

        Vector3 f1NextPos = new Vector3(  Mathf.Cos(currTime)*.16f, .04f*Mathf.Sin(currTime), 0 );
        if ( flame1.localPosition.x - f1NextPos.x <= 0) flame1.GetComponent<Renderer>().sortingOrder = 3;
        else flame1.GetComponent<Renderer>().sortingOrder = 4;
        flame1.localPosition = f1NextPos;

        Vector3 f2NextPos = new Vector3( Mathf.Cos(currTime + xdiff)*.16f, .04f*Mathf.Sin(currTime + ydiff), 0 );
        if ( flame2.localPosition.x - f2NextPos.x <= 0) flame2.GetComponent<Renderer>().sortingOrder = 3;
        else flame2.GetComponent<Renderer>().sortingOrder = 4;
        flame2.localPosition = f2NextPos;

        Vector3 f3NextPos = new Vector3( Mathf.Cos(currTime + xdiff*2)*.16f, .04f*Mathf.Sin(currTime + ydiff*2), 0 );
        if ( flame3.localPosition.x - f3NextPos.x <= 0) flame3.GetComponent<Renderer>().sortingOrder = 3;
        else flame3.GetComponent<Renderer>().sortingOrder = 4;
        flame3.localPosition = f3NextPos;
    }
}
