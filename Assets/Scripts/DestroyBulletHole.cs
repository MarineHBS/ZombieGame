﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBulletHole : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Destroy (gameObject, 5);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 forward = transform.TransformDirection(Vector3.forward);
		RaycastHit hit;

	}
}