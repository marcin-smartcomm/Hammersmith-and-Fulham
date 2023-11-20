namespace H_and_F_Room_Controller
{
    public class ClimateControlValues
    {
        public uint currentTemp_AV_ID { get; set; }
        public decimal currentTemp { get; set; }
        public uint spaceCO2_AV_ID { get; set; }
        public int spaceCO2 { get; set; }
        public uint setpoint_AV_ID { get; set; }
        public decimal setpoint { get; set; }
        public uint occupancy_MSV_ID { get; set; }
        public int occupancy { get; set; }
    }
}
