using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

	public float rateOfFire;
	protected float lastTimeFired;
	public AudioClip fire;
	public AudioClip emptyClip;
	public Ammo ammo;
	public int damage;

	void Start () {
		lastTimeFired = Time.time - 5;	//Optional, if u want to fire instantly when the game starts
	}
		
	protected virtual void Update (){}	//These methods are overriden

	private void processHit(GameObject hitObject){
		if (hitObject.GetComponent<Zombie> () != null || hitObject.tag == "Zombie") {
			hitObject.GetComponent<Zombie> ().TakeDamage (damage);
		}
	}


	protected void Fire(){
		if (Time.timeScale == 1) {				//If returning from pause menu, don't shoot once, only after clicking on resume
			if (ammo.HasAmmo (tag)) {
				GetComponent<AudioSource> ().PlayOneShot (fire);
				ammo.ConsumeAmmo (tag);
				Ray ray = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));		//Middle of the screen (x = 0.5, y = 0.5, z = 0);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) {				//Infinite range, 3rd parameter
					processHit (hit.collider.gameObject);
				}
			} else {
				GetComponent<AudioSource> ().PlayOneShot (emptyClip);
			}
			GetComponentInChildren<Animator> ().Play ("Fire");
		}

	}
}
