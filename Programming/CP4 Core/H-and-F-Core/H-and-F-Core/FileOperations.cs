using Crestron.SimplSharp;
using Newtonsoft.Json;
using System;
using System.IO;

namespace H_and_F_Core
{
    public static class FileOperations
    {
        static string _absolutePath;
        static JsonSerializerSettings _serializerSettings;

        public static void InitializeFileSystem()
        {
            if (CrestronEnvironment.DevicePlatform == eDevicePlatform.Appliance) _absolutePath = "/user/";
            else if (CrestronEnvironment.DevicePlatform == eDevicePlatform.Server) _absolutePath = "../user/";

            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        public static string loadJson(string configFile)
        {
            try { using (StreamReader sr = new StreamReader(_absolutePath + configFile + ".json")) return sr.ReadToEnd(); }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.loadJson\n" + ex.ToString());
                return string.Empty;
            }
        }

        public static bool saveFreeviewBoxes(FreeviewBoxes freeviewBoxes)
        {
            try
            {
                File.Delete(_absolutePath + "FreeviewBoxes.json");
                File.WriteAllText(
                    _absolutePath + "FreeviewBoxes.json",
                    JsonConvert.SerializeObject(freeviewBoxes, Formatting.Indented, _serializerSettings)
                    );
                return true;
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveFreeviewBoxes(): " + ex.ToString());
                return false;
            }
        }

        public static bool saveAssistanceCards(AssistanceCards assistanceCards)
        {
            try
            {
                File.Delete(_absolutePath + "AssistanceRequests.json");
                File.WriteAllText(
                    _absolutePath + "AssistanceRequests.json",
                    JsonConvert.SerializeObject(assistanceCards, Formatting.Indented, _serializerSettings)
                    );
                return true;
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveAssistanceCards(): " + ex.ToString());
                return false;
            }
        }

        public static bool savePanelInfo(PanelInfoList panelInfoList)
        {
            try
            {
                File.Delete(_absolutePath + "panelSettings.json");
                File.WriteAllText(
                    _absolutePath + "panelSettings.json",
                    JsonConvert.SerializeObject(panelInfoList, Formatting.Indented, _serializerSettings)
                    );
                return true;
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveAssistanceCards(): " + ex.ToString());
                return false;
            }
        }

        public static bool saveAlertCards(SystemAlerts assistanceCards)
        {
            try
            {
                File.Delete(_absolutePath + "SystemAlerts.json");
                File.WriteAllText(
                    _absolutePath + "SystemAlerts.json",
                    JsonConvert.SerializeObject(assistanceCards, Formatting.Indented, _serializerSettings)
                    );
                return true;
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveAlertCards(): " + ex.ToString());
                return false;
            }
        }

        public static bool saveNewPassword(SlaveiPadPass newPass)
        {
            try
            {
                File.Delete(_absolutePath + "SlaveiPadsPass.json");
                File.WriteAllText(
                    _absolutePath + "SlaveiPadsPass.json",
                    JsonConvert.SerializeObject(newPass, Formatting.Indented, _serializerSettings)
                    );
                return true;
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveNewPassword(): " + ex.ToString());
                return false;
            }
        }

        public static bool saveVideoReceivers(PortableAVVideoReceivers receivers)
        {
            try
            {
                File.Delete(_absolutePath + "PortableEquipment/VideoReceivers.json");
                File.WriteAllText(
                    _absolutePath + "PortableEquipment/VideoReceivers.json",
                    JsonConvert.SerializeObject(receivers, Formatting.Indented, _serializerSettings)
                    );
                return true;
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveVideoReceivers(): " + ex.ToString());
                return false;
            }
        }

        public static bool saveTVs(PortableTVs tvs)
        {
            try
            {
                File.Delete(_absolutePath + "PortableEquipment/TVs.json");
                File.WriteAllText(
                    _absolutePath + "PortableEquipment/TVs.json",
                    JsonConvert.SerializeObject(tvs, Formatting.Indented, _serializerSettings)
                    );
                return true;
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveTVs(): " + ex.ToString());
                return false;
            }
        }

        public static bool saveTransmitter(PortableTransmitter transmitter)
        {
            try
            {
                File.Delete(_absolutePath + "PortableEquipment/Transmitter.json");
                File.WriteAllText(
                    _absolutePath + "PortableEquipment/Transmitter.json",
                    JsonConvert.SerializeObject(transmitter, Formatting.Indented, _serializerSettings)
                    );
                return true;
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveTransmitter(): " + ex.ToString());
                return false;
            }
        }

        public static bool saveColabReceiverData(ColabVideoReceivers receivers)
        {
            try
            {
                File.Delete(_absolutePath + "ColabScreens/VideoReceivers.json");
                File.WriteAllText(
                    _absolutePath + "ColabScreens/VideoReceivers.json",
                    JsonConvert.SerializeObject(receivers, Formatting.Indented, _serializerSettings)
                    );
                return true;
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveColabReceiverData(): " + ex.ToString());
                return false;
            }
        }

        public static bool saveGlobalTemp(GlobalTemp globalTemp)
        {
            try
            {
                File.Delete(_absolutePath + "GlobalTemp.json");
                File.WriteAllText(
                    _absolutePath + "GlobalTemp.json",
                    JsonConvert.SerializeObject(globalTemp, Formatting.Indented, _serializerSettings)
                    );
                return true;
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveGlobalTemp(): " + ex.ToString());
                return false;
            }
        }

        public static bool saveSignageZonesInfo(DigitalSignageZones signageZones)
        {
            try
            {
                File.Delete(_absolutePath + "DigitalSignage/zoneAssignment.json");
                File.WriteAllText(
                    _absolutePath + "DigitalSignage/zoneAssignment.json",
                    JsonConvert.SerializeObject(signageZones, Formatting.Indented, _serializerSettings)
                    );
                return true;
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveGlobalTemp(): " + ex.ToString());
                return false;
            }
        }
    }
}
