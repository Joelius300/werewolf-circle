import Player from '@/model/Player';
import { DeepReadonly, readonly, Ref } from 'vue';

export default class Game {
  public readonly players: DeepReadonly<Ref<Player[]>>;

  constructor(public readonly roomId: string, _players: Ref<Player[]>) {
    this.players = readonly(_players);
  }
}
