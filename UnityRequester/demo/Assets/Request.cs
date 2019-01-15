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

			var msg = reqSocket.ReceiveFrameString();
			Debug.Log("Receive: " + msg);
		}
	}
}
