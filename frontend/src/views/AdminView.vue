<template>
  <div>
    <PlayerCircle
      :players="players"
      container-size="90vh"
    />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import PlayerCircle from '@/components/PlayerCircle.vue';
import useGameStore from '@/stores/game';

export default defineComponent({
  name: 'AdminView',
  components: {
    PlayerCircle,
  },
  setup() {
    const store = useGameStore();

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
