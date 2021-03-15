import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { token } from '@/stores/tokenStore';

let hubConnectionSingleton: HubConnection;

function getHubConnection() {
  if (hubConnectionSingleton) return hubConnectionSingleton;

  function accessTokenFactory() {
    if (!token.value) throw new Error('No token registered')

    return token.value;
  }

  hubConnectionSingleton = new HubConnectionBuilder()
    .withUrl('http://localhost:5000/gameHub', { accessTokenFactory }) // process.env.BASE_URL TODO
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
