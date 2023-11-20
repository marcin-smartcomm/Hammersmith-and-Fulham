using System.Collections.Generic;

namespace H_and_F_Room_Controller
{
    public class RoomMenuItem
    {
        public string menuItemName { get; set; }
        public string menuItemIcon { get; set; }
        public string menuItemPageAssigned { get; set; }
    }

    public class RoomCoreInfo
    {
        public int roomID { get; set; }
        public int floor { get; set; }
        public string roomName { get; set; }
        public string emailAddress { get; set; } 

        public bool isGrouped { get; set; }

        public string sourceSelected { get; set; }

        public bool signageAccess { get; set; }

        public List<RoomMenuItem> menuItems { get; set; }
    }
}
