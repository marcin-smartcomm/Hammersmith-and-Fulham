using System.Collections.Generic;

namespace H_and_F_Core
{
    public class DigitalSignageBox
    {
        public int boxID { get; set; }
        public string boxIPAddress { get; set; }
    }

    public class DigitalSignageBoxes
    {
        public List<DigitalSignageBox> boxes;
    }
}
