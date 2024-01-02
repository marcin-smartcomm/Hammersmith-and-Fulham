using System.Collections.Generic;

namespace H_and_F_Room_Controller
{
    internal class NewMeetingInfo
    {
        public string organiser { get; set; }
        public string startTime { get; set; }

        public List<string> endTimes { get; set; }
    }
}
