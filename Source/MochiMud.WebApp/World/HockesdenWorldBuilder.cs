namespace MochiMud.WebApp.World
{
    public static class HockesdenWorldBuilder
    {
        public static readonly Guid TownSquareRoomId = Guid.Parse("6b57b778-87c7-4f24-9f53-645257db4b68");

        private static readonly Guid WestMainStreetRoomId = Guid.Parse("f3519f3f-3cb4-4e18-967b-221936c4d128");
        private static readonly Guid EastMainStreetRoomId = Guid.Parse("6a2dca48-2730-4947-bc01-b52a25711f95");
        private static readonly Guid WarriorGuildRoomId = Guid.Parse("bba8a148-370f-4d28-9973-c9c9ec5c9257");
        private static readonly Guid ClericGuildRoomId = Guid.Parse("7f948de0-1a9d-4778-bab7-9f045311cead");
        private static readonly Guid MageGuildRoomId = Guid.Parse("4f9b48c9-3c5f-4de2-b13c-0ad4cf2ad066");
        private static readonly Guid ThievesGuildRoomId = Guid.Parse("65cfac21-09f4-45f3-81d0-c4121b54258c");
        private static readonly Guid TempleStepsRoomId = Guid.Parse("1aca0620-7565-4cec-98bd-d85bf8ea0299");
        private static readonly Guid WhiteSpireTempleRoomId = Guid.Parse("fd71fda6-cc24-4965-a383-cdbf5340fb17");
        public static readonly Guid TempleAnteroomRoomId = Guid.Parse("df8ff9a2-730d-47f4-8329-2c11e9d2e176");
        private static readonly Guid PortalRoomId = Guid.Parse("7db66507-6adf-476b-a2fe-3bed97789dd8");
        private static readonly Guid ChessboardPortalRoomId = Guid.Parse("dc5b52e7-ccf3-4cf1-bb70-a2d653b1f25b");

        public static IReadOnlyDictionary<Guid, Room> CreateRooms()
        {
            var rooms = new[]
            {
                CreateRoom(
                    TownSquareRoomId,
                    "Hockesden Town Square",
                    "The center of Hockesden is a broad town square where Main Street runs east and west. Main Street leads away in both directions, and temple steps rise to the north.",
                    Exit(Direction.West, WestMainStreetRoomId),
                    Exit(Direction.East, EastMainStreetRoomId),
                    Exit(Direction.North, TempleStepsRoomId)),
                CreateRoom(
                    WestMainStreetRoomId,
                    "West Main Street",
                    "Main Street continues west past the sturdy halls of Hockesden's martial and sacred guilds. The town square lies back to the east, with guild doors opening to the north and south.",
                    Exit(Direction.East, TownSquareRoomId),
                    Exit(Direction.North, WarriorGuildRoomId),
                    Exit(Direction.South, ClericGuildRoomId)),
                CreateRoom(
                    EastMainStreetRoomId,
                    "East Main Street",
                    "Main Street continues east past narrow guild halls marked by arcane signs and shadowed doorways. The town square lies back to the west, with guild doors opening to the north and south.",
                    Exit(Direction.West, TownSquareRoomId),
                    Exit(Direction.North, MageGuildRoomId),
                    Exit(Direction.South, ThievesGuildRoomId)),
                CreateRoom(
                    WarriorGuildRoomId,
                    "Warrior Guild",
                    "Weapon racks and scarred practice posts fill the Warrior Guild. The door back to West Main Street stands to the south.",
                    Exit(Direction.South, WestMainStreetRoomId)),
                CreateRoom(
                    ClericGuildRoomId,
                    "Cleric Guild",
                    "Quiet benches and polished icons line the Cleric Guild. The door back to West Main Street stands to the north.",
                    Exit(Direction.North, WestMainStreetRoomId)),
                CreateRoom(
                    MageGuildRoomId,
                    "Mage Guild",
                    "Shelves of scrolls and faintly glowing lamps crowd the Mage Guild. The door back to East Main Street stands to the south.",
                    Exit(Direction.South, EastMainStreetRoomId)),
                CreateRoom(
                    ThievesGuildRoomId,
                    "Thieves Guild",
                    "The Thieves Guild is dim, narrow, and full of carefully watched corners. The door back to East Main Street stands to the north.",
                    Exit(Direction.North, EastMainStreetRoomId)),
                CreateRoom(
                    TempleStepsRoomId,
                    "Temple Steps",
                    "Stone steps climb north from the square toward a temple crowned by a white spire. The temple waits above, while the town square spreads out below.",
                    Exit(Direction.South, TownSquareRoomId),
                    Exit(Direction.North, WhiteSpireTempleRoomId)),
                CreateRoom(
                    WhiteSpireTempleRoomId,
                    "Temple of the White Spire",
                    "The temple is cool and bright beneath the white spire rising overhead. The steps descend behind you, and an anteroom waits deeper within the temple.",
                    Exit(Direction.South, TempleStepsRoomId),
                    Exit(Direction.North, TempleAnteroomRoomId)),
                CreateRoom(
                    TempleAnteroomRoomId,
                    "Temple Anteroom",
                    "A small anteroom waits beyond the temple nave, quiet and whitewashed. The nave lies back to the south, and a side passage opens toward the portal gallery.",
                    Exit(Direction.South, WhiteSpireTempleRoomId),
                    Exit(Direction.West, PortalRoomId)),
                CreateRoom(
                    PortalRoomId,
                    "Portal Gallery",
                    "Ancient portals shimmer along the walls, each opening onto a different world. One portal glows to the north, while the anteroom remains safely to the east.",
                    Exit(Direction.East, TempleAnteroomRoomId),
                    Exit(Direction.North, ChessboardPortalRoomId)),
                CreateRoom(
                    ChessboardPortalRoomId,
                    "The Chessboard Portal",
                    "A massive chessboard stretches beyond a large floor to ceiling window. The image shimmers, waves, and flickers like a reflection on disturbed water. The portal gallery waits behind you, and the chessboard world presses close beyond the glass.",
                    [
                        new RoomObject(
                            "plaque",
                            "A bronze plaque hangs on the wall beside the window.",
                            "Beyond this portal lies the Chessboard, a realm where kings and pawns wage endless war across black and white stone.  Beware not to disturb the peace of the King and Queen.")
                    ],
                    Exit(Direction.South, PortalRoomId),
                    Exit(Direction.North, ChessboardWorldBuilder.GetRoomId('E', 4))),
            };

            return rooms.ToDictionary(room => room.Id);
        }

        private static Room CreateRoom(Guid id, string title, string description, params Exit[] exits)
        {
            return new Room(id, title, description, exits);
        }

        private static Room CreateRoom(
            Guid id,
            string title,
            string description,
            IReadOnlyCollection<RoomObject> objects,
            params Exit[] exits)
        {
            return new Room(id, title, description, exits, objects);
        }

        private static Exit Exit(Direction direction, Guid destinationRoomId)
        {
            return new Exit(direction, destinationRoomId);
        }
    }
}
