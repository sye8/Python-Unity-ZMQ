using System;

using System.Collections.Concurrent;
using System.Threading;

using NetMQ;
using NetMQ.Sockets;

using UnityEngine;

public class ZMQListener{
	private readonly Thread listener;
	private bool listenerStopped;
	public delegate void MessageDelegate(string message);
	private readonly MessageDelegate msgDelegate;
	private readonly ConcurrentQueue<string> msgQueue = new ConcurrentQueue<string>();

	public ZMQListener(MessageDelegate messageDelegate){
        msgDelegate = messageDelegate;
        listener = new Thread(ListenerWorker);
    }

	private void ListenerWorker(){
		AsyncIO.ForceDotNet.Force();

		var respSocket = new ResponseSocket(); 

		try{
			respSocket.Bind("tcp://*:5558");

			while(!listenerStopped){
				string frameString;
				if(!respSocket.TryReceiveFrameString(out frameString)){
					continue;
				}
				Debug.Log(frameString);
				msgQueue.Enqueue(frameString);
				respSocket.SendFrame("Command " + frameString + "received. Will Execute.");
			}
			respSocket.Close();
		}finally{ // Destructor (GC)
			if(respSocket != null){
				((IDisposable)respSocket).Dispose();
			}
		}
		NetMQConfig.Cleanup();
	}

	public void Update(){

        while (!msgQueue.IsEmpty){
            string message;
            if (msgQueue.TryDequeue(out message)){
                msgDelegate(message);
            }
            else{
                break;
            }
        }
    }

    public void Start(){
        listenerStopped = false;
        listener.Start();
    }

    public void Stop(){
        listenerStopped = true;
        listener.Join();
    }
}

public class RespondLight : MonoBehaviour{
	public ZMQListener zmq;
	public Light light;

	void MessageHandler(string msg){
		if(string.Equals(msg, "on")){
			light.enabled = true;
		}else if(string.Equals(msg, "off")){
			light.enabled = false;
		}else{
			return;
		}
	}

	void Start(){
		zmq = new ZMQListener(MessageHandler);
		zmq.Start();
	}

	void Update(){
		zmq.Update();
	}

	void OnDestroy(){
		zmq.Stop();
	}
}