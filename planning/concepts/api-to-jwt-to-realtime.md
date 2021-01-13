## Summary
A more complex concept which more closely imitates a potential real world application with authentication etc.

Instead of doing everything over an ongoing connection, only the game updates are done in realtime. Everything else is done with an API. This means that we can use JWT to identify the user and make sure they only join the correct game and have the correct permissions etc. It also opens doors for allowing the same person to connect from multiple devices but I don't think that'll ever be a requirement.

**Realtime:**
- Game events (player joined/died/left, role/faction changed)

**API**:
- Create game
- Join game

**Up for discussion:**
- Adjusting game properties (possible roles) -> probably API
  - Realtime would allow easy two-way communication
  - Realtime changes would be commited one-by-one
  - API changes can be done one-by-one or in bulk (easier to implement "save" feature)
  - API is more standard to fetch the roles, put them in a drop down and send a PUT for updating the game

## Questions
### How are users identified?
Via the JWT stored in the users browser.

### How to revover from connection loss?
Opening the website will automatically redirect the user to the active game if they have a valid JWT stored. Recovering from connection loss is therefore as easy as reloading the page.

## JWT data
- Unique playername (since it's only unique within a certain game, we probably can't use the sub field otherwise the backend (SignalR) might get confused but not sure)
- Room-Id
- Expires in {the same interval used for automatic deletion of games}

## Scenarios
### User creates game
0. User is not currently associated with an active game
1. User browses to the root page
2. User clicks the create game button
3. Frontend sends API request to backend
4. Backend creates the game and returns the room-id as well as a JWT for the admin user
5. Frontend stores JWT
6. Frontend connects to SignalR Hub and passes the JWT
7. Frontend redirects to admin page for just created game and waits for events

### User joins game
0. User is not currently associated with an active game
1. User browses to the root- or game-page
2. User inputs room-id and playername and clicks the join button
3. Frontend sends API request to backend with room-id and playername
4. Assuming both values are valid, the backend adds the user to the game and returns a JWT for the player
5. Backend emits player joined event for all other users in the game
6. Frontend stores JWT
7. Frontend connects to SignalR Hub and passes the JWT
8. Frontend redirects to player page for just created game and waits for events

### User (player) leaves game
0. User is currently registered as player in an active game
1. User browses to the game-page (actually redirected there)
2. User clicks the leave button
3. Frontend sends API request to backend
4. Assuming the request and state is valid, the backend removes the player from the game
5. The backend emits player left event for all other users in the game
6. Frontend deletes JWT
7. Frontend redirects to root-page
