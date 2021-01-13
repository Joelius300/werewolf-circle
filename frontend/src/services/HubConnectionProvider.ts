import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';

class HubConnectionProvider {
  private hubConnection: HubConnection;

  public get connection(): HubConnection {
    return this.hubConnection;
  }

  constructor() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:5000/gameHub') // process.env.BASE_URL TODO
      .build();
  }

  /**
   * Starts the connection if it's disconnected. It will no-op otherwise
   * (which includes connecting and disconnecting).
   */
  public ensureConnected(): Promise<void> {
    if (this.hubConnection.state === HubConnectionState.Disconnected) {
      return this.hubConnection.start();
    }

    return Promise.resolve();
  }

  public stopConnection(): Promise<void> {
    return this.hubConnection.stop();
  }
}

export default new HubConnectionProvider();
