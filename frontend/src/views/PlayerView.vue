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
import { defineComponent } from 'vue';
import { useRouter } from 'vue-router';
import PlayerCircle from '@/components/PlayerCircle.vue';
import useGameStore from '@/stores/game';

export default defineComponent({
  name: 'PlayerView',
  components: {
    PlayerCircle,
  },
  setup() {
    const router = useRouter();
    const store = useGameStore();

    async function leave(): Promise<void> {
      await store.leaveGame();
      router.push('/');
    }

    if (store.game == null) {
      throw new Error('No Game in store.');
    }

    const { game } = store;

    return {
      players: game.players,
      leave,
    };
  },
});
</script>
