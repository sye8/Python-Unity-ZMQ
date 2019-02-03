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

	public delegate void MessageDelegate(string obj);
	private readonly MessageDelegate messageDelegate;

	public ParallelRequester(MessageDelegate msgDelegate){
		requester = new Thread(RequesterWorker);
		messageDelegate = msgDelegate;
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
						messageDelegate(msg);
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

[System.Serializable]
public class JSONDemo
{
	public string uuid;
	public float x;
	public float y;

	public override string ToString(){
		return "UUID: " + uuid + "; x: " + x + "; y: " + y;
	}
}

public class Request : MonoBehaviour {

	public ParallelRequester requester;

	public Transform spawnPos;
	public GameObject sphere;
	public GameObject cube;

	private string reply = "";

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<InputField>().onEndEdit.AddListener(InputFieldHandler);
		requester = new ParallelRequester(MsgDelegate);
		requester.Start();
	}
	
	// Update is called once per frame
	void Update () {
		if (!string.Equals (reply, "")) {
			int spaceIndex = reply.IndexOf (' ');
			string type = reply.Substring (0, spaceIndex);
			string args = reply.Substring (spaceIndex + 1);

			switch (type) {
				case "Spawn":
					switch (args) {
					case "sphere":
						Instantiate (sphere, spawnPos.position, spawnPos.rotation);
						Debug.Log ("Spawning a sphere");
						break;
					case "cube":
						Instantiate (cube, spawnPos.position, spawnPos.rotation);
						Debug.Log ("Spawning a cube");
						break;
					default:
						Debug.Log ("Spawn: Unrecognized object");
						break;
					}
					break;
				case "JSON":
					Debug.Log (JsonUtility.FromJson<JSONDemo> (args));
					break;
				case "Error":
					Debug.Log ("ResponderError: " + args);
					break;
				default:
					Debug.Log (reply);
					break;
				}
			reply = "";
		}
	}

	private void InputFieldHandler(string txt){
		requester.Update(txt);
	}

	private void MsgDelegate(string msg){
		reply = msg;
	}

}
