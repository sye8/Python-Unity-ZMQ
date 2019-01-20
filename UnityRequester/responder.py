import zmq
import time

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5556")

while True:
    msg = socket.recv()
    print("Received Request with Message: " + msg)
    args = msg.split()
    if args[0] == "gib":
        if len(args) > 1:
            socket.send(args[1])
        else:
            socket.send("gib usage: gib [object]")
    elif args[0] == "ping":
        socket.send("ping back from server")
    else:
        socket.send("Unidentified command")
