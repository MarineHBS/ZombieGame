using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
	//Scenes
	public const string GameplayScene = "Gameplay";

	//Weapons
	public const string Pistol = "Pistol";
	public const string Ak47 = "Ak47";
	public const string GatlingGun = "GatlingGun";


	//Opponents
	public const string NormalZombie = "NormalZombie";
	public const string Person = "Person";

	//Pickups
	public const int PickUpAmmo = 1;
	public const int PickUpKey = 2;

	//Misc
	public const string Game = "Game";
	public const float CameraDefaultZoom = 60f;

	public static readonly int[] AllPickupTypes = new int[2] {
		PickUpAmmo,
		PickUpKey
	};


}
