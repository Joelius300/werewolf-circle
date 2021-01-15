import { createApp } from 'vue';
import piniaInstance from '@/stores/piniaProvider';
import App from './App.vue';
import router from './router';

createApp(App)
  .use(router)
  .use(piniaInstance)
  .mount('#app');
