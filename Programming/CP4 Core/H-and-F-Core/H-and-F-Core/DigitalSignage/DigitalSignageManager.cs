using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace H_and_F_Core
{
    public class DigitalSignageManager
    {
        static Task performingFileOperation;

        public static async void Power(int zoneID, string action)
        {
            if (performingFileOperation != null)
                await performingFileOperation;

            performingFileOperation = Task.Run(() =>
            {
                DigitalSignageZones signageZones = JsonConvert.DeserializeObject<DigitalSignageZones>(FileOperations.loadJson("/DigitalSignage/zoneAssignment"));
                DigitalSignageBoxes signageBoxes = JsonConvert.DeserializeObject<DigitalSignageBoxes>(FileOperations.loadJson("/DigitalSignage/oneLanBoxes"));

                List<string> IPs = new List<string>();
                foreach (var zone in signageZones.zones)
                    if (zone.zoneID == zoneID)
                        for (int i = 0; i < signageBoxes.boxes.Count; i++)
                            for (int j = 0; j < zone.boxIDMembers.Length; j++)
                                if (signageBoxes.boxes[i].boxID == zone.boxIDMembers[j])
                                    IPs.Add(signageBoxes.boxes[i].boxIPAddress);

                Task.Run(() =>
                {
                    SignagePlayersHTTPPost(IPs, action);
                });
            });
        }

        static void SignagePlayersHTTPPost(List<string> IPs, string action)
        {
            foreach (var ip in IPs) { ConsoleLogger.WriteLine("Sending Command to: " + ip + " || Action: " + action); }
        }

        public static async void ScheduleTimeUp(int zoneID)
        {
            if(performingFileOperation != null)
                await performingFileOperation;

            performingFileOperation = Task.Run(() =>
            {
                try
                {
                    DigitalSignageZones signageZones = JsonConvert.DeserializeObject<DigitalSignageZones>(FileOperations.loadJson("/DigitalSignage/zoneAssignment"));

                    foreach(var zone in signageZones.zones)
                        if(zone.zoneID == zoneID)
                        {
                            zone.shutdownMinute += 15;
                            if(zone.shutdownMinute == 60)
                            {
                                zone.shutdownMinute = 0;
                                zone.shutdownHour += 1;
                                if(zone.shutdownHour == 24) zone.shutdownHour = 0;
                            }

                            zone.shutdownTime = GetTimeInStringFormat(zone.shutdownHour, zone.shutdownMinute);
                        }

                    FileOperations.saveSignageZonesInfo(signageZones);
                    SSE_Server.UpdateAllConnected("ScheduleTimes");
                }
                catch (Exception ex)
                {
                    ConsoleLogger.WriteLine("Exception in ScheduleTimeDown(): " + ex);
                }
            });
        }

        public static async void ScheduleTimeDown(int zoneID)
        {
            if (performingFileOperation != null)
                await performingFileOperation;

            performingFileOperation = Task.Run(() =>
            {
                try
                {
                    DigitalSignageZones signageZones = JsonConvert.DeserializeObject<DigitalSignageZones>(FileOperations.loadJson("/DigitalSignage/zoneAssignment"));

                    foreach (var zone in signageZones.zones)
                        if (zone.zoneID == zoneID)
                        {
                            zone.shutdownMinute -= 15;
                            if (zone.shutdownMinute == -15)
                            {
                                zone.shutdownMinute = 45;
                                zone.shutdownHour -= 1;
                                if (zone.shutdownHour == -1) zone.shutdownHour = 23;
                            }

                            zone.shutdownTime = GetTimeInStringFormat(zone.shutdownHour, zone.shutdownMinute);
                        }

                    FileOperations.saveSignageZonesInfo(signageZones);
                    SSE_Server.UpdateAllConnected("ScheduleTimes");
                }
                catch(Exception ex)
                {
                    ConsoleLogger.WriteLine("Exception in ScheduleTimeDown(): " + ex);
                }
            });
        }

        static string GetTimeInStringFormat(int hour, int minute)
        {
            string hourTxt, minuteTxt;

            if (hour < 10) hourTxt = "0" + hour;
            else hourTxt = hour.ToString();

            if (minute < 10) minuteTxt = "0" + minute;
            else minuteTxt = minute.ToString();

            return hourTxt + ":" + minuteTxt;
        }

        public static async void RunThroughScheduledOffTimes(int hour, int minute)
        {
            if (performingFileOperation != null)
                await performingFileOperation;

            performingFileOperation = Task.Run(() =>
            {
                DigitalSignageZones signageZones = JsonConvert.DeserializeObject<DigitalSignageZones>(FileOperations.loadJson("/DigitalSignage/zoneAssignment"));

                foreach (var zone in signageZones.zones)
                    if (zone.shutdownHour == hour && zone.shutdownMinute == minute)
                        Power(zone.zoneID, "Off");
            });
        }
    }
}