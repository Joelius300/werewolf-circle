import { createApp } from 'vue';
import piniaInstance from '@/stores/piniaProvider';
import App from './App.vue';
import router from './router';

const app = createApp(App)
  .use(piniaInstance)
  .use(router);

app.mount('#app');
