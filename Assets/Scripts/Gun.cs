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
	public GameObject[] bulletHoles;

	void Start () {
		lastTimeFired = Time.time - 5;	//Optional, if u want to fire instantly when the game starts
	}
		
	protected virtual void Update (){}	//These methods are overriden

	//Vector3 forward = gameObject.transform.TransformDirection(Vector3.forward);
	RaycastHit hit;

	private void processHit(GameObject hitObject){
		if (hitObject.GetComponentInParent<Zombie> () != null || hitObject.tag == "Zombie") {
			hitObject.GetComponentInParent<Zombie> ().TakeDamage (damage);
			hitObject.GetComponentInParent<Zombie> ().setShotPosition (transform);
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
					if (hit.collider.gameObject.tag == "Wall") {
						Instantiate (bulletHoles [Random.Range (0, 1)], hit.point, Quaternion.FromToRotation (Vector3.up, hit.normal));
					}
				}
			} else {
				GetComponent<AudioSource> ().PlayOneShot (emptyClip);
			}
			GetComponentInChildren<Animator> ().Play ("Fire");
		}

	}
}
