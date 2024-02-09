using System.Collections.Generic;

namespace H_and_F_Lighting
{
    public class Area
    {
        public string currentScene { get; set; }
        public ushort AreaNum { get; set; }
        public uint AreaNumAnalogJoin { get; set; }
        public uint AreaPresetsDigitalJoinStart { get; set; }
    }

    public class AreaList
    {
        public List<Area> areaList { get; set; }
    }
}
