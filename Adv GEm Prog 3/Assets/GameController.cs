using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {


    public Camera cam;
    private HashSet<UnitMovement> selectedUnits;
    public Transform[] allUnitsTransform;
   

    //for rect select
    private bool lmbDown = false;
    private Vector3 position1;
    private float timeLmbDown;

    private void Start()
    {
        selectedUnits = new HashSet<UnitMovement>();
    }
    // Update is called once per frame
    void Update()
    {
        //left mouse button
        if (Input.GetMouseButtonDown(0))  
        {
            timeLmbDown = Time.time;
            position1 = Input.mousePosition;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);  // this is our mouse position ray
            RaycastHit hit;

            //if we hit unit
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "PlayerUnit") 
            {
                if (!Input.GetKey(KeyCode.LeftShift)) { 
                    if (selectedUnits.Count > 0 )
                    {
                        foreach (UnitMovement uMov in selectedUnits)
                        {
                            uMov.Deselect();
                        }
                    selectedUnits.Clear();
                     }
                }

                selectedUnits.Add(hit.collider.gameObject.GetComponent<UnitMovement>());
                foreach(UnitMovement uMov in selectedUnits)
                {
                    uMov.Select();
                }
             
            }
            else
            {
                if(selectedUnits.Count > 0)
                {
                    foreach (UnitMovement uMov in selectedUnits)
                    {
                        uMov.Deselect();
                    }
                    selectedUnits.Clear();
                }

                
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            lmbDown = false;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);  // this is our mouse position ray
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "Environment" || hit.collider.gameObject.tag == "PlayerUnit") //returns true if we hit something
            {
                if (selectedUnits.Count != 0)
                {
                    foreach (UnitMovement uMov in selectedUnits)
                    {
                        uMov.SetDestination(hit.point);
                    }
                }

            }
        }else
        {
            lmbDown = false;
        }


        
       //when we release the mouse key after holding it
        if (Input.GetMouseButtonUp(0) && Time.time > timeLmbDown + 0.2 )  // wird nur aufgerufen wenn länger als 0.2 sekunden gehalten wird
        {
            Vector3 position2 = Input.mousePosition;
            Debug.Log(position2);
            foreach(Transform transform in allUnitsTransform)
            {
                Vector3 unitScreenPosition = cam.WorldToScreenPoint(transform.position);
                bool isInRectangle = false;
                //wann drinn
                if (position1.x > position2.x && position1.y>position2.y)
                {
                    if (unitScreenPosition.x < position1.x && unitScreenPosition.y < position1.y && unitScreenPosition.x > position2.x && unitScreenPosition.y > position2.y) isInRectangle = true;
                }
                else if (position1.x > position2.x && position1.y < position2.y)
                {
                    if (unitScreenPosition.x < position1.x && unitScreenPosition.y > position1.y && unitScreenPosition.x > position2.x && unitScreenPosition.y <  position2.y) isInRectangle = true;
                }
                else if (position1.x < position2.x && position1.y < position2.y)
                {
                    if (unitScreenPosition.x > position1.x && unitScreenPosition.y > position1.y && unitScreenPosition.x < position2.x && unitScreenPosition.y < position2.y) isInRectangle = true;
                }
                else //(position1.x < position2.x && position1.y > position2.y)
                {
                    if (unitScreenPosition.x > position1.x && unitScreenPosition.y < position1.y && unitScreenPosition.x < position2.x && unitScreenPosition.y > position2.y) isInRectangle = true;
                }

                UnitMovement unit = transform.gameObject.GetComponent<UnitMovement>();
                if (isInRectangle) {
                    unit.Select();
                    selectedUnits.Add(unit);
                }
                else { 
                    transform.gameObject.GetComponent<UnitMovement>().Deselect();
                    selectedUnits.Remove(unit);
                }
            }
        }
    }
}
