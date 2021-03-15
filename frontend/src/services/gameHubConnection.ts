import { HubConnection } from '@microsoft/signalr';

/**
 * Abstraction layer for the hub connection with typed callbacks and vue-like unsubscribe return values.
 */
export default abstract class GameHubConnection {
  public static onPlayerJoined(hubConnection: HubConnection, callback: (name: string) => void): () => void {
    return GameHubConnection.addCallback(hubConnection, 'PlayerJoined', callback);
  }

  public static onPlayerLeft(hubConnection: HubConnection, callback: (name: string) => void): () => void {
    return GameHubConnection.addCallback(hubConnection, 'PlayerLeft', callback);
  }

  public static onGameDestroyed(hubConnection: HubConnection, callback: () => void): () => void {
    return GameHubConnection.addCallback(hubConnection, 'GameDestroyed', callback);
  }

  /* eslint-disable-next-line @typescript-eslint/no-explicit-any */
  private static addCallback(hubConnection: HubConnection, name: string, func: (...args: any[]) => void): () => void {
    if (!hubConnection) throw new Error('No hubConnection provided');

    if (name == null) throw new Error('The event name cannot be null or undefined.');

    if (func == null) throw new Error('The callback cannot be null or undefined.');

    hubConnection.on(name, func);

    const unsubscribe = () => {
      hubConnection.off(name, func);
    }

    return unsubscribe;
  }
}
