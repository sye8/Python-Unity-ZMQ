using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using NetMQ;
using NetMQ.Sockets;

using UnityEngine;
using UnityEngine.UI;


public class ParallelRequester{
	private readonly Thread requester;

	private bool requesterStopped;

	private string sendMsg = "";

	public delegate void MessageDelegate(string message);

	public ParallelRequester(){
		requester = new Thread(RequesterWorker);
	}

	private void RequesterWorker(){
		AsyncIO.ForceDotNet.Force();

		var reqSocket = new RequestSocket("tcp://localhost:5556");

		try{
			while(!requesterStopped){
				if(!string.Equals(sendMsg, "")){
					reqSocket.SendFrame(sendMsg);
					Debug.Log("Send: " + sendMsg);
					sendMsg = "";

					string msg;
					if (reqSocket.TryReceiveFrameString(TimeSpan.FromSeconds(3), out msg)) {
						Debug.Log("Received: " + msg);
					}
				}
			}
		}finally{
			if (reqSocket != null) {
				((IDisposable)reqSocket).Dispose();
			}
		}
	}

	public void Update(string msg){
		sendMsg = msg;
	}

	public void Start(){
		requesterStopped = false;
		requester.Start();
	}

	public void Stop(){
		requesterStopped = true;
		requester.Join();
	}
}

public class Request : MonoBehaviour {

	public ParallelRequester requester;

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<InputField>().onEndEdit.AddListener(InputFieldHandler);
		requester = new ParallelRequester();
		requester.Start();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void InputFieldHandler(string txt){
		requester.Update(txt);
	}
}
