# PerformantSocketServer

This is a socket server built on top of SocketAsyncEvents.

### Basic Usage

* Implement an instance of IMessageHandler.  This is where your processing goes.
* Implement an instance of ISocketStateData for passing around connection specific state.
* Optionally implement an instance of IServerTrace for logging.
* Create a instance of `SocketListener<T>` where T is an instance of ISocketStateData (that you created above).
* Call `StartListen` and the magic should begin.

## Change Notes

### 2.0.0-beta3

* (Breaking) Now include the number of active connections in the watchdog.

### 2.0.0-beta2

* (Breaking) Now include the server configuration in the watchdog (for when you need reference values or the custom object).

### 2.0.0-beta1

* (Breaking) Can now indicate the behaviour of the socket after processing is complete.  Before, no data to send back meant closing the socket.  You can now send back no data and continue to listen.
* (Breaking) Added support for a very loosely typed connection state object to track state across a connection while it's open (for example to log application state data that only shows up when it first talks)
* Added a custom object for the SocketListenerSettings so that multiple instances of the listener can have some kind of unique state to identify it by.
* (Breaking) Added more fields to IServerTrace, and distinguished between starting to Send, and the Send calling back with progress.
