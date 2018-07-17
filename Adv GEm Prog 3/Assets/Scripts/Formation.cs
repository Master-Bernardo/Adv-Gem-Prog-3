using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FormationPosition
{
    public Vector3 position;
    public Vector3 lookDirection;

    public FormationPosition(Vector3 position, Vector3 lookDirection)
    {
        this.position = position;
        this.lookDirection = lookDirection;
    }
}

public class Formation: MonoBehaviour{

	/*
     *  Holds a Listo r Set? of Positions where the troops are supposed to go
     */

    private List<FormationPosition> positions; // the positions ale alwys relative to the middle od the formation
    private List<GameObject> triangles;

    public GameObject PlaceholderUnitPrefab; // just for testing

    public enum FormationType
    {
        Line,
        Square,
        Keil,
        //usw
    }

    /*public void Start() //just for debug
    {
        createFormation(36);
        DrawFormation();
    }*/

    /*public Formation(int unitNumber)
    {
        createFormation(unitNumber);
        DrawFormation();
    }*/

    //Constructor for now a default one  - just a block like in TW or a A4 paper
    //public Formation(int unitNumber)
    public void CreateFormation(int unitNumber, Vector3 formationPositionInWorldSpace, Vector3 formationLookDirection)
    {
        transform.position = formationPositionInWorldSpace;
        transform.rotation = Quaternion.LookRotation(formationLookDirection);

        positions = new List<FormationPosition>();
        triangles = new List<GameObject>();

        //fill our list: 

        float spacing = 2.5f;
        //wir füllen ein Rechteck mit positions
        int widthInUnits = Mathf.RoundToInt(Mathf.Sqrt(unitNumber)*1.2f  +0.5f) ; //Rechteck längere Ecke - Länge in Anzahl der Units gegeben - aufgerunded
        int lengthInUnits = Mathf.RoundToInt(Mathf.Sqrt(unitNumber) * 0.8f  +0.5f); //länge der Tiefe - Länge in Anzahl der Units gegeben - aufgerunded

        float width = (widthInUnits-1) * spacing;
        float length = -((lengthInUnits-1) * spacing) ;

        float currentWidth = 0;
        float currentLength = 0;

        bool lastLine = false; //for the lastLineCheck - so the last line gets verschoben only once

        for (int i = 0; i < unitNumber; i++) //-we start from the top left corner as 0
        {
            //Debug.Log("current length " + currentLength);
            //Debug.Log("length " + length);

            //when we are at the last line: they should be in center not on the left
            if (currentLength <= length &&!lastLine) //TODO this check is wrong
            {
                lastLine = true;
                int unitsLeft = unitNumber - i;
                int wievieleEinheitenFehlenZumKomplettenLinie = widthInUnits - unitsLeft;
                currentWidth += (wievieleEinheitenFehlenZumKomplettenLinie / 2f) * spacing;
            }

            positions.Add(new FormationPosition(new Vector3(currentWidth, 0f, currentLength), new Vector3(0f,0f,1f)));

            currentWidth += spacing;

            if (currentWidth > width)  //here we change the line
            {
                currentLength -= spacing;
                currentWidth = 0f;
            }  
        }

        //nun alles verschieben, sodass das zentrum (0/0/0) nicht oben links sondern in derMitte sit, das wollen wir erstmal
        foreach (FormationPosition formationPosition in positions)
        {
            formationPosition.position -= new Vector3(width / 2f, 0f, length / 2f);
        }

        InstantiateTriangles();
        StartCoroutine("DestroyTriangles");
    }

    public void InstantiateTriangles()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        triangles.Clear();


        foreach (FormationPosition formationWorldPosition in GetTheWorldPositions())
        {
            //GameObject triangle = Instantiate(PlaceholderUnitPrefab, transform.position + formationPosition.position, Quaternion.LookRotation(formationPosition.lookDirection));
            GameObject triangle = Instantiate(PlaceholderUnitPrefab, formationWorldPosition.position, Quaternion.LookRotation(formationWorldPosition.lookDirection));
            triangle.transform.SetParent(transform);
            triangles.Add(triangle);
        }
    }

    private IEnumerator DestroyTriangles()
    {
        yield return new WaitForSeconds(2f);
        foreach( GameObject triangle in triangles)
        {
            Destroy(triangle);
        }
    }
  

    public Formation(FormationType formationType, float width, float spacing, Vector3 position, int UnitNumber)
    {
        //formations are crated all the time when we hold rmb and change the spacing with mmb or the position with mouse movement, the Type with UI or the width like in TW
    }

    public List<FormationPosition> GetTheWorldPositions()
    {
        List<FormationPosition> worldPositions = new List<FormationPosition>();
        foreach (FormationPosition formationPosition in positions)
        {
            worldPositions.Add(new FormationPosition(transform.TransformPoint(formationPosition.position), transform.TransformDirection(formationPosition.lookDirection)));
        }
            
        return worldPositions;
    }
    
}
