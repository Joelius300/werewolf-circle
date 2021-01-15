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
import { defineComponent, toRefs } from 'vue';
import { useRouter } from 'vue-router';
import GameService from '@/services/GameService';
import useMainStore from '@/stores/main';

export default defineComponent({
  name: 'LobbyView',
  setup() {
    const store = useMainStore();
    const router = useRouter();

    async function joinCore() {
      return router.push({ name: 'PlayerView', params: { roomId: store.roomId } });
    }

    async function create(): Promise<void> {
      const gameService = new GameService();
      await gameService.ensureConnected();
      const createdRoomId = await gameService.createGame();

      store.roomId = createdRoomId;
      store.playerName = 'Admin';
      await joinCore();
    }

    async function join() {
      return joinCore();
    }

    const { roomId, playerName } = toRefs(store);

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
