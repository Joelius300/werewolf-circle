import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';
import PlayerView from '../views/PlayerView.vue';
import LobbyView from '../views/LobbyView.vue';

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'Lobby',
    component: LobbyView,
    // beforeEnter: Guard that player is not in game
  },
  {
    path: '/:initialRoomId',
    name: 'LobbyWithId',
    component: LobbyView,
    props: true,
    // beforeEnter: Guard that player is not in game
  },
  {
    path: '/:roomId',
    name: 'PlayerView',
    component: PlayerView,
    props: true,
    // beforeEnter: Guard that player is in game
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

export default router;
