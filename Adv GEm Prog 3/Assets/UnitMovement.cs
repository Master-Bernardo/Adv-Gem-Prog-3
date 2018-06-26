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
    private bool moving = false;

    //for manuall returning via holding rmb
    private bool manualTurning = false;
    Quaternion wishRotation;

    private void Start()
    {
        GameManager.Instance.AddUnitToGame(transform, playerID);
        currentHealth = maxHealth;
        agent.updateRotation = true;
    }

    protected virtual void Update()
    {
        if (agent.velocity.magnitude > 1) moving = true;
        else moving = false;
        if (selected)
        {
            healthbar.UpdateHealthBar(currentHealth / maxHealth);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            currentHealth -= 10;
        }
        
        if (manualTurning)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, wishRotation, agent.angularSpeed*Time.deltaTime/30);
            if (transform.rotation == wishRotation)
            {
                manualTurning = false;
                //agent.updateRotation = true;
            }
        }
        
    }

    public void Select()
    {
        selected = true;
        selectedMarker.SetActive(true);
        healthbar.UpdateHealthBar(currentHealth / maxHealth);
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
        manualTurning = true;
        wishRotation = Quaternion.LookRotation(destination-transform.position);
        agent.SetDestination(destination);
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
        //agent.updateRotation = false;  // so the agent does not snap back to last position prior moving

        /*
        direction.y = 0;
        direction.Normalize();
        SetDestination(transform.position + direction*10f);
        agent.updatePosition = false;
        */

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
    public virtual void Attack(UnitMovement target)
    {
        Debug.Log("superclass Attack");
    }
    
}
