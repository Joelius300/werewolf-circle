namespace WerewolfCircle.Auth
{
    /// <summary>
    /// Generates auth-tokens for this application.
    /// </summary>
    public interface IAuthTokenGenerator
    {
        /// <summary>
        /// Generates an auth-token which authorizes and identifies
        /// the admin of the game with the provided <paramref name="roomId"/>.
        /// </summary>
        /// <param name="roomId">The game's roomId.</param>
        string GenerateAdminToken(string roomId);

        /// <summary>
        /// Generates an auth-token which authorizes and identifies
        /// a player of the game with the provided <paramref name="roomId"/>.
        /// The <paramref name="playerName"/> won't ever change
        /// and must be incorporated in the token. The token must not
        /// contain information that could change (e.g. game-role).
        /// </summary>
        /// <param name="roomId">The game's roomId.</param>
        /// <param name="playerName">The (definite) name of the player for this game.</param>
        string GeneratePlayerToken(string roomId, string playerName);
    }
}
