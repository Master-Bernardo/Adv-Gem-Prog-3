using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensePoint : MonoBehaviour {

    /*
     * Wenn ein Feind diesen Trigger erreicvht, haben wir verloren
     */
    bool lost = false;

    private void OnTriggerEnter(Collider other)
    {
        if(!lost) { 
        if (other.gameObject.tag == "Unit")
        {
            if (other.gameObject.GetComponent<UnitMovement>().playerID == 2)
            {
                lost = true;
                Debug.Log("Game Over,  Time Survived: " + Time.time);
            }
        }
    }
    }
}
