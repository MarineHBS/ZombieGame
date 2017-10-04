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


	UnityEngine.AI.NavMeshAgent agent;

	private Image healthBar;
	private float nextAttack = 0;
	private Transform player;
	private int maxHealth;
	private bool isDead;
	private bool isWalking;
	private bool isAttacking;
	private bool isAttacked;

	void Start () {
		isDead = false;
		isWalking = false;
		isAttacking = false;
		isAttacked = false;
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		maxHealth = health;
		healthBar = transform.Find ("EnemyCanvas").Find("HealthBar").Find("Health").GetComponent<Image> ();
		GetComponent<AudioSource> ().clip = zombieWalking;
	}

	private void setZombieAttributes(float actualDistance){
		if (actualDistance <= detectionDistance && actualDistance > attackRange) {
			isWalking = true;
			isAttacking = false;

		} else if (actualDistance <= attackRange) {
			isWalking = false;
			isAttacking = true;

		} else {
			isWalking = false;
			isAttacking = false;
		}
	}

	void Update () {

		//RaycastHit hit;
		Vector3 directionOfLine = player.transform.position - transform.position;
		float angle = Vector3.Angle (directionOfLine, transform.forward);
		if (angle < 5) {
			RaycastHit hit;
			Debug.Log ("sikerült");
			if(Physics.Raycast(transform.position + transform.up, directionOfLine.normalized, out hit, 10)){
				if (hit.collider.gameObject == player || hit.collider.tag == "Player") {
					transform.LookAt (player);
					Debug.Log ("sikerült");
				}
			}
		}
		/*
		//Debug.DrawRay ((transform.position + (Vector3.left * 6) + (Vector3.forward * 4)), directionOfLine * 3, Color.green);
		Debug.DrawRay (transform.position + Vector3.up * 1.5f, directionOfLine, Color.green);
		//Debug.DrawRay (transform.position + Vector3.up*1.5, directionOfLine, Color.green);
		Debug.DrawRay (transform.position + Vector3.up, directionOfLine, Color.green);
		if(Physics.Raycast((transform.position + (Vector3.up * 6) + (Vector3.forward * 4)), directionOfLine, out hit, 50)){
			if(hit.collider.tag == "Player"){
				transform.LookAt (player);
			}
		}*/
	




			/*

		if(player!= null){
			healthBarCanvas.transform.LookAt (player);
		}
		float actualDistance = Vector3.Distance (player.transform.position, transform.position);

		if (isDead) {
			return;
		} else {
			setZombieAttributes (actualDistance);
			if ((isWalking && !isAttacking) || isAttacked) {
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

			} else if (!isWalking && isAttacking) {
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
			}
		}
		*/
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


	IEnumerator DealDamage(){
		yield return new WaitForSeconds (hitRate);
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
		yield return new WaitForSeconds(5.5f);
		Destroy (gameObject);
	}

}
