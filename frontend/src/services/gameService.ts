import
{
  DeepReadonly,
  reactive,
  readonly,
  watch,
} from 'vue';
import axios from '@/services/axios';
import { HubConnection } from '@microsoft/signalr';
import Player from '@/model/Player';
import Game from '@/model/Game';
import { setToken } from '@/stores/tokenStore';
import { ensureHubConnected } from './hubService';
import GHC from './gameHubConnection';

/*
These functions are closely related to the game store and extracted to avoid bloat in the store definition.
They feature some weird typing to mimic the necessary properties of 'this' in the game store.
*/
declare type MinimalGameStore = { game: DeepReadonly<Game> | null; readonly roomId: string | undefined };
declare type MinimalGameStoreWithInternalState = MinimalGameStore & { $state: { game: DeepReadonly<Game> | null } };

function subscribeToGameEvents(hubConnection: HubConnection, game: Game) {
  const unsubPlayerJoined = GHC.onPlayerJoined(hubConnection, (n) => game.players.push({ name: n, color: '#808080' }));
  const unsubPlayerLeft = GHC.onPlayerLeft(hubConnection, (name) => {
    const index = game.players.findIndex((p) => p.name === name);
    game.players.splice(index, 1);
  });

  return () => {
    unsubPlayerJoined();
    unsubPlayerLeft();
  }
}

function getGame(roomId: string, players?: Player[]): Game {
  return reactive({ roomId, players: players || [] });
}

async function connectGame(store: MinimalGameStoreWithInternalState, gameMut: Game) {
  if (!store.game || !gameMut) {
    throw new Error('The supplied game cannot be null and has to be stored in the store as well (just readonly).');
  }

  const connection = await ensureHubConnected();
  const unsubFromEvents = subscribeToGameEvents(connection, gameMut);

  // I'd love to avoid directly accessing the "internal" state here but
  // 0. () => store.game and using toRefs both don't work
  // 1. it watches exactly what I want it to watch unlike $subscribe for example
  const watchStopHandle = watch(() => store.$state.game, (newVal) => {
    if (newVal == null) {
      unsubFromEvents();
      watchStopHandle();
    }
  });
}

export async function createGame(store: MinimalGameStoreWithInternalState): Promise<Game> {
  const { token: jwt } = (await axios.post<{ token: string }>('/game/create')).data;
  setToken(jwt);

  /* eslint-disable-next-line @typescript-eslint/no-non-null-assertion */
  const game = getGame(store.roomId!);
  store.game = readonly(game);

  await connectGame(store, game);

  return game;
}

export async function joinGame(store: MinimalGameStoreWithInternalState, roomId: string, playerName: string): Promise<Game> {
  const body = { PlayerName: playerName, RoomId: roomId };
  const response = await axios.post<{ token: string; players: string[] }>('/game/join', body);
  const { token: jwt, players: playerNames } = response.data;
  setToken(jwt);

  const players = playerNames.map<Player>((n: string) => ({ name: n, color: n === playerName ? '#fd912a' : '#808080' }));
  /* eslint-disable-next-line @typescript-eslint/no-non-null-assertion */
  const game = getGame(store.roomId!, players);
  store.game = readonly(game);

  await connectGame(store, game);

  return game;
}

export async function leaveGame(store: MinimalGameStore): Promise<void> {
  await axios.post('/game/leave');

  store.game = null;
  setToken(null);
}
