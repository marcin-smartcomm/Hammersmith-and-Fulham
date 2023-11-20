using System;
using System.Collections.Generic;

namespace H_and_F_Room_Controller
{
    public class Bookings
    {
        public List<Value> value { get; set; }
    }

    public class Value
    {
        public string id { get; set; }
        public string subject { get; set; }
        public Start start { get; set; }
        public End end { get; set; }
        public Organizer organizer { get; set; }
    }
    public class Start
    {
        public DateTime dateTime { get; set; }
    }
    public class End
    {
        public DateTime dateTime { get; set; }
    }

    public class Organizer
    {
        public EmailAddress emailAddress { get; set; }
    }
    public class EmailAddress
    {
        public string name { get; set; }
        public string address { get; set; }
    }
}
