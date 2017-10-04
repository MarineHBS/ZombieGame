using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager Instance = null;
	private AudioSource soundEffects;

	public AudioClip zombieWalking;
	public AudioClip zombieAttacking;
	public AudioClip zombieDeath;
	public AudioClip pistolFire;
	public AudioClip ak47Fire;
	public AudioClip gatlingGunFire;
	public AudioClip emptyClip;
	public AudioClip[] footSteps;
	public AudioClip jumpSound;
	public AudioClip fallSound;




	void Start () {
		if (Instance == null) {				//Singleton code design pattern, only one copy can exist of this
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);
		}

		AudioSource[] sources = GetComponents<AudioSource> ();		//Don't change the background music, only use the other audioclip
		foreach (AudioSource source in sources) {
			if (source == null) {
				soundEffects = source;
			}
		}


	}

	public void PlayOneShot(AudioClip clip){
		soundEffects.PlayOneShot (clip);
	}
}
