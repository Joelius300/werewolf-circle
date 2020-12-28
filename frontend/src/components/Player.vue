<template>
  <div :style="style">
    <span>{{ player.name }}</span>
  </div>
</template>

<script lang="ts">
import { defineComponent, PropType } from 'vue';
import Player from '@/model/Player';

export default defineComponent({
  name: 'Player',
  props: {
    player: {
      type: Object as PropType<Player>,
      required: true,
    },
    radius: {
      type: String,
      required: true,
    },
  },
  computed: {
    style(): object {
      return {
        backgroundColor: this.player.color,
        '--d': `calc(${this.radius} * 2)`,
      };
    },
  },
});
</script>

<style scoped lang="scss">
div {
  position: absolute;
  top: 50%;
  left: 50%;
  margin: calc(-.5 * var(--d));
  width: var(--d);
  height: var(--d);
  border-radius: 50%;
  display: flex;
  justify-content: center;
  align-items: center;
}
</style>
