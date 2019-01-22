using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (this.transform.position.y <= 200) {
			Destroy();
		}
	}

	void Destroy(){
		Destroy (this.gameObject);
	}
}
