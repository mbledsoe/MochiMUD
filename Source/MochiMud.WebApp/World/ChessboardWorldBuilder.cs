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
            var piece = GetStartingPiece(file, rank);

            return piece is null
                ? $"This is a {squareColor} square."
                : $"A {piece} is on this {squareColor} square.";
        }

        private static IReadOnlyCollection<Exit> GetExits(char file, int rank)
        {
            var exits = new List<Exit>();

            AddExit(exits, "north", file, rank + 1);
            AddExit(exits, "south", file, rank - 1);
            AddExit(exits, "east", (char)(file + 1), rank);
            AddExit(exits, "west", (char)(file - 1), rank);

            return exits;
        }

        private static void AddExit(List<Exit> exits, string direction, char file, int rank)
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

        private static string? GetStartingPiece(char file, int rank)
        {
            return rank switch
            {
                1 => $"white {GetBackRankPiece(file)}",
                2 => "white pawn",
                7 => "black pawn",
                8 => $"black {GetBackRankPiece(file)}",
                _ => null
            };
        }

        private static string GetBackRankPiece(char file)
        {
            return file switch
            {
                'A' or 'H' => "rook",
                'B' or 'G' => "knight",
                'C' or 'F' => "bishop",
                'D' => "queen",
                'E' => "king",
                _ => throw new ArgumentOutOfRangeException(nameof(file), file, "Unknown chess file.")
            };
        }

        private static Guid GetRoomId(char file, int rank)
        {
            if (file == 'E' && rank == 1)
            {
                return WorldConstants.DefaultStartRoomId;
            }

            return new Guid(0x1f313c8a, 0x5f07, 0x4caa, 0x90, 0x00, 0x00, 0x00, 0x00, 0x00, (byte)file, (byte)rank);
        }
    }
}
