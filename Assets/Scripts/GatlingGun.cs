using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingGun : Gun {

	override protected void Update(){
		base.Update();
		if (Input.GetMouseButton (0) && (Time.time - lastTimeFired) > rateOfFire) {
			lastTimeFired = Time.time;
			Fire ();
		}
	}

}
