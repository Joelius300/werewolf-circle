<template>
  <div class="container" :style="containerStyle">
    <PlayerComponent class="player" v-for="(player, index) in players" :key="player.name"
                     :player="player" :radius="playerRadius"
                     :style="{ '--a': `${angle * index}rad` }" />
  </div>
</template>

<script lang="ts">
import { defineComponent, PropType } from 'vue';
import Player from '@/model/Player';
import PlayerComponent from '@/components/Player.vue';

export default defineComponent({
  name: 'PlayerCircle',
  components: {
    PlayerComponent,
  },
  props: {
    players: {
      type: Array as PropType<Player[]>,
      required: true,
    },
    containerSize: {
      type: String,
      required: true,
    },
    minPlayerRadius: {
      type: String,
      default: '2em',
    },
    maxPlayerRadius: {
      type: String,
      default: '3em',
    },
    minPlayerDistance: {
      type: String,
      default: '20px',
    },
  },
  computed: {
    angle(): number {
      return (Math.PI * 2) / this.players.length;
    },
    sin(): number {
      return Math.sin(this.angle / 2);
    },
    playerRadius(): string {
      return `calc(max(min((${this.sin} * var(--r) * 2 - ${this.minPlayerDistance}) / 2, ${this.maxPlayerRadius}), ${this.minPlayerRadius}))`;
    },
    containerStyle(): object {
      return {
        // So far so good but now there's too much room when the playerRadius isn't maximized
        // because we go from the maxPlayerRadius. Something would need to be adjusted here
        // or in playerRadius but to work around it don't set a large maxPlayerRadius.
        // Maybe using margin or padding would be an option.
        '--r': `calc(var(--s) / 2 - ${this.maxPlayerRadius})`,
        '--s': this.containerSize,
      };
    },
  },
});
</script>

<style scoped lang="scss">
.container {
  position: relative;
  width: var(--s);
  height: var(--s);

  .player {
    /* for whatever reason this can't be bound to a prop
       otherwise you get a _this is not defined error.. */
    transition-duration: 650ms;
    transition-property: transform, width, height, margin;
    transform:
      rotate(var(--a))
      translate(var(--r))
      rotate(calc(-1 * var(--a)));
  }
}
</style>
