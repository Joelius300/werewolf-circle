namespace WerewolfCircle.Utils
{
    public interface IRoomIdGenerator
    {
        /// <summary>
        /// Generates a random, cryptographically sound, url-safe id
        /// of arbitrary length which can be considered unique within this application.
        /// </summary>
        string GenerateRoomId();
    }
}
