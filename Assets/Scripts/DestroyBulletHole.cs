using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBulletHole : MonoBehaviour
{
	void Start ()
	{
		Destroy (gameObject, 5);
	}
}
