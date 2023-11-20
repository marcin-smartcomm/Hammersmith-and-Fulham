using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace H_and_F_Core
{
    public class GroupedRooms
    {
        public List<GroupedRoom> rooms;
    }

    public class GroupedRoom
    {
        public string name { get; set; }
        public string ipAddress { get; set; }
        public string roomID { get; set; }
        public bool isGroupabloe { get; set; }
    }

    public class RoomGroupingResponse
    {
        public string roomName { get; set;}
        public string groupStatus { get; set;}
    }

    public class RoomGroupingResult
    {
        public string result { get; set;}
    }

    public class GroupManager
    {
        ControlSystem _cs;
        GroupedRoom _masterRoom;
        GroupedRooms _slaveRooms;

        public GroupManager(ControlSystem cs)
        {
            _cs = cs;

            _masterRoom = new GroupedRoom();
            _slaveRooms = new GroupedRooms();
            _slaveRooms.rooms = new List<GroupedRoom>();
        }

        public void CreateNewRoomGroup(GroupedRooms rooms)
        {
            Task.Run(() =>
            {
                InformSlavesOfGroupBreakUp();

                _masterRoom = rooms.rooms[0];
                rooms.rooms.RemoveAt(0);
                _slaveRooms = rooms;

                InformSlavesOfGroupAssignment();
            });
        }

        public void BreakRoomFromGroup(string roomID, string ipAdderss)
        {
            foreach (var room in _slaveRooms.rooms)
                if (room.ipAddress == ipAdderss && room.roomID == roomID)
                {
                    _slaveRooms.rooms.Remove(room);
                    break;
                }
        }

        public void SourceChanged(string serverIP, string roomID, AVSource newSource)
        {
            if(_masterRoom.ipAddress == serverIP && _masterRoom.roomID == roomID)
            {
                Task.Run(() =>
                {
                    foreach(var room in _slaveRooms.rooms)
                    {
                        try
                        {
                            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + room.ipAddress + ":50000/api/MasterSourceChanged?" + room.roomID);
                            httpWebRequest.ContentType = "application/json";
                            httpWebRequest.Method = "POST";
                            httpWebRequest.Timeout = 5000;

                            ConsoleLogger.WriteLine("------------------- Sending New Master Source To: " + room.ipAddress + " (" + room.name + ")" + " - RoomID: " + room.roomID + "----------------------");
                            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                            {
                                streamWriter.Write(JsonConvert.SerializeObject(newSource));
                            }
                            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                var result = streamReader.ReadToEnd();
                                ConsoleLogger.WriteLine("Received Response from " + room.ipAddress + ": " + result.ToString());
                            }
                        }
                        catch (WebException ex)
                        {
                            ConsoleLogger.WriteLine("WebException Caught: " + ex.Message);
                        }
                    }
                });
            }
        }

        void InformSlavesOfGroupBreakUp()
        {
            try
            {
                foreach (var room in _slaveRooms.rooms)
                {
                    try
                    {
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + room.ipAddress + ":50000/api/GroupBreakUp?" + room.roomID);
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "GET";
                        httpWebRequest.Timeout = 5000;

                        ConsoleLogger.WriteLine("------------------- Sending Group Break Request To: " + room.ipAddress + " (" + room.name + ")" + " - RoomID: " + room.roomID + "----------------------");
                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            ConsoleLogger.WriteLine("Received Response from " + room.ipAddress + ": " + result.ToString());
                        }
                    }
                    catch (WebException ex)
                    {
                        ConsoleLogger.WriteLine("WebException Caught: " + ex.Message);
                    }
                }
            }catch(Exception ex)
            {
                ConsoleLogger.WriteLine("Exception Caught: " + ex.Message);
            }
        }

        void InformSlavesOfGroupAssignment()
        {
            List<RoomGroupingResponse> roomGroupingResponse = new List<RoomGroupingResponse>();
            List<GroupedRoom> noAnswerRooms = new List<GroupedRoom>();

            foreach (var room in _slaveRooms.rooms)
            {
                try
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + room.ipAddress + ":50000/api/GroupMasterDetails?" + room.roomID);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    httpWebRequest.Timeout = 5000;

                    ConsoleLogger.WriteLine("------------------- Sending Grouping Request To: " + room.ipAddress + " (" + room.name + ")" + " - RoomID: " + room.roomID + "----------------------");
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(_masterRoom));
                    }
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        ConsoleLogger.WriteLine("Received Response from " + room.ipAddress + ": " + result.ToString());

                        RoomGroupingResult rgr = JsonConvert.DeserializeObject<RoomGroupingResult>(result);
                        roomGroupingResponse.Add(new RoomGroupingResponse() { roomName = room.name, groupStatus = rgr.result });
                    }
                }
                catch (WebException ex)
                {
                    ConsoleLogger.WriteLine("WebException Caught: " + ex.Message);
                    roomGroupingResponse.Add(new RoomGroupingResponse() { roomName = room.name, groupStatus = ex.Message });
                    noAnswerRooms.Add(room);
                }
            }

            foreach(var room in noAnswerRooms)
                _slaveRooms.rooms.Remove(room);

            SSE_Server.UpdateAllConnected("GroupingResults" + JsonConvert.SerializeObject(roomGroupingResponse));
        }
    }
}