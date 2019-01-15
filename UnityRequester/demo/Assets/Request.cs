using System;
using System.Collections;
using System.Collections.Generic;

using NetMQ;
using NetMQ.Sockets;

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
		using (var reqSocket = new RequestSocket ("tcp://localhost:5556")) {
			reqSocket.SendFrame(txt);
			Debug.Log("Send: " + txt);

			string msg;
			if (reqSocket.TryReceiveFrameString(TimeSpan.FromSeconds(3), out msg)) {
				Debug.Log("Receive: " + msg);
			}
		}
	}
}
