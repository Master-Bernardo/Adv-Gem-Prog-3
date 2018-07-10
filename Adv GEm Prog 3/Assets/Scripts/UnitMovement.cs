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
    public bool moving = false;
    [Space(10)]

    //for manuall turning via holding rmb - wil be changed
    protected bool manualTurning = false;
    protected Quaternion wishRotation;

    //for running
    public float runSpeed;
    public float normalSpeed;
    public bool run = false; //if true we run, so our speed is 2 times bigger

    //for animation
    public Animator animator;

    private void Start()
    {
        GameManager.Instance.AddUnitToGame(transform, playerID);
        currentHealth = maxHealth;
        //agent.updateRotation = true;
    }

    protected virtual void Update()
    {
        if (agent.velocity.magnitude > 1)
        {
            moving = true;
            manualTurning = false;
            agent.updateRotation = true;
            animator.SetBool("isRunning", true);
        }
        else
        {
            moving = false;
            animator.SetBool("isRunning", false);
        }

        if (selected)
        {
            healthbar.UpdateHealthBar(currentHealth / maxHealth);
        }

        if (manualTurning)  //turns in the desired direction depending on angularSpeed
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, wishRotation, agent.angularSpeed*Time.deltaTime/30); ///10
            //Debug.Log(wishRotation.eulerAngles);
            if (transform.rotation == wishRotation)
            {
                //Debug.Log("transform.rotation == wishRotation");
                manualTurning = false;
                agent.updateRotation = true;
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
        if (run) agent.speed = runSpeed;
        else agent.speed = normalSpeed;

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
        else
        {
            currentHealth = 0;
            Die();
        }
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
        //Debug.Log("superclass Attack");
    }

    public virtual void HandleAttack(DamageType damageType, int damageAmount)
    {
        GetDamage(damageType, damageAmount);
    }

    public int getPlayerID()
    {
        return playerID;
    }

    protected void Die()
    {
        //for now without fancy animation and "leichenspawn"
       
        GameManager.Instance.RemoveUnitFromGame(transform, playerID);
        Destroy(gameObject);

    }
    
}
