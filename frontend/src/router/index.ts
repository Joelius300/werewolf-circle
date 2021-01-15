import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';
import useMainStore from '@/stores/main';
import piniaInstance from '@/stores/piniaProvider';
import isValidPlayerName from '@/services/validators/PlayerNameValidator';
import PlayerView from '../views/PlayerView.vue';
import LobbyView from '../views/LobbyView.vue';

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'Lobby',
    component: LobbyView,
  },
  {
    path: '/:roomId',
    name: 'PlayerView',
    component: PlayerView,
    props: true,
    beforeEnter: (to, from, next) => {
      const store = useMainStore(piniaInstance);
      if (!isValidPlayerName(store.playerName)) {
        store.roomId = to.params.roomId as string;
        next('/');
      } else {
        next();
      }
    },
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

router.beforeEach((to, from, next) => {
  const store = useMainStore(piniaInstance);

  if (store.isInGame && store.roomId && to.name !== 'PlayerView') {
    next(`/${store.roomId}`);
    return;
  }

  next();
});

export default router;
