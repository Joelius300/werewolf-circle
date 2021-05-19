import { DeepReadonly } from 'vue';
import JwtToken from '@/model/JwtToken';
import Game from '@/model/Game';
import jwtDecode from 'jwt-decode';
import { defineStore } from 'pinia';
import { createGame, joinGame, leaveGame } from '@/services/gameService';
import { token } from './tokenStore';

const useGameStore = defineStore({
  id: 'game',
  state: () => ({
    game: null as (DeepReadonly<Game> | null),
    // TODO Move this in a new store (e.g. main store) where other things like preferences and stuff
    // would be. Maybe this can be solved more elegantly anyway, ask SO about the routing?
    initialRoomId: '',
  }),
  getters: {
    token(): string | null {
      return token.value;
    },
    decodedToken(): JwtToken | null {
      return this.token ? jwtDecode<JwtToken>(this.token) : null;
    },
    playerName(): string | undefined {
      return this.decodedToken?.given_name;
    },
    isAdmin(): boolean | undefined {
      return this.decodedToken?.role === 'Admin';
    },
    roomId(): string | undefined {
      return this.decodedToken?.roomId;
    },
  },
  actions: {
    createGame() {
      return createGame(this);
    },
    joinGame(roomId: string, playerName: string) {
      return joinGame(this, roomId, playerName);
    },
    leaveGame() {
      return leaveGame(this);
    },
  },
});

export default useGameStore;
