using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    public Camera mainCam;
    private HashSet<Transform> player1Units;
    private HashSet<Transform> player2Units;

    public void Awake() // wir setzen sicher dass es immer existier aber immer nur eins
    {
        if (Instance != null)
        {
            DestroyImmediate(Instance); // es kann passieren wenn wir eine neue Scene laden dass immer noch eine Instanz existiert
        }
        else
        {
            Instance = this;
        }

        player1Units = new HashSet<Transform>();
        player2Units = new HashSet<Transform>();
    }


    public HashSet<Transform> GetAllPlayerUnits(int playerId)
    {
        if (playerId == 1)
            return player1Units;
        else
            return player2Units;
    }

    public HashSet<Transform> GetAllUnits()
    {
        HashSet<Transform> allUnits = new HashSet<Transform>();
        allUnits = player1Units;
        allUnits.UnionWith(player2Units);
        return allUnits;
    }

    public void AddUnitToGame(Transform unit, int playerID)
    {
        if (playerID == 1)
            player1Units.Add(unit);
            //Debug.Log(unit);
        else
            player2Units.Add(unit);
            //Debug.Log(unit);
    }

    public Quaternion GetCameraRotation()
    {
        return mainCam.transform.localRotation;
    }
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
