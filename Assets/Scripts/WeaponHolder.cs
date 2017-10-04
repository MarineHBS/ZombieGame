using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour {

	public static string chosenWeaponConstraint;

	public GameObject pistol;
	public GameObject ak47;
	public GameObject gatlingGun;
	public GameUI UI;

	GameObject chosenWeapon;
	[SerializeField]
	Ammo ammo;


	void Start () {
		chosenWeaponConstraint = Constants.Pistol;
		chosenWeapon = pistol;
	}
	private void loadWeapon(GameObject weapon){
		pistol.SetActive (false);
		ak47.SetActive (false);
		gatlingGun.SetActive (false);
		weapon.SetActive (true);
		chosenWeapon = weapon;

		UI.SetAmmoText (ammo.GetAmmo (chosenWeapon.tag));	//Get the chosen weapons tag, then change ammo text according to the weapon type
	}

	public GameObject GetActiveWeapon(){
		return chosenWeapon;
	}

	public string GetActiveWeaponConstraint(){
		return chosenWeaponConstraint;
	}


	void Update () {
		if (Input.GetKeyDown ("1")) {
			loadWeapon (pistol);
			chosenWeaponConstraint = Constants.Pistol;
		}else if (Input.GetKeyDown ("2")) {
			loadWeapon (ak47);
			chosenWeaponConstraint = Constants.Ak47;
		}else if (Input.GetKeyDown ("3")) {
			loadWeapon (gatlingGun);
			chosenWeaponConstraint = Constants.GatlingGun;
		}
	}
}
