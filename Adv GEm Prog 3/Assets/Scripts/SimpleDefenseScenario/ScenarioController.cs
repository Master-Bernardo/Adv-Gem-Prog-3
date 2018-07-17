using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioController : MonoBehaviour {

    public Spawner enemySpawner1;
    public Spawner enemySpawner2;
    public Spawner friendlySpawner;
    public Transform DefensePoint;

    public float spawnIntervall = 60f;
    private float nextSpawnTime;

	// Use this for initialization
	void Awake () {
        nextSpawnTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time > nextSpawnTime)
        {

            //List<UnitFighter> spawnedArcherUnit = enemySpawner2.Spawn(Spawner.UnitType.Archer, 10);
            List<UnitFighter> spawnedSwordFighterUnit = enemySpawner1.Spawn(Spawner.UnitType.Swordman, 12);

            /*foreach (UnitFighter archer in spawnedArcherUnit)
            {
                archer.SetDestination(DefensePoint.position);
            }*/
            foreach (UnitFighter swordsman in spawnedSwordFighterUnit)
            {
                swordsman.SetDestination(DefensePoint.position);
            }



            nextSpawnTime = Time.time + spawnIntervall;
        }
	}
}
