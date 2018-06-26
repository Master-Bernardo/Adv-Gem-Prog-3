using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAnimation : MonoBehaviour {

    public float speed = 0.2f;
    public float strength = 9f;


    void Update () {
            Vector3 pos = transform.position;
            pos.x = Mathf.Sin(Time.time * speed ) * strength;
            transform.position = pos;
    }
}
