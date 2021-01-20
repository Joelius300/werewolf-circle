import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';
import useGameStore from '@/stores/game';
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
    beforeEnter: (to, _from, next) => {
      const store = useGameStore(piniaInstance);
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

router.beforeEach((to, _from, next) => {
  const store = useGameStore(piniaInstance);

  if (store.isInGame && store.roomId && to.name !== 'PlayerView') {
    next(`/${store.roomId}`);
    return;
  }

  next();
});

export default router;
