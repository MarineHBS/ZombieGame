using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKControl : MonoBehaviour
{

	protected Animator animator;

	public bool ikActive = false;
	public Transform rightHandIndexFinger = null;
	public Transform leftHandIndexFinger = null;
	public Transform[] rightLookObj = null;
	public Transform[] leftLookObj = null;

	WeaponHolder wp;

	void Start ()
	{
		animator = GetComponent<Animator> ();
		wp = GameObject.FindGameObjectWithTag ("Player").GetComponent<WeaponHolder> ();
	}

	void OnAnimatorIK ()
	{
		animator.SetIKPositionWeight (AvatarIKGoal.RightHand, 1);
		animator.SetIKRotationWeight (AvatarIKGoal.RightHand, 1);

		animator.SetIKPositionWeight (AvatarIKGoal.LeftHand, 1);
		animator.SetIKRotationWeight (AvatarIKGoal.LeftHand, 1);

		switch (wp.GetActiveWeaponConstraint ()) {
		case "Pistol":
			animator.SetIKPosition (AvatarIKGoal.RightHand, rightLookObj [0].position);
			animator.SetIKRotation (AvatarIKGoal.RightHand, rightLookObj [0].rotation);
			break;
		case "Ak47":
			animator.SetIKPosition (AvatarIKGoal.RightHand, rightLookObj [1].position);
			animator.SetIKRotation (AvatarIKGoal.RightHand, rightLookObj [1].rotation);

			animator.SetIKPosition (AvatarIKGoal.LeftHand, leftLookObj [1].position);
			animator.SetIKRotation (AvatarIKGoal.LeftHand, leftLookObj [1].rotation);
			break;
		case "GatlingGun":
			animator.SetIKPosition (AvatarIKGoal.RightHand, rightLookObj [2].position);
			animator.SetIKRotation (AvatarIKGoal.RightHand, rightLookObj [2].rotation);

			animator.SetIKPosition (AvatarIKGoal.LeftHand, leftLookObj [2].position);
			animator.SetIKRotation (AvatarIKGoal.LeftHand, leftLookObj [2].rotation);
			break;
		default:
			break;
		}
	}
}
