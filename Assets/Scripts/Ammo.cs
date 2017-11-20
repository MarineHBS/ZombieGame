using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour {

	public GameUI UI;

	[SerializeField]
	private int pistolAmmo = 10;

	[SerializeField]
	private int ak47Ammo = 30;

	[SerializeField]
	private int gatlingGunAmmo = 100;

	public Dictionary<string, int> tagToAmmo;

	void Awake(){
		tagToAmmo = new Dictionary<string, int> {
			{ Constants.Pistol, pistolAmmo },
			{ Constants.Ak47, ak47Ammo },
			{ Constants.GatlingGun, gatlingGunAmmo }
		};
	}

	public void AddAmmo(string tag, int ammo){
		if (!tagToAmmo.ContainsKey (tag)) {
			Debug.LogError ("No such gun" + tag);
		}
		tagToAmmo [tag] += ammo;
	}

	public bool HasAmmo(string tag){
		if (!tagToAmmo.ContainsKey (tag)) {
			Debug.LogError ("No such gun" + tag);
		}
		return tagToAmmo [tag] > 0;
	}

	public int GetAmmo(string tag){
		if (!tagToAmmo.ContainsKey (tag)) {
			Debug.LogError ("No such gun" + tag);
		}
		return tagToAmmo [tag];
	}

	public void ConsumeAmmo(string tag){
		if (!tagToAmmo.ContainsKey (tag)) {
			Debug.LogError ("No such gun" + tag);
		}

		tagToAmmo [tag] -= 1;
		UI.SetAmmoText (tagToAmmo [tag]);	//Refreshing the ammo UI text

	}

}
