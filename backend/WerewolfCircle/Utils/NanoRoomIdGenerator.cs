using System;

namespace WerewolfCircle.Utils
{
    // NanoIdGenerator might be overkill for this but why not :)
    public class NanoRoomIdGenerator : IRoomIdGenerator
    {
        private const int DefaultRoomIdLength = 6;

        public int RoomIdLength { get; }

        public NanoRoomIdGenerator(int roomIdLength = DefaultRoomIdLength)
        {
            if (roomIdLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(roomIdLength), "The roomId length has to be positive.");

            RoomIdLength = roomIdLength;
        }

        public string GenerateRoomId() => NanoIdGenerator.Generate(length: RoomIdLength);
    }
}
