import Player from '@/model/Player';
import { defineStore } from 'pinia';

export default defineStore({
  id: 'game',
  state: () => ({
    roomId: '',
    playerName: '',
    isInGame: false,
    players: [] as Player[],
  }),
});
