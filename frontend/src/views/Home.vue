<template>
  <div class="home">
    <PlayerCircle :players="players" containerSize="80vh" />
  </div>
  <button @click="removePlayer">Remove</button>
  <button @click="addPlayer">Add</button>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import PlayerCircle from '@/components/PlayerCircle.vue'; // @ is an alias to /src
import Player from '@/model/Player';

export default defineComponent({
  name: 'Home',
  components: {
    PlayerCircle,
  },
  data() {
    return {
      players: [] as Player[],
    };
  },
  methods: {
    getNewPlayer(index: number): Player {
      return {
        name: String.fromCharCode(65 + index).repeat(index + 1), // 65 = A
        color: `hsla(${Math.random() * 360}, 100%, 50%, 1)`,
      };
    },
    removePlayer(): void {
      this.players.pop();
    },
    addPlayer(): void {
      this.players.push(this.getNewPlayer(this.players.length));
    },
  },
  created() {
    for (let i = 0; i < 5; i++) {
      this.players.push(this.getNewPlayer(i));
    }
  },
});
</script>
