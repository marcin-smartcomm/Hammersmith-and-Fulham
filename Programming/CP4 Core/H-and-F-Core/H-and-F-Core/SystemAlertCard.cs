using System.Collections.Generic;

namespace H_and_F_Core
{
    public class SystemAlerts
    {
        public List<SystemAlertCard> cardList;
    }

    public class SystemAlertCard
    {
        public int alertID { get; set; }

        public string floor { get; set; }
        public string roomName { get; set; }
        public string issue { get; set; }

        public string date { get; set; }
        public string time { get; set; }

        public string ipAddress { get; set; }
    }

    public class SystemAlertRequest
    {
        public string createAlert { get; set; }
        public string floor { get; set; }
        public string roomName { get; set; }
        public string issue { get; set; }
    }
}
