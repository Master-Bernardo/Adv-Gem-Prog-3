using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour {

    [Header("Basic UnitMovement")]
    public NavMeshAgent agent;
    public GameObject selectedMarker;
    public UnitHealthbar healthbar;

    private float currentHealth;
    public int maxHealth = 50;

    public int playerID = 1;
    private bool selected = false;
    private bool moving = false;
    [Space(10)]

    //for manuall turning via holding rmb - wil be changed
    protected bool manualTurning = false;
    protected Quaternion wishRotation;

    private void Start()
    {
        GameManager.Instance.AddUnitToGame(transform, playerID);
        currentHealth = maxHealth;
        //agent.updateRotation = true;
    }

    protected virtual void Update()
    {
        if (agent.velocity.magnitude > 1) moving = true;
        else moving = false;

        if (selected)
        {
            healthbar.UpdateHealthBar(currentHealth / maxHealth);
        }

        if (Input.GetKeyDown(KeyCode.K)) // for debugging
        {
            currentHealth -= 10;
        }

        if (moving)
        {
            manualTurning = false;
            agent.updateRotation = true;
        }
        
        if (manualTurning)  //turns in the desired direction depending on angularSpeed
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, wishRotation, agent.angularSpeed*Time.deltaTime/30); ///10
            if (transform.rotation == wishRotation)
            {
                manualTurning = false;
                agent.updateRotation = true;
                //agent.updateRotation = true;
            }
        }
        
    }

    public void Select()
    {
        selected = true;
        selectedMarker.SetActive(true);
        healthbar.UpdateHealthBar(currentHealth / maxHealth); //do this once before, so it does not flashes on enabling
        healthbar.gameObject.SetActive(true);
    }
    public void Deselect()
    {
        selected = false;
        selectedMarker.SetActive(false);
        healthbar.gameObject.SetActive(false);
    }
    public virtual void SetDestination(Vector3 destination)
    {
        TurnToDestination(destination);
        agent.SetDestination(destination);
    }

    protected virtual void TurnToDestination(Vector3 destination)
    {
        manualTurning = true;
        agent.updateRotation = false;
        Vector3 ourPosition = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 destinationPosition = new Vector3(destination.x, 0f, destination.z);
        wishRotation = Quaternion.LookRotation(destinationPosition - ourPosition);
    }

    public void GetDamage(DamageType damageType, int damageAmount)
    {
        if (currentHealth > 0) currentHealth -= damageAmount;
        else currentHealth = 0;
        Debug.Log("Damage Taken");
    }

    public void FaceDirection(Vector3 direction)
    {
        if (!moving) { 
            direction.y = 0;
            wishRotation = Quaternion.LookRotation(direction);
            manualTurning = true;
        }
    }

    //we get a collection of waypoints the last one in the collection is the end goal - in progress
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
    public virtual void Attack(UnitMovement target)
    {
        Debug.Log("superclass Attack");
    }

    public int getPlayerID()
    {
        return playerID;
    }
    
}
