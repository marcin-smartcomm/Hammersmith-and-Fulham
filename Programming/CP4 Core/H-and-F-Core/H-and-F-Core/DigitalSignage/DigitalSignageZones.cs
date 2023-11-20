using System.Collections.Generic;

namespace H_and_F_Core
{
    public class DigitalSignageZone
    {
        public int zoneID { get; set; }
        public string zoneName { get; set; }
        public string shutdownTime { get; set; }
        public int shutdownHour { get; set; }
        public int shutdownMinute { get; set; }
        public int[] boxIDMembers { get; set; }
    }

    public class DigitalSignageZones
    {
        public List<DigitalSignageZone> zones;
    }
}
