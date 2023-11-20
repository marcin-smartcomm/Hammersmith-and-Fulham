using Crestron.SimplSharp.CrestronData;

namespace H_and_F_Room_Controller
{
    public class CurrentAndNextMeetingInfo
    {
        public string dateNow { get; set; } = "";
        public bool freeAllDay { get; set; } = false;
        public bool inMeeting { get; set; } = false;

        public string currentMeetingID { get; set; }
        public int currentHoursRemaining { get; set; } = 0;
        public int currentMinutesRemaining { get; set; } = 0;

        public string currentMeetingStartEndTime { get; set; } = "";
        public string currentMeetingOrganiser { get; set; } = "";
        public string currentMeetingSubject { get; set; } = "";


        public int hoursUntilNextMeeting { get; set; } = 0;
        public int minutesUntilNextMeeting { get; set; } = 0;
        public string nextMeetingStartEndTime { get; set; } = "";
        public string nextMeetingOrganiser { get; set; } = "";
        public string nextMeetingSubject { get; set; } = "";
    }
}
