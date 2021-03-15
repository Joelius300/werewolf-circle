import Player from './Player';

export default interface Game {
  roomId: string;
  players: Player[];
}
