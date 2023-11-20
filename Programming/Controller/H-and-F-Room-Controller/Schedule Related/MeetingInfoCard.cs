using System.Collections.Generic;

namespace H_and_F_Room_Controller
{
    public class MeetingInfoCard
    {
        public string meetingSubject { get; set; }
        public string meetingOrganiser { get; set; }
        public string meetingDurationText { get; set; }
        public int startHour { get; set; }
        public int startMinute { get; set; }
        public int endHour { get; set; }
        public int endMinute { get; set; }
    }

    public class MeetingInfoCardCollection
    {
        public List<MeetingInfoCard> meetingInfoCards { get; set; }
    }
}
