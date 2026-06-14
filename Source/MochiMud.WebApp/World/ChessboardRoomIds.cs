namespace MochiMud.WebApp.World
{
    public static class ChessboardRoomIds
    {
        public static Guid GetRoomId(char file, int rank)
        {
            return new Guid(0x1f313c8a, 0x5f07, 0x4caa, 0x90, 0x00, 0x00, 0x00, 0x00, 0x00, (byte)file, (byte)rank);
        }
    }
}
