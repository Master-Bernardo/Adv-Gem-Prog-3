using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//will be changed later to a better one form asset store
public class CameraMovement : MonoBehaviour {

    public float movementSpeed = 1f;
    public float scrollSpeed = 2f;
    public float minY = 20f;
    public float maxY = 120f;
    
	// Update is called once per frame
	void Update () {

        float moveHorizontally = Input.GetAxis("Horizontal") * movementSpeed;
        float moveVertically = Input.GetAxis("Vertical") * movementSpeed;

        float scroll = Input.GetAxis("Mouse ScrollWheel");


        transform.position += new Vector3(moveHorizontally * Time.deltaTime, - scroll * 100f * scrollSpeed * Time.deltaTime, moveVertically * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y,minY,maxY), transform.position.z);  //clamp

    }
}
