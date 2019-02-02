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

	public delegate void InstantiateDelegate(string obj);
	private readonly InstantiateDelegate instantiateDelegate;

	public ParallelRequester(InstantiateDelegate instDelegate){
		requester = new Thread(RequesterWorker);
		instantiateDelegate = instDelegate;
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
						instantiateDelegate(msg);
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
	public string name;
	public float x;
	public float y;

	public override string ToString(){
		return "Name: " + name + "; x: " + x + "; y: " + y;
	}
}

public class Request : MonoBehaviour {

	public ParallelRequester requester;

	public Transform spawnPos;
	public GameObject sphere;
	public GameObject cube;

	private string spawn = "";

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<InputField>().onEndEdit.AddListener(InputFieldHandler);
		requester = new ParallelRequester(Instantiator);
		requester.Start();
	}
	
	// Update is called once per frame
	void Update () {
		if (string.Equals(spawn, "sphere")) {
			Instantiate(sphere, spawnPos.position, spawnPos.rotation);
			Debug.Log("Spawning a sphere");
			spawn = "";
		}else if (string.Equals(spawn, "cube")) {
			Instantiate (cube, spawnPos.position, spawnPos.rotation);
			Debug.Log ("Spawning a cube");
			spawn = "";
		} else if (!string.Equals(spawn, "")){
			Debug.Log(JsonUtility.FromJson<JSONDemo>(spawn));
			spawn = "";
		}
	}

	private void InputFieldHandler(string txt){
		requester.Update(txt);
	}

	private void Instantiator(string obj){
		spawn = obj;
	}

}
