<template>
  <div class="container">
    <div class="card" style="height: 30vh; width: 50vw;">
      <label for="roomInput">Room-Id</label>
      <input id="roomInput" v-model="roomId" />
      <label for="nameInput">Username</label>
      <input id="nameInput" v-model="username" />
      <button @click="join">Join existing game</button>
      <p>or</p>
      <button @click="create">Create a new game</button>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref } from 'vue';
import { useRouter } from 'vue-router';
import GameService from '@/services/GameService';

export default defineComponent({
  name: 'LobbyView',
  props: {
    initialRoomId: String,
  },
  setup(props) {
    const router = useRouter();
    const roomId = ref(props.initialRoomId ?? '');
    const username = ref('');

    async function joinCore(room: string, name: string) {
      return router.push({ name: 'PlayerView', params: { roomId: room, name } });
    }

    async function create(): Promise<void> {
      const gameService = new GameService();
      await gameService.ensureConnected();
      const createdRoomId = await gameService.createGame();

      // TODO This is bad.. I should be using some state-management
      // instead of passing state via route without url params
      // https://dev.to/blacksonic/you-might-not-need-vuex-with-vue-3-52e4
      await joinCore(createdRoomId, 'Admin');
    }

    async function join() {
      return joinCore(roomId.value, username.value);
    }

    return {
      create,
      roomId,
      username,
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
