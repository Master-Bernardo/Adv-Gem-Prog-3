using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnitProto : UnitMovement {

    public float attackRate; //the same like fireRate bot for melee
    public float damage; //for now always the same damage on hit
    public float attackRange;
    private State state;

    private enum State
    {

    }

    private enum Behaviour
    {

    }

	// Use this for initialization
	void Start () {
        
	}
	
	public void Attack(UnitMovement target)
    {
        //follows the target ant attacks it if in range
    }
}
