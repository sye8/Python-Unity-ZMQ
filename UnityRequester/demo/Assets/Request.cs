using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Request : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<InputField>().onEndEdit.AddListener(InputFieldHandler);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void InputFieldHandler(string txt){
		print(txt);
	}
}
