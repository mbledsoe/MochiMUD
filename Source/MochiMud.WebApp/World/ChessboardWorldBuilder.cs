namespace MochiMud.WebApp.World
{
    public static class ChessboardWorldBuilder
    {
        private const string Files = "ABCDEFGH";

        public static IReadOnlyDictionary<Guid, Room> CreateRooms()
        {
            var rooms = new Dictionary<Guid, Room>();

            foreach (var file in Files)
            {
                for (var rank = 1; rank <= 8; rank++)
                {
                    var room = CreateRoom(file, rank);
                    rooms.Add(room.Id, room);
                }
            }

            return rooms;
        }

        private static Room CreateRoom(char file, int rank)
        {
            var roomId = GetRoomId(file, rank);
            var squareName = $"{file}{rank}";

            return new Room(
                roomId,
                $"Square {squareName}",
                GetDescription(file, rank),
                GetExits(file, rank));
        }

        private static string GetDescription(char file, int rank)
        {
            var squareColor = GetSquareColor(file, rank);

            return $"This is a {squareColor} square.";
        }

        private static IReadOnlyCollection<Exit> GetExits(char file, int rank)
        {
            var exits = new List<Exit>();

            AddExit(exits, Direction.North, file, rank + 1);
            AddExit(exits, Direction.South, file, rank - 1);
            AddExit(exits, Direction.East, (char)(file + 1), rank);
            AddExit(exits, Direction.West, (char)(file - 1), rank);

            return exits;
        }

        private static void AddExit(List<Exit> exits, Direction direction, char file, int rank)
        {
            if (!IsValidSquare(file, rank))
            {
                return;
            }

            exits.Add(new Exit(direction, GetRoomId(file, rank)));
        }

        private static bool IsValidSquare(char file, int rank)
        {
            return Files.Contains(file) && rank is >= 1 and <= 8;
        }

        private static string GetSquareColor(char file, int rank)
        {
            var fileIndex = Files.IndexOf(file);

            return (fileIndex + rank) % 2 == 0
                ? "light"
                : "dark";
        }

        public static Guid GetRoomId(char file, int rank)
        {
            if (file == 'E' && rank == 1)
            {
                return WorldConstants.DefaultStartRoomId;
            }

            return new Guid(0x1f313c8a, 0x5f07, 0x4caa, 0x90, 0x00, 0x00, 0x00, 0x00, 0x00, (byte)file, (byte)rank);
        }
    }
}
