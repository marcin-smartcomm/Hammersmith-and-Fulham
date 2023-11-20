using System.Collections.Generic;

namespace H_and_F_Core
{
    public class AssistanceCards
    {
        public List<AssistanceNeeddCard> cards;
    }

    public class AssistanceNeeddCard
    {
        public int requestID { get; set; }
        public string floor { get; set; }
        public string roomName { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string ipAddress { get; set; }

        public bool requestAcknowledged { get; set; } = false;
    }
}
