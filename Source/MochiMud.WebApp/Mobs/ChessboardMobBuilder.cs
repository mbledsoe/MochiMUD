using MochiMud.WebApp.Combat;
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
                var backRankPiece = GetBackRankPiece(file);

                mobs.Add(CreateMob($"White {backRankPiece}", file, 1));
                mobs.Add(CreateMob("White Pawn", file, 2));
                mobs.Add(CreateMob("Black Pawn", file, 7));
                mobs.Add(CreateMob($"Black {backRankPiece}", file, 8));
            }

            return mobs;
        }

        private static Mob CreateMob(string name, char file, int rank)
        {
            var mob = new Mob(name, ChessboardWorldBuilder.GetRoomId(file, rank));

            if (name.EndsWith("King", StringComparison.Ordinal))
            {
                mob.BareHandDamage = new DamageDiceSpecification(5, 5);
            }
            else if (name.EndsWith("Queen", StringComparison.Ordinal))
            {
                mob.BareHandDamage = new DamageDiceSpecification(5, 50);
            }

            return mob;
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
