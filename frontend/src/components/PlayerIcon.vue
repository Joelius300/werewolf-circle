<template>
  <div :style="style">
    <span>{{ playerName }}</span>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent } from 'vue';

export default defineComponent({
  name: 'PlayerIcon',
  props: {
    playerName: {
      type: String,
      required: true,
    },
    radius: {
      type: String,
      required: true,
    },
    bgColor: {
      type: String,
      default: '#808080',
    },
  },
  setup(props) {
    const style = computed(() => ({
      backgroundColor: props.bgColor,
      '--d': `calc(${props.radius} * 2)`,
    }));

    return {
      style,
    };
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
