using Crestron.SimplSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace H_and_F_Room_Controller
{
    public static class FileOperations
    {
        static string _GlobalPath, _RoomSettingsPath, _roomDirectoryPath;
        static JsonSerializerSettings _serializerSettings;

        public static void InitializeFileSystem()
        {
            if (CrestronEnvironment.DevicePlatform == eDevicePlatform.Appliance)
            {
                _RoomSettingsPath = "/user/RoomSettings";
                _roomDirectoryPath = @"\user\RoomSettings";
                _GlobalPath = "/user/";
            }
            else if (CrestronEnvironment.DevicePlatform == eDevicePlatform.Server)
            {
                _RoomSettingsPath = "../user/RoomSettings";
                _roomDirectoryPath = @"..\user\RoomSettings";
                _RoomSettingsPath = "../user/";
            }

            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        public static void saveRoomData(string roomID, RoomCoreInfo roomData)
        {
            string filePath = $@"{_RoomSettingsPath}/Room{roomID}/Core.json";
            try
            {
                File.Delete(filePath);
                File.WriteAllText(
                    filePath,
                    JsonConvert.SerializeObject(roomData, Formatting.Indented, _serializerSettings)
                    );
            }catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveRoomData(): " + ex.ToString());
            }
        }
        public static void saveRoomSources(string roomID, AVSources sources)
        {
            string filePath = $@"{_RoomSettingsPath}/Room{roomID}/AVSources.json";
            try
            {
                File.Delete(filePath);
                File.WriteAllText(
                    filePath,
                    JsonConvert.SerializeObject(sources, Formatting.Indented, _serializerSettings)
                    );
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveRoomData(): " + ex.ToString());
            }
        }

        public static void saveRoomBookings(int roomID, string settingType, Bookings bookingData)
        {
            string filePath = $@"{_RoomSettingsPath}/Room{roomID}/{settingType}.json";
            try
            {
                File.Delete(filePath);
                File.WriteAllText(
                    filePath,
                    JsonConvert.SerializeObject(bookingData, Formatting.Indented, _serializerSettings)
                    );
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveRoomBookings(): " + ex.ToString());
            }
        }

        public static void saveRoomBookingStats(int roomID, CurrentAndNextMeetingInfo currentAndNextMeetingInfo)
        {
            string filePath = $@"{_RoomSettingsPath}/Room{roomID}/BookingStats.json";
            try
            {
                File.Delete(filePath);
                File.WriteAllText(
                    filePath,
                    JsonConvert.SerializeObject(currentAndNextMeetingInfo, Formatting.Indented, _serializerSettings)
                    );
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveRoomBookingStats(): " + ex.ToString());
            }
        }

        public static void saveMeetingDurations(int roomID, MeetingDurations meetingDurations)
        {
            string filePath = $@"{_RoomSettingsPath}/Room{roomID}/MeetingDurations.json";
            try
            {
                File.Delete(filePath);
                File.WriteAllText(
                    filePath,
                    JsonConvert.SerializeObject(meetingDurations, Formatting.Indented, _serializerSettings)
                    );
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveMeetingDurations(): " + ex.ToString());
            }
        }

        public static void saveMeetingInfoCards(int roomID, MeetingInfoCardCollection meetingInfoCardsCollection)
        {
            string filePath = $@"{_RoomSettingsPath}/Room{roomID}/MeetingInfoCards.json";
            try
            {
                File.Delete(filePath);
                File.WriteAllText(
                    filePath,
                    JsonConvert.SerializeObject(meetingInfoCardsCollection, Formatting.Indented, _serializerSettings)
                    );
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveMeetingInfoCards(): " + ex.ToString());
            }
        }

        public static void saveMasterRoomInfo(int roomID, GroupMasterRoom masterRoom)
        {
            string filePath = $@"{_RoomSettingsPath}/Room{roomID}/MasterRoom.json";
            try
            {
                File.Delete(filePath);
                File.WriteAllText(
                    filePath,
                    JsonConvert.SerializeObject(masterRoom, Formatting.Indented, _serializerSettings)
                    );
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveMasterRoomInfo(): " + ex.ToString());
            }
        }

        public static void saveRoomClimateValues(int roomID, ClimateControlValues climateValues)
        {
            string filePath = $@"{_RoomSettingsPath}/Room{roomID}/ClimateControl.json";
            try
            {
                File.Delete(filePath);
                File.WriteAllText(
                    filePath,
                    JsonConvert.SerializeObject(climateValues, Formatting.Indented, _serializerSettings)
                    );
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveRoomClimateValues(): " + ex.ToString());
            }
        }

        public static void saveGlobalTemp(GlobalTemp globalTemp)
        {
            try
            {
                File.Delete(_GlobalPath + "GlobalTemp.json");
                File.WriteAllText(
                    _GlobalPath + "GlobalTemp.json",
                    JsonConvert.SerializeObject(globalTemp, Formatting.Indented, _serializerSettings)
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
                string filePath = $@"{_RoomSettingsPath}/Room{roomID}/";
                using (StreamReader sr = new StreamReader(filePath + settingType + ".json")) return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.loadRoomJson\n" + ex.ToString());
                return "";
            }
        }

        public static string loadCoreInfo(string jsonFileName)
        {
            try { using (StreamReader sr = new StreamReader(_GlobalPath + $"{jsonFileName}.json")) return sr.ReadToEnd(); }
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
                List<string> roomDirectories = Directory.GetDirectories($@"{_roomDirectoryPath}").ToList();
                return roomDirectories;
            }
            catch (Exception ex) { ConsoleLogger.WriteLine(ex.Message); return null; }
        }
    }
}
