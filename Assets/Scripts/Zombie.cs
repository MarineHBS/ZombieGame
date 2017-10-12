using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Zombie : MonoBehaviour {

	public int health;
	public int attackRange;
	public float hitRate;
	public float detectionDistance;
	public float zombieWalkingSpeed;
	public Animator bodyAnimator;
	public AudioClip zombieWalking;
	public AudioClip zombieAttacking;
	public int damage;
	public AudioClip zombieDeath;
	private SoundManager manager;
	public ParticleSystem zombieBlood;
	public Canvas healthBarCanvas;
	public float alertDistance;
	public float fov;
	public LayerMask zombieLayer;
	public int eyeSight;

	UnityEngine.AI.NavMeshAgent agent;

	private Image healthBar;
	private float nextAttack = 0;
	private Transform player;
	private int maxHealth;
	private bool isDead;
	private bool isChasingPlayer;
	private bool isAttacking;
	private bool isAttacked;
	private bool isFollowingZombie;

	void Start () {
		fov = 50;
		isDead = false;
		isChasingPlayer = false;
		isAttacking = false;
		isAttacked = false;
		isFollowingZombie = false;
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		maxHealth = health;
		healthBar = transform.Find ("EnemyCanvas").Find("HealthBar").Find("Health").GetComponent<Image> ();
		GetComponent<AudioSource> ().clip = zombieWalking;
		alertDistance = 8;
	}

	void setZombieAttributes(float actualDistance){
		//if (actualDistance <= detectionDistance && actualDistance > attackRange) {
		if (actualDistance > attackRange) {
			isChasingPlayer = true;
			isAttacking = false;

		} else if (actualDistance <= attackRange) {
			isChasingPlayer = false;
			isAttacking = true;

		} /*else {
			isChasing = false;
			isAttacking = false;
		}*/
	}

	void Alerted(){
		Collider[] zombies = Physics.OverlapSphere (transform.position, alertDistance, zombieLayer);
		//StartCoroutine ("Alerted");
		for(int i = 0; i < zombies.Length; ++i){
			Zombie z = zombies [i].gameObject.GetComponent<Zombie>();
			//Debug.Log (z.tag + " ALERTED FV " + z.name);
				if (z.ChasingPlayer()) {
				if (!SeesPlayer ()) {
					isChasingPlayer = false;
					isFollowingZombie = true;
					agent.SetDestination (z.transform.position);
					agent.speed = zombieWalkingSpeed;
					bodyAnimator.SetBool ("isWalking", true);
					bodyAnimator.SetBool ("isAttacking", false);
					transform.LookAt (z.transform);
					transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0);
				} else {
					isFollowingZombie = false;
				}
			}
		}
	}

	bool SeesPlayer(){
		float distance = Vector3.Distance (player.transform.position, transform.position);
		RaycastHit hit;
		Vector3 rayDirection = player.transform.position - transform.position;

		if (Physics.Raycast (transform.position, rayDirection, out hit)) {
			if ((hit.transform.tag == "Player") && (distance < detectionDistance)) {
				return true;
			}
		}
		Debug.DrawRay ((transform.position + new Vector3(0,0.3f,0)), rayDirection, Color.green);
		if (Vector3.Angle (rayDirection, transform.forward) < fov) {
			if (Physics.Raycast ((transform.position + new Vector3(0,0.3f,0)), rayDirection, out hit, eyeSight)) {
				//Debug.Log (hit.transform.tag);
				if (hit.transform.tag != "Wall") {
					return true;
				} else {
					return false;
				}
			}
		}
		return false;
	}

	public bool ChasingPlayer(){
		if (isChasingPlayer) {
			return true;
		}
		return false;
	}

	void Update () {
		bool seesPlayer = SeesPlayer ();
		if(player!= null){
			healthBarCanvas.transform.LookAt (player);
		}
		float actualDistance = Vector3.Distance (player.transform.position, transform.position);

		if (isDead) {
			return;
		} else {
			if (seesPlayer) {
				Collider[] zombies = Physics.OverlapSphere (transform.position, 20, zombieLayer);
				for(int i = 0; i < zombies.Length; ++i){
					zombies [i].SendMessage ("Alerted");
				}
				//Debug.Log (zombies.Length);
				setZombieAttributes (actualDistance);
				if (isChasingPlayer && !isAttacking) {
					bodyAnimator.SetBool ("isWalking", true);
					bodyAnimator.SetBool ("isAttacking", false);
					transform.LookAt (player);
					transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0);

					agent.SetDestination (player.position);
					agent.speed = zombieWalkingSpeed;

					if (GetComponent<AudioSource> ().clip == zombieAttacking) {
						GetComponent<AudioSource> ().Stop ();
						GetComponent<AudioSource> ().clip = zombieWalking;
					}
					if(!GetComponent<AudioSource>().isPlaying){
						GetComponent<AudioSource> ().PlayOneShot (zombieWalking);			//*------------------- HANG, DE ÚGY KÉNE HOGY FÉLBESZAKAD AMIKOR KILÉP AZ IFBŐL ------*
					}

				} else if (!isChasingPlayer && isAttacking) {
					agent.speed = 0;
					transform.LookAt (player);
					transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0); //ne nézzen felfelé
					bodyAnimator.SetBool ("isAttacking", true);
					bodyAnimator.SetBool ("isWalking", false);
					MeleeAttack ();

					if (GetComponent<AudioSource> ().clip == zombieWalking) {
						GetComponent<AudioSource> ().Stop ();
						GetComponent<AudioSource> ().clip = zombieAttacking;
					}

					if (!GetComponent<AudioSource> ().isPlaying) {
						GetComponent<AudioSource> ().PlayOneShot (zombieAttacking);
					}

				} 
			}else {
				agent.speed = 0;
				bodyAnimator.SetBool ("isAttacking", false);
				bodyAnimator.SetBool ("isWalking", false);
			}


			/*
			setZombieAttributes (actualDistance);
			if ((isChasing && !isAttacking) || isAttacked) {
				bodyAnimator.SetBool ("isWalking", true);
				bodyAnimator.SetBool ("isAttacking", false);
				transform.LookAt (player);
				transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0);

				agent.SetDestination (player.position);
				agent.speed = zombieWalkingSpeed;

				if (GetComponent<AudioSource> ().clip == zombieAttacking) {
					GetComponent<AudioSource> ().Stop ();
					GetComponent<AudioSource> ().clip = zombieWalking;
				}
				if(!GetComponent<AudioSource>().isPlaying){
					GetComponent<AudioSource> ().PlayOneShot (zombieWalking);			//*------------------- HANG, DE ÚGY KÉNE HOGY FÉLBESZAKAD AMIKOR KILÉP AZ IFBŐL ------*
				}

			} else if (!isChasing && isAttacking) {
				agent.speed = 0;
				transform.LookAt (player);
				transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0); //ne nézzen felfelé
				bodyAnimator.SetBool ("isAttacking", true);
				bodyAnimator.SetBool ("isWalking", false);
				MeleeAttack ();

				if (GetComponent<AudioSource> ().clip == zombieWalking) {
					GetComponent<AudioSource> ().Stop ();
					GetComponent<AudioSource> ().clip = zombieAttacking;
				}

				if (!GetComponent<AudioSource> ().isPlaying) {
					GetComponent<AudioSource> ().PlayOneShot (zombieAttacking);
				}

			} else {
				agent.speed = 0;
				bodyAnimator.SetBool ("isAttacking", false);
				bodyAnimator.SetBool ("isWalking", false);
			}*/
		}

	
	}

	void MeleeAttack(){
		if (Time.time > nextAttack) {
			nextAttack = Time.time + hitRate;
			Collider[] colliders = Physics.OverlapSphere (transform.position, attackRange);	//Get all colliders in range, if its a player, hit
			foreach (Collider hit in colliders) {
				if (hit && hit.tag == "Player") {
					float dist = Vector3.Distance (hit.transform.position, transform.position);
					if (dist <= attackRange) {
						hit.gameObject.GetComponent<Player> ().TakeDamage (damage);
					}
				}
			}
		}
	}

	void OnCollisionEnter(Collision other){
		Debug.Log ("Hit");
	}


	IEnumerator DealDamage(){
		yield return new WaitForSeconds (hitRate);
	}

	IEnumerator AggroZombies(){
		yield return new WaitForSeconds (1.0f);
	}


	public void TakeDamage(int damage){
		if (isDead) {
			return;
		}
		isAttacked = true;
		health -= damage;
		if(zombieBlood.isPlaying){
			zombieBlood.Stop ();
		}
		zombieBlood.Emit (30);
		healthBar.fillAmount = (float)health / (float)maxHealth;

		if (health <= 0) {
			isDead = true;
			bodyAnimator.Play ("Die");
			StartCoroutine ("DestroyZombie");
			agent.speed = 0;
			GetComponent<AudioSource> ().Stop ();
			GetComponent<AudioSource> ().PlayOneShot (zombieDeath);

		}
	}

	IEnumerator DestroyZombie() {
		yield return new WaitForSeconds(1.5f);
		Destroy (gameObject);
	}

}
