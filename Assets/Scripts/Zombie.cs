using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Zombie : MonoBehaviour
{

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
	private bool isAttacked;
	private SoundManager manager;
	private Transform shotPosition;

	float speedY;
	float lookDirection = 0;
	bool moveZombie = false;
	int zombiesNearby;
	bool isSpeedPositive;

	void Start ()
	{
		isDead = false;
		isChasingPlayer = false;
		isAttacking = false;
		isAttacked = false;
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		maxHealth = health;
		healthBar = transform.Find ("EnemyCanvas").Find ("HealthBar").Find ("Health").GetComponent<Image> ();
		GetComponent<AudioSource> ().clip = zombieWalking;
		GetComponent<AudioSource> ().Stop ();

		InvokeRepeating ("IdleMovement", 0, 4f);
		InvokeRepeating ("MoveZombies", 2, 4);
	}

	void setZombieAttributes (float actualDistance)
	{
		if (actualDistance > attackRange) {
			if (SeesPlayer ()) {
				isChasingPlayer = true;
			}
			isAttacking = false;

		} else if (actualDistance <= attackRange) {
			isChasingPlayer = false;
			isAttacking = true;

		} 
	}

	float sumArray (float[] summed)
	{
		float sum = 0;
		foreach (float stuff in summed) {
			sum += stuff;
		}

		return sum;
	}

	float angleBetween (Vector3 v1, Vector3 v2, Vector3 n)
	{
		float angle = Vector3.Angle (v1, v2);
		float sign = Mathf.Sign (Vector3.Dot (n, Vector3.Cross (v1, v2)));
		float signed_angle = angle * sign;
		return signed_angle;
	}


	void IdleMovement ()
	{
		if (isChasingPlayer) {
			return;
		}
		Collider[] zombies = Physics.OverlapSphere (transform.position, moveDetectionDistance, zombieLayer);
		zombiesNearby = zombies.Length;
		float averageDistX;

		float[] distancesToOtherZombies = new float[zombies.Length];
		float[] zombieForwardAngles = new float[zombies.Length];
		float[] mainZombieToOthersAngles = new float[zombies.Length];

		Vector3[] mainZombieToOthersVectors = new Vector3[zombies.Length];
		Vector3[] zombieForwardVectors = new Vector3[zombies.Length];

		for (int i = 0; i < zombies.Length; ++i) {
			if (!zombies [i].gameObject.transform.Equals (gameObject.transform)) {
				Transform z = zombies [i].gameObject.transform;
				zombieForwardVectors [i] = z.forward;
				mainZombieToOthersVectors [i] = z.position - transform.position;
				distancesToOtherZombies [i] = Vector3.Distance (transform.position, z.position);
				mainZombieToOthersAngles [i] = angleBetween (transform.forward, (z.position - transform.position), new Vector3 (1, 1, 1));
			}
		}
		averageDistX = sumArray (distancesToOtherZombies) / (distancesToOtherZombies.Length);
		if (zombiesNearby > 2) {
			speedY = (averageDistX * (-0.2f)) + 3f;
			lookDirection += Random.Range (0, 180);
		} else if (zombiesNearby == 2) {
			speedY = (averageDistX * (-0.2f)) + 2.5f;
		}

		agent.speed = speedY;

		if (zombiesNearby > 1) {
			lookDirection = sumArray (mainZombieToOthersAngles);				//Lonely zombies shouldnt move
			lookDirection /= zombieForwardAngles.Length - 1;
			lookDirection += transform.eulerAngles.y;
		}
	}

	void MoveZombies ()
	{	
		if (isChasingPlayer) {
			return;
		}
		if (zombiesNearby > 1) {
			Vector3 angles = transform.eulerAngles;
			angles.y = lookDirection;
			transform.eulerAngles = angles;
			moveZombie = true;
		} else {
			moveZombie = false;
		}

	}

	void Alerted ()
	{
		if (SeesPlayer ()) {
			return;
		}
		Collider[] zombies = Physics.OverlapSphere (transform.position, alertDistance, zombieLayer);
		for (int i = 0; i < zombies.Length; ++i) {
			Zombie z = zombies [i].gameObject.GetComponent<Zombie> ();
			if (z.ChasingPlayer ()) {
				if (!SeesPlayer ()) {
					isChasingPlayer = false;
					agent.SetDestination (z.transform.position);
					agent.speed = zombieWalkingSpeed;
					bodyAnimator.SetBool ("isWalking", true);
					bodyAnimator.SetBool ("isAttacking", false);
					transform.LookAt (z.transform);
					transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0);
				}
			}
		}
	}

	bool SeesPlayer ()
	{
		float distance = Vector3.Distance (player.transform.position, transform.position);
		RaycastHit hit;
		Vector3 rayDirection = player.transform.position - transform.position;

		if (Physics.Raycast (transform.position, rayDirection, out hit)) {
			if ((hit.transform.tag == "Player") && (distance < detectionDistance)) {
				return true;
			}
		}

		if (Vector3.Angle (rayDirection, transform.forward) < fov) {
			
			if (Physics.Raycast ((transform.position + new Vector3 (0, 2f, -1f)), (rayDirection + new Vector3 (0, -1.3f, 1f)), out hit, eyeSight)) {
				if (hit.transform.tag == "Player") {
					return true;
				} else {
					return false;
				}
			}
		}
		return false;
	}

	public void setShotPosition (Transform shotPos)
	{
		shotPosition = shotPos;
	}

	public bool ChasingPlayer ()
	{
		if (isChasingPlayer) {
			return true;
		}
		return false;
	}


	void Update ()
	{		
		bool seesPlayer = SeesPlayer ();
		if (player != null) {
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
						GetComponent<AudioSource> ().PlayOneShot (zombieWalking);			
					}

				} else if (!isChasingPlayer && isAttacking) {
					agent.speed = 0;
					transform.LookAt (player);
					transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0); 
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
			} else if (moveZombie && !seesPlayer) {
				if (!isSpeedPositive) {
					Vector3 angles = transform.eulerAngles;
					angles.y += 180;
					transform.eulerAngles = angles;
					isSpeedPositive = true;
				}
				transform.position += transform.forward * Time.deltaTime * speedY;
				bodyAnimator.SetBool ("isAttacking", false);
				bodyAnimator.SetBool ("isWalking", true);
			} else {				
				setZombieAttributes (actualDistance);
				agent.speed = 0;
				bodyAnimator.SetBool ("isAttacking", false);
				bodyAnimator.SetBool ("isWalking", false);
			}
		}
	}

	void MeleeAttack ()
	{
		if (Time.time > nextAttack) {
			nextAttack = Time.time + hitRate;
			Collider[] colliders = Physics.OverlapSphere (transform.position, attackRange);	
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

	IEnumerator DealDamage ()
	{
		yield return new WaitForSeconds (hitRate);
	}

	IEnumerator AggroZombies ()
	{
		yield return new WaitForSeconds (1.0f);
	}


	public void TakeDamage (int damage)
	{
		if (isDead) {
			return;
		}
		health -= damage;
		if (zombieBlood.isPlaying) {
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

	IEnumerator DestroyZombie ()
	{
		yield return new WaitForSeconds (1.5f);
		Destroy (gameObject);
	}

	IEnumerator WaitASecond ()
	{
		yield return new WaitForSeconds (3f);
	}

}