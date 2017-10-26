using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

	[SerializeField]
	private Text healthText;
	[SerializeField]
	private Text ammoText;
	[SerializeField]
	private Text keyText;

	public Player player;

	void Start () {
		SetHealthText (player.health);
		//SetAmmoText (player.ammo.GetAmmo(tag));
	}

	public void SetHealthText(int health){
		healthText.text = "" + health;
	}

	public void SetAmmoText(int ammo){
		ammoText.text = ammo + "";
	}
	public void SetKeyText(int amountOfKeys){
		keyText.text = amountOfKeys + "";
	}

	void Update () {
		
	}
}
