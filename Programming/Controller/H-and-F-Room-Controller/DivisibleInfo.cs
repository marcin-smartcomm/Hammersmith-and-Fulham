using System.Collections.Generic;

namespace H_and_F_Room_Controller
{
    public class DivisionScenario
    {
        public int masterRoomID { get; set; } = -1;
        public int[] slaveRoomIDs { get; set; }
        public int[] roomIDsNotInPlay { get; set; }
    }

    public class DivisibleInfo
    {
        public bool hasDivisibleRooms { get; set; }
        public int numberOfWalls { get; set; }
        public int[] actors { get; set; }
        public List<DivisionScenario> divisionScenarios { get; set; }
    }
}
