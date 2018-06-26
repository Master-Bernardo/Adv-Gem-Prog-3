using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {


    public Camera cam;
    private HashSet<UnitMovement> selectedUnits;
    private int playerID = 1;
    //public Transform[] allUnitsTransform;
   

    //for rect select
    //private bool lmbDown = false;
    private Vector3 lmbPosition1;
    private Vector3 rmbPosition1;
    private float timeLmbDown;
    private float timeRmbDown;

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
            lmbPosition1 = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))  
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);  // this is our mouse position ray
            RaycastHit hit;

            //if we hit unit
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "Unit") 
            {
                UnitMovement hittedUnit = hit.collider.gameObject.GetComponent<UnitMovement>();
                if (!Input.GetKey(KeyCode.LeftShift)) { 
                    if (selectedUnits.Count > 0 )
                    {
                        foreach (UnitMovement uMov in selectedUnits)
                        {
                            uMov.Deselect();
                        }
                    selectedUnits.Clear();
                     }
                    selectedUnits.Add(hittedUnit);
                }
                else
                {
                    if (!selectedUnits.Contains(hittedUnit))
                    {
                        selectedUnits.Add(hittedUnit);
                    }else
                    {
                        selectedUnits.Remove(hittedUnit);
                        hittedUnit.Deselect();

                    }
                    
                }
                
                
                foreach(UnitMovement uMov in selectedUnits)
                {
                    uMov.Select();
                }
             
            }
            else
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    if (selectedUnits.Count > 0)
                    {
                        foreach (UnitMovement uMov in selectedUnits)
                        {
                            uMov.Deselect();
                        }
                        selectedUnits.Clear();
                    }
                }

                
            }
        }
        if (Input.GetMouseButtonUp(1) && Time.time <= timeRmbDown + 0.15)
        {
            Debug.Log("short select");
            //lmbDown = false;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);  // this is our mouse position ray
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "Unit") //returns true if we hit something
            {
                //if i hit my own unit, go there
                if (GameManager.Instance.IsThisMyUnit(playerID, hit.collider.gameObject.transform)){ 
                    if (selectedUnits.Count != 0)
                    {
                        foreach (UnitMovement uMov in selectedUnits)
                        {
                            if (uMov.playerID == playerID) { 
                                uMov.SetDestination(hit.point);
                            }
                        }
                    }
                }else   //if I hit enemy Unit
                {
                    if (selectedUnits.Count != 0)
                    {
                        foreach (UnitMovement uMov in selectedUnits)
                        {
                            if (uMov.playerID == playerID)
                            {
                                uMov.Attack(hit.collider.gameObject.GetComponent<UnitMovement>());
                            }
                        }
                    }
                }

            }else if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "Environment")
            {
                if (selectedUnits.Count != 0)
                {
                    foreach (UnitMovement uMov in selectedUnits)
                    {
                        if (uMov.playerID == playerID)
                        {
                            uMov.SetDestination(hit.point);
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            timeRmbDown = Time.time;
            rmbPosition1 = Input.mousePosition;
        }





        //when we release the mouse key after holding it
        if (Input.GetMouseButtonUp(0) && Time.time > timeLmbDown + 0.1 )  // wird nur aufgerufen wenn länger als 0.2 sekunden gehalten wird
        {
            Vector3 lmbPosition2 = Input.mousePosition;

            foreach(Transform transform in GameManager.Instance.GetAllUnits())
            {
                Vector3 unitScreenPosition = cam.WorldToScreenPoint(transform.position);
                bool isInRectangle = false;
                //wann drinn
                if (lmbPosition1.x > lmbPosition2.x && lmbPosition1.y>lmbPosition2.y)
                {
                    if (unitScreenPosition.x < lmbPosition1.x && unitScreenPosition.y < lmbPosition1.y && unitScreenPosition.x > lmbPosition2.x && unitScreenPosition.y > lmbPosition2.y) isInRectangle = true;
                }
                else if (lmbPosition1.x > lmbPosition2.x && lmbPosition1.y < lmbPosition2.y)
                {
                    if (unitScreenPosition.x < lmbPosition1.x && unitScreenPosition.y > lmbPosition1.y && unitScreenPosition.x > lmbPosition2.x && unitScreenPosition.y <  lmbPosition2.y) isInRectangle = true;
                }
                else if (lmbPosition1.x < lmbPosition2.x && lmbPosition1.y < lmbPosition2.y)
                {
                    if (unitScreenPosition.x > lmbPosition1.x && unitScreenPosition.y > lmbPosition1.y && unitScreenPosition.x < lmbPosition2.x && unitScreenPosition.y < lmbPosition2.y) isInRectangle = true;
                }
                else //(position1.x < position2.x && position1.y > position2.y)
                {
                    if (unitScreenPosition.x > lmbPosition1.x && unitScreenPosition.y < lmbPosition1.y && unitScreenPosition.x < lmbPosition2.x && unitScreenPosition.y > lmbPosition2.y) isInRectangle = true;
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

        if (Input.GetMouseButtonUp(1) && Time.time > timeRmbDown + 0.15)
        {
            Debug.Log("long select");
            Vector3 rmbPosition2 = Input.mousePosition;

            Vector3 direction = new Vector3(rmbPosition2.x - rmbPosition1.x,0f, rmbPosition2.y - rmbPosition1.y) ;
            foreach (UnitMovement uMov in selectedUnits)
            {
                if (uMov.playerID == playerID)
                {
                    uMov.FaceDirection(direction);
                }
            }
            Debug.Log(direction);

        }
    }//Update End
}
