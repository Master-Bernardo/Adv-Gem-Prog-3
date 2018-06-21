using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour {

    public Camera cam;
    public NavMeshAgent agent;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))  //left mouse button
        {
            //Input.mousePosition // current mouse position in screen coordinates
            //we need to convert from screen to world space, our camera has this function
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);  // this is our mouse position ray
            RaycastHit hit;
           // Debug.Log("hit");
            // now we shoot the ray
            if (Physics.Raycast(ray, out hit)) //returns true if we hit something
            {
                //move our agent
                agent.SetDestination(hit.point);
            }
        }
	}
}
