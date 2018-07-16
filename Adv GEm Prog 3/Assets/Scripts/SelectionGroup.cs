using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionGroup {
    /*
     * Holds our selected Units, and a formation, formation can bes changed and positioned, units will be send to the formation, 
     * there is always one default selection group for the current selection, but groups can also be saved and assigned to shortcuts later
     * 
     */

    private List<UnitMovement> selectionGroup;
    [SerializeField]
    private Formation formation;


    #region boring setter getter stuff
    
        //Constructor
    public SelectionGroup(Formation formation)
    {
        selectionGroup = new List<UnitMovement>();
        this.formation = formation;
    }

    public void Add(UnitMovement unitMovement) //like the HashSetMethod
    {
        selectionGroup.Add(unitMovement);
    }

    public void Remove(UnitMovement unitMovement)
    {
        selectionGroup.Remove(unitMovement);
    }

    public List<UnitMovement> GetSet()
    {
        return selectionGroup;
    }

    public void DeselectAll()
    {
        foreach (UnitMovement uMov in selectionGroup)
        {
            uMov.Deselect();
        }
        selectionGroup.Clear();
    }

    public void SelectAll()
    {
        foreach (UnitMovement uMov in selectionGroup)
        {
            uMov.Select();
        }
    }

    public bool Contains(UnitMovement unitMovement)
    {
        if (selectionGroup.Contains(unitMovement)) return true;
        else return false;
    }

    public int Count()
    {
        return selectionGroup.Count;
    }
    #endregion

    private void UpdateFormation(Vector3 formationPosition,Vector3 formationLookDirection) //creates the formation 
    {
        //formation = new Formation(selectionGroup.Count);
        formation.CreateFormation(selectionGroup.Count, formationPosition, formationLookDirection);
        //formation.transform.position = formationPosition;
        //formation.transform.rotation = Quaternion.LookRotation(formationLookDirection);
        
        //formation.DrawFormation();
    }

    //sends troops to positions determined by formation object, selectedUnits MUST be the same count as the count of the formation positions
    // the local formation positions are translatet to world coordinates here? 
    public void SendTroopsToFormation(Vector3 formationPosition, Vector3 formationLookDirection, bool run)
    {
        UpdateFormation(formationPosition, formationLookDirection);

        List<FormationPosition> positionsArrayList = formation.GetTheWorldPositions();

        for (int i = 0; i < positionsArrayList.Count; i++)
        {
            selectionGroup[i].run = run;
            selectionGroup[i].SetDestination(positionsArrayList[i].position, positionsArrayList[i].lookDirection);
        }
    }

    public Vector3 GetMiddleInWorldCoordinates() // returns the middle point of all the units ofthe group
    {
        float xValue = 0;
        float zValue = 0;

        foreach(UnitMovement uMov in selectionGroup)
        {
            xValue += uMov.transform.position.x;
            zValue += uMov.transform.position.z;
        }

        Vector3 middle = new Vector3(xValue / selectionGroup.Count, 0f, zValue / selectionGroup.Count);
        //Debug.Log(middle);
        return middle;
    }
}
