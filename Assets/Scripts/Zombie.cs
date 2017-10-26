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
	public ParticleSystem zombieBlood;
	public Canvas healthBarCanvas;
	public float alertDistance;
	public float fov;
	public LayerMask zombieLayer;
	public int eyeSight;
	public float moveDetectionDistance;

	UnityEngine.AI.NavMeshAgent agent;

	private Image healthBar;
	private float nextAttack = 0;
	private Transform player;
	private int maxHealth;
	private bool isDead;
	private bool isChasingPlayer;
	private bool isAttacking;
	//private bool isAttacked;
	//private bool isFollowingZombie;
	private SoundManager manager;

	float speedY;
	float lookDirection = 0;
	bool moveZombie = false;
	int zombiesNearby;
	bool isSpeedPositive;

	void Start () {
		isDead = false;
		isChasingPlayer = false;
		isAttacking = false;
		//isAttacked = false;
		//isFollowingZombie = false;
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		maxHealth = health;
		healthBar = transform.Find ("EnemyCanvas").Find("HealthBar").Find("Health").GetComponent<Image> ();
		GetComponent<AudioSource> ().clip = zombieWalking;
		GetComponent<AudioSource> ().Stop ();

		InvokeRepeating ("IdleMovement", 0, 4f);
		//IdleMovement ();
		InvokeRepeating ("MoveZombies", 2, 4);
	}

	void setZombieAttributes(float actualDistance){
		//if (actualDistance <= detectionDistance && actualDistance > attackRange) {
		if (actualDistance > attackRange) {
			if (SeesPlayer ()) {
				isChasingPlayer = true;
			}
			isAttacking = false;

		} else if (actualDistance <= attackRange) {
			isChasingPlayer = false;
			isAttacking = true;

		} /*else {
			isChasing = false;
			isAttacking = false;
		}*/
	}

	float sumArray(float[] summed){
		float sum = 0;
		foreach (float stuff in summed) {
			sum += stuff;
		}

		return sum;
	}

	float angleBetween(Vector3 v1, Vector3 v2, Vector3 n){
		//angle in [0,180]
		float angle = Vector3.Angle(v1, v2);
		float sign = Mathf.Sign (Vector3.Dot (n, Vector3.Cross (v1, v2)));

		//angle in [-179, 180]
		float signed_angle = angle*sign;

		//angle in [0, 360]
		//float angle360 = (signed_angle + 180)%360;

		return signed_angle;
	}


	void IdleMovement(){
		Collider[] zombies = Physics.OverlapSphere (transform.position, moveDetectionDistance, zombieLayer);
		//StartCoroutine ("Alerted");
		zombiesNearby = zombies.Length;
		//float lookDirection = 0;
		//float speedY;
		float averageDistX;

		float[] distancesToOtherZombies = new float[zombies.Length];
		float[] zombieForwardAngles = new float[zombies.Length];
		float[] mainZombieToOthersAngles = new float[zombies.Length];

		Vector3[] mainZombieToOthersVectors = new Vector3[zombies.Length];
		Vector3[] zombieForwardVectors = new Vector3[zombies.Length];

		for (int i = 0; i < zombies.Length; ++i) {
			if (!zombies[i].gameObject.transform.Equals(gameObject.transform)) {
				Transform z = zombies [i].gameObject.transform;
				zombieForwardVectors [i] = z.forward;
				mainZombieToOthersVectors[i] = z.position - transform.position;
				distancesToOtherZombies[i] = Vector3.Distance (transform.position, z.position);
				//zombieForwardAngles [i] = Vector3.Angle (z.transform.forward, transform.forward);
				mainZombieToOthersAngles[i] = angleBetween (transform.forward, (z.position - transform.position), new Vector3 (1, 1, 1));
			}
		}

		for (int i = 0; i < distancesToOtherZombies.Length; ++i) {
			//Debug.Log (gameObject.name + "'s distance to others: " + distancesToOtherZombies [i]);
			//Debug.Log (distancesToOtherZombies.Length);
		}
		averageDistX = sumArray (distancesToOtherZombies) / (distancesToOtherZombies.Length);
		if (zombiesNearby > 2) {
			speedY = (averageDistX * (-0.2f)) + 3f;
			lookDirection += Random.Range(0, 180);
		} else if (zombiesNearby == 2) {
			speedY = (averageDistX * (-0.2f)) + 2.5f;
		}
		if (speedY < 0) {		//Stop them from moonwalking
			//lookDirection += 180;
		}

		agent.speed = speedY;
		Debug.Log (gameObject.name + "'s speed: " + speedY);


		/*for (int i = 0; i < zombieForwardVectors.Length; ++i) {
			//Debug.Log (gameObject.name + "'s forward vectors are: " + zombieForwardVectors [i]);
			zombieForwardAngles [i] = angleBetween (transform.forward, zombieForwardVectors [i], new Vector3 (1, 1, 1));
			mainZombieToOthersAngles[i] = angleBetween (transform.forward, mainZombieToOthersVectors [i], new Vector3 (1, 1, 1));
		}*/

		for (int i = 0; i < zombieForwardAngles.Length; ++i) {
			//zombieForwardAngles [i] = Vector3.Angle (transform.forward, zombieForwardVectors[i]);
			//Debug.Log (gameObject.name + "'s angle to others: " + mainZombieToOthersAngles[i]);
		}

		/*
		for (int i = 0; i < zombieForwardAngles.Length; ++i) {
			//zombieForwardAngles [i] = Vector3.Angle (transform.forward, zombieForwardVectors[i]);
			Debug.Log (gameObject.name + "'s angle to others: " + angleBetween(transform.forward, zombieForwardVectors[i], new Vector3(1,1,1)));
		}*/

		/*
		if (zombiesNearby > 1) {
			lookDirection = sumArray (zombieForwardAngles) - 90;				//Lonely zombies shouldnt move
				lookDirection /= zombieForwardAngles.Length - 1;
				Debug.Log (gameObject.name + "'s lookdirection is : " + lookDirection);
				Debug.Log (gameObject.name + "'s starting lookdirection is : " + transform.eulerAngles.y);
				lookDirection += transform.eulerAngles.y;
				Debug.Log (gameObject.name + "'s after fixed lookdirection is : " + lookDirection);

		}*/

		if (zombiesNearby > 1) {
			lookDirection = sumArray (mainZombieToOthersAngles);				//Lonely zombies shouldnt move
			lookDirection /= zombieForwardAngles.Length-1;
			//Debug.Log (gameObject.name + "'s lookdirection is : " + lookDirection);
			//Debug.Log (gameObject.name + "'s starting lookdirection is : " + transform.eulerAngles.y);
			lookDirection += transform.eulerAngles.y;
			//lookDirection += 90;
			//Debug.Log (gameObject.name + "'s after fixed lookdirection is : " + lookDirection);

		}

	

	}

	void MoveZombies(){														// NOT RANDOM LOOKDIRECTION, NOT WORKING BECAUSE ZOMBIES WILL FACE THE SAME DIRECTION
		if (zombiesNearby > 1) {
		Debug.Log (lookDirection + " " + speedY);
		Vector3 angles = transform.eulerAngles;
		angles.y = lookDirection;
		transform.eulerAngles = angles;
		moveZombie = true;
		} else {
			moveZombie = false;
		}

	}
	/*
	void MoveZombies(){
		if (zombiesNearby > 1 || !isChasingPlayer) {
			Vector3 angles = transform.eulerAngles;
			angles.y = Random.Range (0, 360);
			if (speedY < 0) {
				isSpeedPositive = false;
			} else {
				isSpeedPositive = true;
			}
			transform.eulerAngles = angles;
			moveZombie = true;
		} else {
			moveZombie = false;
		}
	}*/


	void Alerted(){
		if (SeesPlayer ()) {
			return;
		}
		Collider[] zombies = Physics.OverlapSphere (transform.position, alertDistance, zombieLayer);
		for(int i = 0; i < zombies.Length; ++i){
			Zombie z = zombies [i].gameObject.GetComponent<Zombie>();
				if (z.ChasingPlayer()) {
				if (!SeesPlayer ()) {
					isChasingPlayer = false;
					//isFollowingZombie = true;
					agent.SetDestination (z.transform.position);
					agent.speed = zombieWalkingSpeed;
					bodyAnimator.SetBool ("isWalking", true);
					bodyAnimator.SetBool ("isAttacking", false);
					transform.LookAt (z.transform);
					transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0);
				} else {
					//isFollowingZombie = false;
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
		//Debug.DrawRay ((transform.position + new Vector3(0,2f,-1f)), (rayDirection + new Vector3(0, -1.3f, 1f)), Color.green);
		//Debug.Log(Vector3.Angle (rayDirection, transform.forward) + gameObject.name);

		if (Vector3.Angle (rayDirection, transform.forward) < fov) {
			
			if (Physics.Raycast ((transform.position + new Vector3(0,2f,-1f)), (rayDirection + new Vector3(0, -1.3f, 1f)), out hit, eyeSight)) {
				Debug.Log (hit.collider.gameObject.tag);
				if (hit.transform.tag == "Player") {
					//Debug.Log(Vector3.Angle (rayDirection, transform.forward) + gameObject.name);
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

	void OnDrawGizmosSelected(){
		//Gizmos.color = Color.yellow;
		//Gizmos.DrawWireSphere (transform.position, 20);
	}

	void Update () {

		//Gizmos.color = Color.yellow;
		//Gizmos.DrawSphere (transform.position, 20);
		
		bool seesPlayer = SeesPlayer ();
		if(player!= null){
			healthBarCanvas.transform.LookAt (player);
		}
		float actualDistance = Vector3.Distance (player.transform.position, transform.position);

		if (isDead) {
			return;
		} else {
			if (seesPlayer) {
				Collider[] zombies = Physics.OverlapSphere (transform.position, alertDistance, zombieLayer);
				for (int i = 0; i < zombies.Length; ++i) {
					zombies [i].SendMessage ("Alerted");
				}
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
					if (!GetComponent<AudioSource> ().isPlaying) {
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
			} else if (moveZombie) {
				if (!isSpeedPositive) {
					Vector3 angles = transform.eulerAngles;
					angles.y += 180;
					transform.eulerAngles = angles;
					isSpeedPositive = true;
				}
				//setZombieAttributes (actualDistance);
				transform.position += transform.forward * Time.deltaTime * speedY;
				Debug.Log (isChasingPlayer);
				bodyAnimator.SetBool ("isAttacking", false);
				bodyAnimator.SetBool ("isWalking", true);
			} else {
				setZombieAttributes (actualDistance);
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
		//isAttacked = true;
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
	IEnumerator WaitASecond() {
		yield return new WaitForSeconds(3f);
	}

}
