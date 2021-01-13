<template>
  <div class="container" :style="containerStyle">
    <PlayerIcon class="player" v-for="(player, index) in players" :key="player.name"
                     :player="player" :radius="playerRadius"
                     :style="{ '--a': `${angle * index}rad` }" />
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, PropType } from 'vue';
import Player from '@/model/Player';
import PlayerIcon from '@/components/PlayerIcon.vue';

export default defineComponent({
  name: 'PlayerCircle',
  components: {
    PlayerIcon,
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
  setup(props) {
    const angle = computed(() => (Math.PI * 2) / props.players.length);
    const sin = computed(() => Math.sin(angle.value / 2));
    const playerRadius = computed(() => `calc(max(min((${sin.value} * var(--r) * 2 - ${props.minPlayerDistance}) / 2, ${props.maxPlayerRadius}), ${props.minPlayerRadius}))`);
    const containerStyle = computed(() => ({
      // So far so good but now there's too much room when the playerRadius isn't maximized
      // because we go from the maxPlayerRadius. Something would need to be adjusted here
      // or in playerRadius but to work around it don't set a large maxPlayerRadius.
      // Maybe using margin or padding would be an option.
      '--r': `calc(var(--s) / 2 - ${props.maxPlayerRadius})`,
      '--s': props.containerSize,
    }));

    return {
      angle,
      sin,
      playerRadius,
      containerStyle,
    };
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
