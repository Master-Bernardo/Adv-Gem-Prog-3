using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public float movementSpeed = 1f;
	// Update is called once per frame
	void Update () {
        float moveHorizontally = Input.GetAxis("Horizontal") * movementSpeed;
        float moveVertically = Input.GetAxis("Vertical") * movementSpeed;

        transform.position += new Vector3(moveHorizontally, 0f, moveVertically);
    }
}
