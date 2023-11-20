using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H_and_F_Room_Controller
{
    internal class NewMeetingInfo
    {
        public string organiser { get; set; }
        public string startTime { get; set; }

        public List<string> endTimes { get; set; }
    }
}
