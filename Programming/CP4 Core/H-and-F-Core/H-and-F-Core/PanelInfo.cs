using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H_and_F_Core
{
    public class PanelInfoList
    {
        public List<PanelInfo> panels;
    }

    public class PanelInfo
    {
        public string roomName { get; set; }
        public int slaveID { get; set; }
        public string panelIP { get; set; }
        public string panelType { get; set; }
        public string serverIP { get; set; }
        public int roomID { get; set; }
        public string lightingServerIP { get; set; }
    }
}