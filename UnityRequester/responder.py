import zmq
import time

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5556")

while True:
    msg = str(socket.recv(), 'utf-8')
    print("Received Request with Message: " + msg)
    args = msg.split()
    if args[0] == "gib":
        if len(args) > 1:
            socket.send_string(args[1])
        else:
            socket.send_string("gib usage: gib [object]")
    elif args[0] == "ping":
        socket.send_string("ping back from server")
    else:
        socket.send_string("Unidentified command")
