using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

	public int health;
	public GameController gameController;
	public GameUI UI;
	public Animator playerAnimator;
	public Animator startDoorAnimator;
	public Animator secretWallAnimator;
	public Animator trapDoorAnimator;
	public Ammo ammo;
	public bool isDead;
	public AudioClip playerDeath;
	public AudioClip pickUpPickedUp;
	public AudioClip trapDoorOpening;
	public AudioClip pickUpKey;
	private WeaponHolder weaponHolder;
	private int amountOfKeys;


	void Start ()
	{
		amountOfKeys = 0;
		ammo = GetComponent<Ammo> ();
		isDead = false;
		weaponHolder = GetComponent<WeaponHolder> ();
	}

	public void TakeDamage (int damage)
	{
		health -= damage;
		UI.SetHealthText (health);

		if (health <= 0) {
			GetComponent<AudioSource> ().PlayOneShot (playerDeath);
			isDead = true;
			StartCoroutine ("PlayerDie");
		}
	}

	IEnumerator PlayerDie ()
	{
		yield return new WaitForSeconds (0.0f);
		gameController.GameOver ();
	}

	public bool playerIsDead ()
	{
		return isDead;
	}

	private void pickUpHealth ()
	{
		if (health + 30 > 150) {
			health = 150;
		} else {
			health += 30;
		}

	}

	private void pickUpPistolAmmo ()
	{
		ammo.AddAmmo (Constants.Pistol, 10);
	}

	private void pickUpAk47Ammo ()
	{
		ammo.AddAmmo (Constants.Ak47, 30);
	}

	private void pickUpGatlingGunAmmo ()
	{
		ammo.AddAmmo (Constants.GatlingGun, 50);
	}

	public void PickUp (string pickUpName)
	{
		switch (pickUpName) {
		case "PistolAmmo":
			GetComponent<AudioSource> ().PlayOneShot (pickUpPickedUp);
			pickUpPistolAmmo ();
			if (weaponHolder.GetActiveWeaponConstraint () == Constants.Pistol) {
				UI.SetAmmoText (ammo.GetAmmo (Constants.Pistol));
			}
			break;
		case "Ak47Ammo":
			GetComponent<AudioSource> ().PlayOneShot (pickUpPickedUp);
			pickUpAk47Ammo ();
			if (weaponHolder.GetActiveWeaponConstraint () == Constants.Ak47) {
				UI.SetAmmoText (ammo.GetAmmo (Constants.Ak47));
			}
			break;
		case "GatlingGunAmmo":
			GetComponent<AudioSource> ().PlayOneShot (pickUpPickedUp);
			pickUpGatlingGunAmmo ();
			if (weaponHolder.GetActiveWeaponConstraint () == Constants.GatlingGun) {
				UI.SetAmmoText (ammo.GetAmmo (Constants.GatlingGun));
			}
			break;
		case "Key":
			GetComponent<AudioSource> ().PlayOneShot (pickUpKey);
			amountOfKeys++;
			UI.SetKeyText (amountOfKeys);
			break;
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.tag == "Door") {
			startDoorAnimator.Play ("DoorOpening");
		}
		if (other.gameObject.tag == "SecretWall") {
			secretWallAnimator.Play ("SecretDoorAnimation");
		}
		if (other.gameObject.tag == "TrapDoor" && amountOfKeys == 3) {
			trapDoorAnimator.Play ("TrapDoorAnimation");
			GetComponent<AudioSource> ().PlayOneShot (trapDoorOpening);
		}
		if (other.gameObject.tag == "TreasureChest") {
			gameController.Win ();
		}
	}

}
