<template>
  <div>
    <PlayerCircle
      :players="players"
      container-size="90vh"
    />
  </div>
  <button @click="leave">
    Leave
  </button>
</template>

<script lang="ts">
import {
  defineComponent,
  onUnmounted,
  toRefs,
} from 'vue';
import { useRouter } from 'vue-router';
import PlayerCircle from '@/components/PlayerCircle.vue';
import GameService from '@/services/GameService';
import useGameStore from '@/stores/game';

export default defineComponent({
  name: 'PlayerView',
  components: {
    PlayerCircle,
  },
  setup() {
    const store = useGameStore();
    const { players } = toRefs(store);
    const router = useRouter();
    const gameService = new GameService();

    function addPlayer(playerName: string): void {
      // TODO The server should send full player data
      // (name, state (alive?) and whatever more there will be for normal players)
      players.value.push({ name: playerName, color: '#808080' });
    }

    function removePlayer(playerName: string): void {
      const index = players.value.findIndex((p) => p.name === playerName);
      players.value.splice(index, 1);
    }

    async function leave(): Promise<void> {
      await gameService.leaveGame();
      router.push('/');
    }

    gameService.onPlayerJoined(addPlayer);
    gameService.onPlayerLeft(removePlayer);

    onUnmounted(() => gameService.stopListening());

    return {
      players,
      leave,
    };
  },
});
</script>
