import { defineStore } from 'pinia';

export default defineStore({
  id: 'main',
  state: () => ({
    roomId: '',
    playerName: '',
    isInGame: false,
  }),
});
