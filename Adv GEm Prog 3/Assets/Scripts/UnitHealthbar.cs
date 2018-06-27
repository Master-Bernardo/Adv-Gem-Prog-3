using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealthbar : MonoBehaviour {


	//takes a float 0-1 as input which represents the percent of current health
	public void UpdateHealthBar(float percent) {
        transform.GetChild(0).transform.localScale = new Vector3(percent, transform.localScale.y, transform.localScale.z);
        transform.rotation = GameManager.Instance.GetCameraRotation()  * Quaternion.Euler(90,0,0);

    }
}
