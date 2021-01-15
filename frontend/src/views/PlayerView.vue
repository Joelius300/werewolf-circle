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
  onBeforeMount,
  onUnmounted,
  ref,
} from 'vue';
import { useRouter } from 'vue-router';
import PlayerCircle from '@/components/PlayerCircle.vue';
import Player from '@/model/Player';
import GameService from '@/services/GameService';
import useMainStore from '@/stores/main';

export default defineComponent({
  name: 'PlayerView',
  components: {
    PlayerCircle,
  },
  props: {
    roomId: {
      type: String,
      required: true,
    },
  },
  async setup(props) {
    const store = useMainStore();
    const players = ref([] as Player[]);
    const router = useRouter();
    const gameService = new GameService();
    const name = store.playerName;

    function addPlayer(playerName: string): void {
      players.value.push({ name: playerName, color: '#808080' });
    }

    function removePlayer(playerName: string): void {
      const index = players.value.findIndex((p) => p.name === playerName);
      players.value.splice(index, 1);
    }

    async function join(): Promise<void> {
      console.log(`joining game ${props.roomId}`);
      const names = await gameService.joinGame(props.roomId, name);
      for (let i = 0; i < names.length; i++) {
        addPlayer(names[i]);
      }

      addPlayer(name);
    }

    async function leave(): Promise<void> {
      await gameService.leaveGame();
      store.isInGame = false;
      router.push('/');
    }

    gameService.onPlayerJoined(addPlayer);
    gameService.onPlayerLeft(removePlayer);

    // Maybe this could help something?
    // https://medium.com/javascript-in-plain-english/handling-asynchrony-in-vue-3-composition-api-part-1-managing-async-state-e993842ebf8f
    // https://medium.com/javascript-in-plain-english/handling-asynchrony-with-vue-composition-api-and-vue-concurrency-part-2-canceling-throttling-4e0305c82367
    onBeforeMount(async () => {
      try {
        await gameService.ensureConnected();
        await join();

        store.isInGame = true;
      } catch (e) {
        // This should not be guarded here but before loading this component.
        // Just leave it like this until reworking it to the API/JWT concept.
        alert(e);
        router.go(-1);
      }
    });

    onUnmounted(() => gameService.stopListening());

    return {
      players,
      leave,
    };
  },
});
</script>
