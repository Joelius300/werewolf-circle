## Summary

This concept is a lot simpler than the API one and probably easiest to implement for prototyping etc.

With this concept, everything is done via the realtime (SignalR) connection. That means creating a game, joining a game, updating a game, etc. is all done over an active connection.

This is simple because:

- We can identify users by connection-id because the ongoing connection is obviously stateful unlike HTTP with an API. This allows us to avoid topics like JWT etc. for now but makes it much harder to identify the user once disconnected (a rejoin token or similar has to be used).
- Everything is done in the same place and less abstraction is requried. It's just less to think about and allows for a less complex implementation.
- I don't think it's standard-conform so I can do what I want but it's messy and not a good candiate to showcase later :)

## Questions

### How are users identified?

The connection-id of the active SignalR connection.

### How to revover from connection loss?

Currently not possible. A simple solution would be to store a unique rejoin token used for identifying the user when they first connect. Subsequent identifications are done via the connection-id.

## Scenarios

All scenarios follow the same general schema.

1. User visits a site
2. Frontend connects to SignalR hub
3. User does something
4. Frontend invokes hub method on the server
5. Backend responds directly (return value) and or with a SignalR call of its own

This is valid for creating a game, joining a game, leaving a game and all game updates and events.
