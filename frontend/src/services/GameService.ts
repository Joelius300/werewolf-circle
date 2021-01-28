import Player from '@/model/Player';
import { Ref, ref } from 'vue';
import useGameStore from '@/stores/game';
import Game from './Game';
import hubConnectionProvider from './HubConnectionProvider';
/* eslint-disable class-methods-use-this */

// with minor downsides (and some upsides?), this could also be done with something like
// https://github.com/bterlson/strict-event-emitter-types
/**
 * Handles the communication with the server and manages the game store.
 */
export default class GameService {
  /* eslint-disable-next-line @typescript-eslint/no-explicit-any */
  private callbacks: { [eventName: string]: ((...args: any[]) => void)[] } = {};

  // reexposed for convenience, HubConnectionProvider should only be used to stop the connection
  public ensureConnected(): Promise<void> {
    return hubConnectionProvider.ensureConnected();
  }

  /* eslint-disable-next-line @typescript-eslint/no-explicit-any */
  private addCallback(name: string, func: (...args: any[]) => void) {
    if (name == null) throw new Error('The event name cannot be null or undefined.');

    if (func == null) throw new Error('The callback cannot be null or undefined.');

    if (this.callbacks[name] == null) this.callbacks[name] = [];

    this.callbacks[name].push(func);
    hubConnectionProvider.connection.on(name, func);
  }

  /**
   * Removes all listeners/callbacks without interrupting the connection.
   * This disconnects all games created by this instance and removes all handlers
   * for the onX methods.
   */
  public stopListening(): void {
    const eventNames = Object.keys(this.callbacks);
    for (let i = 0; i < eventNames.length; i++) {
      const eventName = eventNames[i];
      const events = this.callbacks[eventName];
      for (let j = 0; j < events.length; j++) {
        hubConnectionProvider.connection.off(eventName, events[j]);
        delete events[j];
      }
      delete this.callbacks[eventName];
    }
  }

  public onPlayerJoined(callback: (name: string) => void): void {
    this.addCallback('PlayerJoined', callback);
  }

  public onPlayerLeft(callback: (name: string) => void): void {
    this.addCallback('PlayerLeft', callback);
  }

  public async createGame(): Promise<Game> {
    await this.ensureConnected();

    const store = useGameStore();
    const roomId = await hubConnectionProvider.connection.invoke('CreateGame') as string;

    const players = ref([] as Player[]);
    const game = new Game(roomId, players);
    this.hookupGameEvents(players);

    store.game = game;
    store.roomId = roomId;
    store.playerName = null;

    return game;
  }

  public async joinGame(roomId: string, playerName: string): Promise<Game> {
    await this.ensureConnected();

    const store = useGameStore();
    const playerNames = await hubConnectionProvider.connection.invoke('JoinGame', roomId, playerName) as string[];

    const players = ref(playerNames.map((n) => ({ name: n, color: n === playerName ? '#fd912a' : '#808080' })));
    const game = new Game(roomId, players);
    this.hookupGameEvents(players);

    store.game = game;
    store.roomId = roomId;
    store.playerName = playerName;

    return game;
  }

  private hookupGameEvents(players: Ref<Player[]>) {
    this.onPlayerJoined((name) => players.value.push({ name, color: '#808080' }));
    this.onPlayerLeft((name) => {
      const index = players.value.findIndex((p) => p.name === name);
      players.value.splice(index, 1);
    });
  }

  public async leaveGame(): Promise<void> {
    await this.ensureConnected();
    await hubConnectionProvider.connection.invoke('LeaveGame');
    const store = useGameStore();
    store.game = null;
    store.roomId = '';
    store.playerName = null;
  }
}
