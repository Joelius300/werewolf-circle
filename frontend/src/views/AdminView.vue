<template>
  <div>
    <PlayerCircle
      :players="players"
      container-size="90vh"
    />
  </div>
</template>

<script lang="ts">
import {
  defineComponent,
  onUnmounted,
} from 'vue';
import PlayerCircle from '@/components/PlayerCircle.vue';
import GameService from '@/services/GameService';
import useGameStore from '@/stores/game';

export default defineComponent({
  name: 'AdminView',
  components: {
    PlayerCircle,
  },
  setup() {
    const store = useGameStore();
    const gameService = new GameService();

    onUnmounted(() => gameService.stopListening());

    if (store.game == null) {
      throw new Error('No Game in store.');
    }

    const { game } = store;

    return {
      players: game.players,
    };
  },
});
</script>
