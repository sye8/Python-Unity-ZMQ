using System;
using System.Collections;
using System.Threading.Tasks;

using UnityEngine;

using NetMQ;
using NetMQ.Sockets;


public class Responder : MonoBehaviour {
	public Light light;
	public bool lightIsOn = false; 

	private bool responderIsStarted = false;


	void Respond(){
		AsyncIO.ForceDotNet.Force();

		var socket = new ResponseSocket("tcp://*:5558");

		try{
			while(responderIsStarted){
				string inMsg;
				if(!socket.TryReceiveFrameString(out inMsg)){
					continue;
				}
				Debug.Log("Received: " + inMsg);
				if(inMsg == "on"){
					lightIsOn = true;
					socket.SendFrame("Light is on");
				}else if(inMsg == "off"){
					lightIsOn = false;
					socket.SendFrame("Light is off");
				}
			}
		}finally{
			if (socket != null) {
                socket.Close();
				((IDisposable)socket).Dispose();
                NetMQConfig.Cleanup(true);
            }
		}
	}

	// Use this for initialization
	void Start () {
		lightIsOn = false;
		responderIsStarted = true;
		Task task = new Task (async() => Respond());
		task.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		if (lightIsOn) {
			light.enabled = true;
		} else {
			light.enabled = false;
		}
	}

	void OnDestroy(){
		responderIsStarted = false;
	}
}
