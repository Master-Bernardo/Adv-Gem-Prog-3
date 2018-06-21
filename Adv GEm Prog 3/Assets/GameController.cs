using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {


    public Camera cam;
    private UnitMovement selectedUnit;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))  //left mouse button
        {

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);  // this is our mouse position ray
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "Unit") //returns true if we hit something
            {
                if (selectedUnit != null)
                {
                    selectedUnit.Deselect();
                }
                
                selectedUnit = hit.collider.gameObject.GetComponent<UnitMovement>();
                selectedUnit.Select();
                // agent.SetDestination(hit.point);
            }
            else
            {
                selectedUnit.Deselect();
                selectedUnit = null;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);  // this is our mouse position ray
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "Environment") //returns true if we hit something
            {
                if (selectedUnit != null)
                {
                    selectedUnit.SetDestination(hit.point);
                }

            }
        }
    }
}
