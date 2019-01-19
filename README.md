# ZMQ Connection Between Python and Unity

## Unity Respond, Python Request

### Execute

Under `UnityResponder`, run `request.py`, then start game mode of `demo.unity`

Lighting on the sphere will flash with 1 second interval, according to the command "on" or "off" sent from the Python script `request.py`. Unity will send back acknowledgement that it has received the command, which `request.py` will print.

## Unity Request, Python Respond

### Execute

Under `UnityRequester`, run `respond.py`, then start game mode of `demo.unity`

After typing into the input field and hitting enter, the message will be sent to `respond.py` and unity will wait for a response for 3 seconds. If there is a response, it will be printed to console. 

The requester is run asynchronously so the UI won't be blocked.

## Note

For the C# code to compile, set editor to .NET 4.6

`File -> Build Settings -> Player Settings... -> Other Settings -> Scripting Runtime Version -> Experimental (.NET 4.6 Equivalent)`

## Acknowledgement

Compiled AsyncIO and NetMQ and multithreading structure is from [here](https://github.com/valkjsaaa/Unity-ZeroMQ-Example)
