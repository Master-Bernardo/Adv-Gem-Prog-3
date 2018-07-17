using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {


    public GameObject archerPrefab;
    public GameObject crossbowmanPrefab;
    public GameObject spearmanPrefab;
    public GameObject swordmanPrefab;

    public enum UnitType
    {
        Archer,
        Crossbowman,
        Spearman,
        Swordman
    }

    //returns the list of the wpawned Objects
    public List<UnitFighter> Spawn(UnitType unitType, int number)
    {
        GameObject prefabToInstantiate = null;
        List<UnitFighter> instantiatedUnits = new List<UnitFighter>();

        switch (unitType)
        {
            case UnitType.Archer:
                prefabToInstantiate = archerPrefab;
                break;
            case UnitType.Crossbowman:
                prefabToInstantiate = crossbowmanPrefab;
                break;
            case UnitType.Spearman:
                prefabToInstantiate = spearmanPrefab;
                break;
            case UnitType.Swordman:
                prefabToInstantiate = swordmanPrefab;
                break;
            default:
                break;
        }


        for (int i = 0; i < number; i++)
        {
            instantiatedUnits.Add(Instantiate(prefabToInstantiate, transform.position, transform.rotation).GetComponent<UnitFighter>());
        }

        return instantiatedUnits;
    }
}
