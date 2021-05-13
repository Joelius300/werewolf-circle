import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';
import useGameStore from '@/stores/game';
import piniaInstance from '@/stores/piniaProvider';
import isValidPlayerName from '@/services/validators/playerNameValidator';
import { initializeTokenStore } from '@/stores/tokenStore';
import PlayerView from '../views/PlayerView.vue';
import LobbyView from '../views/LobbyView.vue';
import AdminView from '../views/AdminView.vue';

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
        store.initialRoomId = to.params.roomId as string;
        next('/');
      } else {
        next();
      }
    },
  },
  {
    path: '/:roomId/admin',
    name: 'AdminView',
    component: AdminView,
    beforeEnter: (to, _from, next) => {
      const store = useGameStore(piniaInstance);
      if (store.playerName == null && store.game != null) {
        next();
      } else {
        store.initialRoomId = to.params.roomId as string;
        next('/');
      }
    },
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

router.beforeEach((_to, _from, next) => {
  const store = useGameStore(piniaInstance);
  if (!store.token) {
    if (!initializeTokenStore()) {
      next('/');
      return;
    }
  }

  if (!store.game) {
    // TODO
    // store.rejoinGame();
    console.log('rejoining game');
    store.game = { roomId: '', players: [] };
  } else {
    next();
    return;
  }

  if (store.isAdmin) {
    next(`/${store.roomId}/admin`);
  } else {
    next(`/${store.roomId}`);
  }
});

export default router;
