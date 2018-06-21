using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour {

    public NavMeshAgent agent;
    public GameObject selectedMarker;

    private bool selected = false;


    public void Select()
    {
        selected = true;
        selectedMarker.SetActive(true);
    }
    public void Deselect()
    {
        selected = false;
        selectedMarker.SetActive(false);
    }
    public void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    //we get a collection of waypoints the last one in the collection is the end goal
    public void SetWaypoints()
    {

    }
}
