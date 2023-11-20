using System.Collections.Generic;

namespace H_and_F_Core
{
    public class PortableAVVideoReceivers
    {
        public List<PortableAVVideoReceiver> receivers;
    }
    public class PortableAVVideoReceiver
    {
        public uint IPID { get; set; }
        public string receiverName { get; set; }
        public string currentRoomServerAttached { get; set; } = string.Empty;
        public string currentRoomIDAttached { get; set; } = string.Empty;
    }

}