using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace H_and_F_Room_Controller
{
    public static class FileOperations
    {
        public static void saveRoomData(string roomID, RoomCoreInfo roomData)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../user/RoomSettings/Room" + roomID + "/";
            try
            {
                File.Delete(absolutePath + "Core.json");
                File.WriteAllText(
                    absolutePath + "Core.json",
                    JsonConvert.SerializeObject(roomData, Formatting.Indented, serializerSettings)
                    );
            }catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveRoomData(): " + ex.ToString());
            }
        }
        public static void saveRoomSources(string roomID, AVSources sources)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../user/RoomSettings/Room" + roomID + "/";
            try
            {
                File.Delete(absolutePath + "AVSources.json");
                File.WriteAllText(
                    absolutePath + "AVSources.json",
                    JsonConvert.SerializeObject(sources, Formatting.Indented, serializerSettings)
                    );
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveRoomData(): " + ex.ToString());
            }
        }

        public static void saveRoomBookings(int roomID, string settingType, Bookings bookingData)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../user/RoomSettings/Room" + roomID + "/";
            try
            {
                File.Delete(absolutePath + settingType + ".json");
                File.WriteAllText(
                    absolutePath + settingType + ".json",
                    JsonConvert.SerializeObject(bookingData, Formatting.Indented, serializerSettings)
                    );
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveRoomBookings(): " + ex.ToString());
            }
        }

        public static void saveRoomBookingStats(int roomID, CurrentAndNextMeetingInfo currentAndNextMeetingInfo)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../user/RoomSettings/Room" + roomID + "/";
            try
            {
                File.Delete(absolutePath + "BookingStats.json");
                File.WriteAllText(
                    absolutePath + "BookingStats.json",
                    JsonConvert.SerializeObject(currentAndNextMeetingInfo, Formatting.Indented, serializerSettings)
                    );
            }catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveRoomBookingStats(): " + ex.ToString());
            }
        }

        public static void saveMeetingDurations(int roomID, MeetingDurations meetingDurations)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../user/RoomSettings/Room" + roomID + "/";
            try
            {
                File.Delete(absolutePath + "MeetingDurations.json");
                File.WriteAllText(
                    absolutePath + "MeetingDurations.json",
                    JsonConvert.SerializeObject(meetingDurations, Formatting.Indented, serializerSettings)
                    );
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveMeetingDurations(): " + ex.ToString());
            }
        }

        public static void saveMeetingInfoCards(int roomID, MeetingInfoCardCollection meetingInfoCardsCollection)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;
                
            string absolutePath = @"../user/RoomSettings/Room" + roomID + "/";
            try
            {
                File.Delete(absolutePath + "MeetingInfoCards.json");
                File.WriteAllText(
                    absolutePath + "MeetingInfoCards.json",
                    JsonConvert.SerializeObject(meetingInfoCardsCollection, Formatting.Indented, serializerSettings)
                    );
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveMeetingInfoCards(): " + ex.ToString());
            }
        }

        public static void saveMasterRoomInfo(int roomID, GroupMasterRoom masterRoom)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../user/RoomSettings/Room" + roomID + "/";
            try
            {
                File.Delete(absolutePath + "MasterRoom.json");
                File.WriteAllText(
                    absolutePath + "MasterRoom.json",
                    JsonConvert.SerializeObject(masterRoom, Formatting.Indented, serializerSettings)
                    );
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveMasterRoomInfo(): " + ex.ToString());
            }
        }

        public static void saveRoomClimateValues(int roomID, ClimateControlValues climateValues)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../user/RoomSettings/Room" + roomID + "/";
            try
            {
                File.Delete(absolutePath + "ClimateControl.json");
                File.WriteAllText(
                    absolutePath + "ClimateControl.json",
                    JsonConvert.SerializeObject(climateValues, Formatting.Indented, serializerSettings)
                    );
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveRoomClimateValues(): " + ex.ToString());
            }
        }

        public static void saveGlobalTemp(GlobalTemp globalTemp)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../user/";
            try
            {
                File.Delete(absolutePath + "GlobalTemp.json");
                File.WriteAllText(
                    absolutePath + "GlobalTemp.json",
                    JsonConvert.SerializeObject(globalTemp, Formatting.Indented, serializerSettings)
                    );
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveGlobalTemp(): " + ex.ToString());
            }
        }

        public static string loadRoomJson(int roomID, string settingType)
        {
            try
            {
                string absolutePath = @"../user/RoomSettings/Room" + roomID + "/";
                StreamReader sr = new StreamReader(absolutePath + settingType + ".json");

                string json = sr.ReadToEnd();
                sr.Close();

                return json;
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.loadRoomJson\n" + ex.ToString());
                return "";
            }
        }

        public static string loadCoreInfo(string jsonFileName)
        {
            try
            {
                string absolutePath = @"../user/";
                StreamReader sr = new StreamReader(absolutePath + $"{jsonFileName}.json");

                string json = sr.ReadToEnd();
                sr.Close();

                return json;
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.loadMicrosoftInfo()\n" + ex.ToString());
                return null;
            }
        }

        public static List<string> GetRoomDirectories()
        {
            try
            {
                List<string> roomDirectories = Directory.GetDirectories(@"..\user\RoomSettings").ToList();
                return roomDirectories;
            }
            catch (Exception ex) { ConsoleLogger.WriteLine(ex.Message); return null; }
        }
    }
}
