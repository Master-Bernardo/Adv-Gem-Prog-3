using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {


    public Camera cam;
    private HashSet<UnitMovement> selectedUnits;
    private int playerID = 1;
   

    //for rect select and rmb hold rotate
    private Vector3 lmbPosition1;
    private Vector3 rmbPosition1;
    private float timeLmbDown;
    private float timeRmbDown;


    private void Awake()
    {
        selectedUnits = new HashSet<UnitMovement>();
    }
  
    void Update()
    {
        //left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            timeLmbDown = Time.time;
            lmbPosition1 = Input.mousePosition;
        }
        if (Input.GetMouseButtonDown(1))
        {
            timeRmbDown = Time.time;
            rmbPosition1 = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))  
        {
            HandleLmbUp(); 
        }

        if (Input.GetMouseButtonUp(1) && Time.time <= timeRmbDown + 0.15)
        {
            HandleRmbUp();
        }

        //when we release the mouse key after holding it
        if (Input.GetMouseButtonUp(0) && Time.time > timeLmbDown + 0.1 )  // wird nur aufgerufen wenn länger als 0.1 sekunden gehalten wird
        {
            HandleLongLmbUp();
        }

        if (Input.GetMouseButtonUp(1) && Time.time > timeRmbDown + 0.15)
        {
            HandleLongRmbUp();
        }

    }//Update End

    //selects Units (Enemies or allies) , selects or deselects multiple with shift key
    private void HandleLmbUp()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);  // this is our mouse position ray
        RaycastHit hit;

        //if we hit unit
        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "Unit")
        {
            UnitMovement hittedUnit = hit.collider.gameObject.GetComponent<UnitMovement>();
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
                selectedUnits.Add(hittedUnit);
            }
            else
            {
                if (!selectedUnits.Contains(hittedUnit))
                {
                    selectedUnits.Add(hittedUnit);
                }
                else
                {
                    selectedUnits.Remove(hittedUnit);
                    hittedUnit.Deselect();

                }

            }


            foreach (UnitMovement uMov in selectedUnits)
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

    //attacks enemy units or sets destination if clicked on gorund or allied unit
    private void HandleRmbUp()
    {
        Debug.Log("short select");
        //lmbDown = false;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);  // this is our mouse position ray
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "Unit")
        {
            //if i hit my own unit, go there
            if (hit.collider.gameObject.GetComponent<UnitMovement>().getPlayerID() == 1)
            {
                Debug.Log("hittetMyUnit");
                if (selectedUnits.Count != 0)
                {
                    foreach (UnitMovement uMov in selectedUnits)
                    {
                        if (uMov.playerID == playerID)
                        {
                            uMov.SetDestination(hit.point);
                            Debug.Log("hittetMyUnitSetDestination");
                        }
                    }
                }
            }
            else if (hit.collider.gameObject.GetComponent<UnitMovement>().getPlayerID() == 2)
            {
                Debug.Log("hittetEnemyUnit");
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
        }
        else if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "Environment")
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

    //selects all units (enemies and allies) in the selection rectangle which we can draw while holding lmb down 
    private void  HandleLongLmbUp()
    {
        Vector3 lmbPosition2 = Input.mousePosition;

        foreach (Transform transform in GameManager.Instance.GetAllUnits())
        {
            Vector3 unitScreenPosition = cam.WorldToScreenPoint(transform.position);
            bool isInRectangle = false;
            //wann drinn
            if (lmbPosition1.x > lmbPosition2.x && lmbPosition1.y > lmbPosition2.y)
            {
                if (unitScreenPosition.x < lmbPosition1.x && unitScreenPosition.y < lmbPosition1.y && unitScreenPosition.x > lmbPosition2.x && unitScreenPosition.y > lmbPosition2.y) isInRectangle = true;
            }
            else if (lmbPosition1.x > lmbPosition2.x && lmbPosition1.y < lmbPosition2.y)
            {
                if (unitScreenPosition.x < lmbPosition1.x && unitScreenPosition.y > lmbPosition1.y && unitScreenPosition.x > lmbPosition2.x && unitScreenPosition.y < lmbPosition2.y) isInRectangle = true;
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
            if (isInRectangle)
            {
                unit.Select();
                selectedUnits.Add(unit);
            }
            else
            {
                transform.gameObject.GetComponent<UnitMovement>().Deselect();
                selectedUnits.Remove(unit);
            }
        }
    }

    //we can draw a line, if our selected unit is not moving, it will face in the direction of that line, will be made better in the future - TW style positioning
    private void HandleLongRmbUp()
    {
        Debug.Log("long -> positioning");
        Vector3 rmbPosition2 = Input.mousePosition;

        Vector3 direction = new Vector3(rmbPosition2.x - rmbPosition1.x, 0f, rmbPosition2.y - rmbPosition1.y);
        foreach (UnitMovement uMov in selectedUnits)
        {
            if (uMov.playerID == playerID)
            {
                uMov.FaceDirection(direction);
            }
        }
        Debug.Log(direction);
    }
}
