import random
import sys
import time

import zmq

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5556")

while True:

    # Decode received bytes to string
    # Version dependent
    if sys.version_info[0] < 3:
        msg = socket.recv().decode('utf-8')
    else:
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
    elif args[0] == "jsonTest":
        socket.send_string("{\"name\":\"test\",\"x\":" + str(random.uniform(-5.0, 5.0)) + ",\"y\":" + str(random.uniform(-5.0, 5.0)) + "}")
    else:
        socket.send_string("Unidentified command")
