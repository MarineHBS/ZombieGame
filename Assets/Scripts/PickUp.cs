using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {

	public string pickUpName;

	void Start () {
		
	}

	void Update () {
		
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.GetComponent<Player> () != null && collider.gameObject.tag == "Player") {
			collider.gameObject.GetComponent<Player> ().PickUp (pickUpName);
			Destroy (gameObject);
		}
	}
}
