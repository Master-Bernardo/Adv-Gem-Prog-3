using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    public Camera mainCam;

    //unit sets - for now only player 1- we and player2 - enemy AI
    private HashSet<Transform> player1Units;
    private HashSet<Transform> player2Units;

    
    public void Awake() 
    {
        //Singleton Code
        if (Instance != null)
        {
            DestroyImmediate(Instance); 
        }
        else
        {
            Instance = this;
        }

        player1Units = new HashSet<Transform>();
        player2Units = new HashSet<Transform>();
    }

    //returns the HashSet of all Units of a player
    public HashSet<Transform> GetAllPlayerUnits(int playerId)
    {
        if (playerId == 1)
            return player1Units;
        else
            return player2Units;
    }

    //returns all Units InGame
    public HashSet<Transform> GetAllUnits()
    {
        HashSet<Transform> allUnits = new HashSet<Transform>();
        allUnits = player1Units;
        allUnits.UnionWith(player2Units);
        return allUnits;
    }

    //adds Units to the Sets, this Mehod is Called by all the units when they are Created
    public void AddUnitToGame(Transform unit, int playerID)
    {
        if (playerID == 1)
            player1Units.Add(unit);
        else
            player2Units.Add(unit);
    }

    //returns the currentRotation of the Camera, is used by The HP bars to rotate to this Camera
    public Quaternion GetCameraRotation()
    {
        return mainCam.transform.localRotation;
    }

    //check if this Unit belongs to a certain Player
    public bool IsThisMyUnit(int playerID,Transform unit)
    {
        if (playerID == 1)
        {
            if (player1Units.Contains(unit)) return true;
            else return false;
        }else
        {
            if (player2Units.Contains(unit)) return true;
            else return false;
        }
    }


}
