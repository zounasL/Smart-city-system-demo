using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class pathTracing : MonoBehaviour
{
    public float speed = 1.19f;
    Vector3 pointA;
    Vector3 pointB;

    // Start is called before the first frame update
    void Start()
    {
        pointA = new Vector3(0, 0, 0);
        pointB = new Vector3(9, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //PingPong between 0 and 1
        float time = Mathf.PingPong(Time.time * speed, 1);
        transform.position = Vector3.Lerp(pointA, pointB, time);
    }
}
