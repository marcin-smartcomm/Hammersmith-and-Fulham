using System.Collections.Generic;

namespace H_and_F_Core
{
    public  class FreeviewBoxes
    {
        public List<FreeviewBox> boxes;
    }

    public class FreeviewBox
    {
        public string boxName { get; set; }
        public string nvxStreamAddress { get; set; }
        public int cp4IRPortNum { get; set; }
        public string icons { get; set; }
        public string type { get; set; }
    }
}