using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

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
    }

    private void Start()
    {
        player1Units = new HashSet<Transform>();
        player1Units = new HashSet<Transform>();
    }


    public HashSet<Transform> GetAllPlayerUnits(int playerId)
    {
        if (playerId == 1)
            return player1Units;
        else
            return player2Units;
    }

    public void AddUnitToGame(Transform unit, int playerID)
    {
        if (playerID == 1)
            player1Units.Add(unit);
        else
            player2Units.Add(unit);
    }


}
