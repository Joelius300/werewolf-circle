import hubConnectionProvider from './HubConnectionProvider';

/* eslint-disable class-methods-use-this */

// with minor downsides (and some upsides?), this could also be done with something like
// https://github.com/bterlson/strict-event-emitter-types
export default class GameService {
  /* eslint-disable-next-line @typescript-eslint/no-explicit-any */
  private callbacks: { [eventName: string]: (...args: any[]) => void } = {};

  // reexposed for convenience, HubConnectionProvider should only be used to stop the connection
  public ensureConnected(): Promise<void> {
    return hubConnectionProvider.ensureConnected();
  }

  /* eslint-disable-next-line @typescript-eslint/no-explicit-any */
  private addCallback(name: string, func: (...args: any[]) => void) {
    if (name == null) throw new Error('The event name cannot be null or undefined.');

    if (func == null) throw new Error('The callback cannot be null or undefined.');

    if (this.callbacks[name] != null) throw new Error(`Cannot register multiple events for '${name}' within this instance.`);

    this.callbacks[name] = func;
    hubConnectionProvider.connection.on(name, func);
  }

  /**
   * Removes all listeners/callbacks without interrupting the connection.
   * Doesn't need to be called if you don't add any listeners with the 'onX'
   * methods.
   */
  public stopListening(): void {
    const eventNames = Object.keys(this.callbacks);
    for (let i = 0; i < eventNames.length; i++) {
      const eventName = eventNames[i];
      hubConnectionProvider.connection.off(eventName, this.callbacks[eventName]);
      delete this.callbacks[eventName];
    }
  }

  public onPlayerJoined(callback: (name: string) => void): void {
    this.addCallback('PlayerJoined', callback);
  }

  public onPlayerLeft(callback: (name: string) => void): void {
    this.addCallback('PlayerLeft', callback);
  }

  public createGame(): Promise<string> {
    return hubConnectionProvider.connection.invoke('CreateGame');
  }

  public joinGame(roomId: string, playerName: string): Promise<string[]> {
    return hubConnectionProvider.connection.invoke('JoinGame', roomId, playerName);
  }

  public leaveGame(): Promise<void> {
    return hubConnectionProvider.connection.invoke('LeaveGame');
  }
}
