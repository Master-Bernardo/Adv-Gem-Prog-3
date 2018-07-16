using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {


    public Camera cam;
    //private HashSet<UnitMovement> selectedUnits;
    private SelectionGroup selectionGroup;
    private int playerID = 1;
   

    //for rect select and rmb hold rotate
    private Vector3 lmbPosition1;
    private Vector3 rmbPosition1;
    private float timeLmbDown = 0f;
    private float timeRmbDown = 0f;

    bool doubleRmbClick = false;

    [SerializeField]
    private Formation formation;

    private void Awake()
    {
        selectionGroup = new SelectionGroup(formation);
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
            //for double rmb  
            if (Time.time - timeRmbDown < 0.25)
            {
                //Debug.Log("doubleClick");
                HandleRmbUp(true);
                doubleRmbClick = true;
            }
            else doubleRmbClick = false;
            timeRmbDown = Time.time;
            rmbPosition1 = Input.mousePosition;
            
        }

        if (Input.GetMouseButtonUp(0))  
        {
            HandleLmbUp(); 
        }

        if (Input.GetMouseButtonUp(1) && Time.time <= timeRmbDown + 0.15)
        {
            if (!doubleRmbClick)
            {
                HandleRmbUp(false);
                //Debug.Log("single");
            }
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
                if (selectionGroup.Count() > 0)
                {
                    selectionGroup.DeselectAll();
                }
                selectionGroup.Add(hittedUnit);
            }
            else
            {
                if (!selectionGroup.Contains(hittedUnit))
                {
                    selectionGroup.Add(hittedUnit);
                }
                else
                {
                    selectionGroup.Remove(hittedUnit);
                    hittedUnit.Deselect();
                }

            }


            selectionGroup.SelectAll();

        }
        else
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                if (selectionGroup.Count() > 0)
                {
                    selectionGroup.DeselectAll();
                }
            }


        }
    }

    //attacks enemy units or sets destination if clicked on gorund or allied unit
    private void HandleRmbUp(bool doubleClick)  //double click makes the units run
    {
        //Debug.Log("short select");
        //lmbDown = false;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);  // this is our mouse position ray
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "Unit")
        {
            //if i hit my own unit, go there
            if (hit.collider.gameObject.GetComponent<UnitMovement>().getPlayerID() == 1)
            {
                //Debug.Log("hittetMyUnit");
                if (selectionGroup.Count() != 0)
                {
                    bool run;
                    if (doubleClick) run = true;
                    else run = false;
                    selectionGroup.SendTroopsToFormation(hit.point + Vector3.up * 0.1f, new Vector3(hit.point.x - selectionGroup.GetMiddleInWorldCoordinates().x, 0f, hit.point.z - selectionGroup.GetMiddleInWorldCoordinates().z), run);
                }
            }
            else if (hit.collider.gameObject.GetComponent<UnitMovement>().getPlayerID() == 2)
            {
                //TODO Add an attack in Formation?
                //Debug.Log("hittetEnemyUnit");
                if (selectionGroup.Count() != 0)
                {
                    foreach (UnitMovement uMov in selectionGroup.GetSet())
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
            if (selectionGroup.Count() != 0)
            {
                bool run;
                if (doubleClick) run = true;
                else run = false;
                selectionGroup.SendTroopsToFormation(hit.point + Vector3.up*0.1f, new Vector3(hit.point.x - selectionGroup.GetMiddleInWorldCoordinates().x,0f, hit.point.z - selectionGroup.GetMiddleInWorldCoordinates().z), run);
            }
        }
    }


    //selects all units (enemies and allies) in the selection rectangle which we can draw while holding lmb down 
    private void  HandleLongLmbUp()
    {
        Vector3 lmbPosition2 = Input.mousePosition;

        foreach (Transform transform in GameManager.Instance.GetAllUnits())
        {
            if (transform != null) { //somehow this is sometimes null idkw
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
                    selectionGroup.Add(unit);
                }
                else
                {
                    transform.gameObject.GetComponent<UnitMovement>().Deselect();
                    selectionGroup.Remove(unit);
                }
            }
        }
    }

    //we can draw a line, units will go to the first point , and when arrived will turn to the second point
    private void HandleLongRmbUp()
    {
        //sende die einheit dahin und rotiere sie in die richtung in die wir ziehen
        Vector3 rmbPosition1WorldSpace = Vector3.zero;
        Ray ray = cam.ScreenPointToRay(rmbPosition1);  // this is our mouse position ray
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            rmbPosition1WorldSpace = hit.point;


            //Debug.Log("long -> positioning");
            Vector3 rmbPosition2 = Input.mousePosition;

            Vector3 direction = new Vector3(rmbPosition2.x - rmbPosition1.x, 0f, rmbPosition2.y - rmbPosition1.y);

            if (selectionGroup.Count() != 0)
            {
                selectionGroup.SendTroopsToFormation(hit.point + Vector3.up * 0.1f, direction, false);
            }
        }
    }

    public void DeleteUnit(UnitMovement unit)
    {
        if (selectionGroup.Contains(unit)) selectionGroup.Remove(unit);
    }


}
