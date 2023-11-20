namespace H_and_F_Room_Controller
{
    public enum TVControlType
    {
        rs232,
        cec,
        roomView
    }

    public enum TVCommand
    {
        PowerOn,
        PowerOff,
        HDMI1,
        HDMI2
    }

    public class TV
    {
        public uint ipid { get; set; }
        public TVControlType controlType { get; set; }
        public uint nvxRXConnected { get; set; }
        public string[] rs232Commands { get; set; }
    }
    public enum CameraControlType
    {
        http
    }

    public class Camera
    {
        public string name { get; set; }
        public string ip { get; set; }
        public CameraControlType controlType { get; set; }
    }

    public class RoomAVData
    {
        public uint[] NVXRxs { get; set; }
        public uint[] NVXTxs { get; set; }
        public string BiampIP { get; set; }
        public int BiampPort { get; set; }
        public TV[] TVs { get; set; }
        public Camera[] Cameras { get; set; }
    }
}
