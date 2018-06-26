using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour {

    public NavMeshAgent agent;
    public GameObject selectedMarker;
    public UnitHealthbar healthbar;

    private float currentHealth;
    public int maxHealth = 50;

    public int playerID = 1;
    private bool selected = false;

    private void Start()
    {
        GameManager.Instance.AddUnitToGame(transform, playerID);
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (selected)
        {
            healthbar.UpdateHealthBar(currentHealth / maxHealth);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            currentHealth -= 10;
        }
    }

    public void Select()
    {
        selected = true;
        selectedMarker.SetActive(true);
        healthbar.gameObject.SetActive(true);
    }
    public void Deselect()
    {
        selected = false;
        selectedMarker.SetActive(false);
        healthbar.gameObject.SetActive(false);
    }
    public void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public void GetDamage(DamageType damageType, int damageAmount)
    {
        currentHealth -= damageAmount;
    }

    //we get a collection of waypoints the last one in the collection is the end goal
    public void SetWaypoints(Vector3[] waypoints)
    {
        int index =0;
        agent.SetDestination(waypoints[index]);
        //agent.SetPath?
        //vllcht lieber coroutine
        while (index != waypoints.Length) {
            //agent.remainingDistance <0.5f
            //agent.pathPending??
            //wenn desitnation == current position oder agent.setDestination erreichtw, 
            //dann index ++
            //und agent.setDestination(waypoints[index])
        }
    }
}
