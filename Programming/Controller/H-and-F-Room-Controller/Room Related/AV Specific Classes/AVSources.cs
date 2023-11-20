using System.Collections.Generic;

namespace H_and_F_Room_Controller
{
    public class AVSources
    {
        public List<AVSource> sources;
    }

    public class AVSource
    {
        public string sourceName { get; set; }
        public string sourceType { get; set; }
        public string sourceStreamAddress { get; set; }
    }
}
