import { createApp } from 'vue';
import piniaInstance from '@/stores/piniaProvider';
import App from './App.vue';
import router from './router';
// import { initializeTokenStore } from './stores/tokenStore';

// initializeTokenStore(); <-- uncomment when rejoining is supported BUT NOT HERE. I think this should be done before routing because the code here could in theory be run on the server if we used SSR (right?).

const app = createApp(App)
  .use(piniaInstance)
  .use(router);

app.mount('#app');
