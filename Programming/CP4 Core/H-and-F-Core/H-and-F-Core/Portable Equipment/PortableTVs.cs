using System.Collections.Generic;

namespace H_and_F_Core
{
    public class PortableTVs
    {
        public List<PortableTV> tvs;
    }
    public class PortableTV
    {
        public uint IPID { get; set; }
        public string TVName { get; set; }
        public string currentRoomServerAttached { get; set; } = string.Empty;
        public string currentRoomIDAttached { get; set; } = string.Empty;
    }
}
