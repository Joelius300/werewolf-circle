import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { token } from '@/stores/tokenStore';

let hubConnectionSingleton: HubConnection;

function getHubConnection() {
  if (hubConnectionSingleton) return hubConnectionSingleton;

  function accessTokenFactory() {
    if (!token.value) throw new Error('No token registered')

    return token.value;
  }

  const url = process.env.VUE_APP_GAME_HUB_URL;
  if (!url) throw new Error('No VUE_APP_GAME_HUB_URL provided!');

  hubConnectionSingleton = new HubConnectionBuilder()
    .withUrl(url, { accessTokenFactory })
    .build();

  return hubConnectionSingleton;
}

async function ensureHubConnected() {
  const hubConnection = getHubConnection();

  if (hubConnection.state === HubConnectionState.Disconnected
      || hubConnection.state === HubConnectionState.Disconnecting) {
    await hubConnection.start();
  }

  return hubConnection;
}

export { getHubConnection, ensureHubConnected };
