# PerformantSocketServer

This is a socket server built on top of SocketAsyncEvents.

### Basic Usage

* Implement an instance of IMessageHandler.  This is where your processing goes.
* Implement an instance of IListenerStateData for passing around listener specific state.
* Implement an instance of ISocketStateData for passing around connection specific state.
* Optionally implement an instance of IServerTrace for logging.
* Optionally implement an instance of IMonitor for performance metric tracking.
* Create a instance of `SocketListener<T, TU>` where T is an instance of ISocketStateData (that you created above), and TU is an instance of IListenerStateData (that you also created above).
* Call `StartListen` and the magic should begin.

## Change Notes

### 2.0.0-beta4

* The custom state object for the listener is now a generic `IListenerStateData`.  This further helps for when you have multiple instances of a socket listener.
* (Breaking) `IListenerStateData` type is now in all the places where it was `object` before.
* (Breaking) Most of the IServerTrace methods now also have the IListenerStateData object.

### 2.0.0-beta3

* (Breaking) Now include the number of active connections in the watchdog.

### 2.0.0-beta2

* (Breaking) Now include the server configuration in the watchdog (for when you need reference values or the custom object).

### 2.0.0-beta1

* (Breaking) Can now indicate the behaviour of the socket after processing is complete.  Before, no data to send back meant closing the socket.  You can now send back no data and continue to listen.
* (Breaking) Added support for a very loosely typed connection state object to track state across a connection while it's open (for example to log application state data that only shows up when it first talks)
* Added a custom object for the SocketListenerSettings so that multiple instances of the listener can have some kind of unique state to identify it by.
* (Breaking) Added more fields to IServerTrace, and distinguished between starting to Send, and the Send calling back with progress.
