<template>
  <div class="container">
    <div
      class="card"
      style="height: 30vh; width: 50vw;"
    >
      <label for="roomInput">Room-Id</label>
      <input
        id="roomInput"
        v-model="roomId"
      >
      <label for="nameInput">Username</label>
      <input
        id="nameInput"
        v-model="playerName"
      >
      <button @click="join">
        Join existing game
      </button>
      <p>or</p>
      <button @click="create">
        Create a new game
      </button>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref } from 'vue';
import { useRouter } from 'vue-router';
import GameService from '@/services/GameService';
import useGameStore from '@/stores/game';

export default defineComponent({
  name: 'LobbyView',
  setup() {
    const store = useGameStore();
    const router = useRouter();
    const roomId = ref(store.roomId ?? '');
    const playerName = ref('');
    const gameService = new GameService();

    async function join() {
      await gameService.joinGame(roomId.value, playerName.value);

      return router.push(`/${roomId.value}`);
    }

    async function create() {
      const game = await gameService.createGame();

      return router.push(`/${game.roomId}/admin`);
    }

    return {
      create,
      roomId,
      playerName,
      join,
    };
  },
});
</script>

<style lang="scss" scoped>
.container {
  position: fixed;
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;

  .card {
    display: flex;
    flex-direction: column;
  }

  /* input {
    display: inline-flex;
    width: 60%;
    align-self: center;
  } */
}
</style>
