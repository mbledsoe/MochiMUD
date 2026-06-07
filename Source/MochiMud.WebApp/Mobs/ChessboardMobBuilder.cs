using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Mobs
{
    public static class ChessboardMobBuilder
    {
        private const string Files = "ABCDEFGH";

        public static IReadOnlyCollection<Mob> CreateMobs()
        {
            var mobs = new List<Mob>();

            foreach (var file in Files)
            {
                mobs.Add(CreateMob($"White {GetBackRankPiece(file)}", file, 1));
                mobs.Add(CreateMob("White Pawn", file, 2));
                mobs.Add(CreateMob("Black Pawn", file, 7));
                mobs.Add(CreateMob($"Black {GetBackRankPiece(file)}", file, 8));
            }

            return mobs;
        }

        private static Mob CreateMob(string name, char file, int rank)
        {
            return new Mob(name, ChessboardWorldBuilder.GetRoomId(file, rank));
        }

        private static string GetBackRankPiece(char file)
        {
            return file switch
            {
                'A' or 'H' => "Rook",
                'B' or 'G' => "Knight",
                'C' or 'F' => "Bishop",
                'D' => "Queen",
                'E' => "King",
                _ => throw new ArgumentOutOfRangeException(nameof(file), file, "Unknown chess file.")
            };
        }
    }
}
