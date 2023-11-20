namespace H_and_F_Core
{
    public class PortableTransmitter
    {
        public uint IPID { get; set; }
        public string transmitterName { get; set; }
        public string transmitterType { get; set; }
        public string currentRoomServerAttached { get; set; } = string.Empty;
        public string currentRoomIDAttached { get; set; } = string.Empty;
        public string transmitterStreamAddress { get; set; }
    }
}
