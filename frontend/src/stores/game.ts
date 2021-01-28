import Game from '@/services/Game';
import { defineStore } from 'pinia';

// This game store is managed by GameService instances.
// Do not manually mutate it without good reasoning.
export default defineStore({
  id: 'game',
  state: () => ({
    roomId: '',
    playerName: '' as (string | null),
    game: null as (Game | null),
  }),
});
