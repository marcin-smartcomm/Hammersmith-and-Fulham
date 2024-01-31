using Newtonsoft.Json;
using System;
using System.IO;

namespace H_and_F_Core
{
    public static class FileOperations
    {
        public static string loadJson(string configFile)
        {
            try
            {
                ConsoleLogger.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory);
                string absolutePath = @"../user/";
                StreamReader sr = new StreamReader(absolutePath + configFile + ".json");

                string json = sr.ReadToEnd();
                sr.Close();

                return json;
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.loadRoomJson\n" + ex.ToString());
                return string.Empty;
            }
        }

        public static bool saveFreeviewBoxes(FreeviewBoxes freeviewBoxes)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../user/";
            try
            {
                File.Delete(absolutePath + "FreeviewBoxes.json");
                File.WriteAllText(
                    absolutePath + "FreeviewBoxes.json",
                    JsonConvert.SerializeObject(freeviewBoxes, Formatting.Indented, serializerSettings)
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
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../user/";
            try
            {
                File.Delete(absolutePath + "AssistanceRequests.json");
                File.WriteAllText(
                    absolutePath + "AssistanceRequests.json",
                    JsonConvert.SerializeObject(assistanceCards, Formatting.Indented, serializerSettings)
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
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../user/";
            try
            {
                File.Delete(absolutePath + "panelSettings.json");
                File.WriteAllText(
                    absolutePath + "panelSettings.json",
                    JsonConvert.SerializeObject(panelInfoList, Formatting.Indented, serializerSettings)
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
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../user/";
            try
            {
                File.Delete(absolutePath + "SystemAlerts.json");
                File.WriteAllText(
                    absolutePath + "SystemAlerts.json",
                    JsonConvert.SerializeObject(assistanceCards, Formatting.Indented, serializerSettings)
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
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../user/";
            try
            {
                File.Delete(absolutePath + "SlaveiPadsPass.json");
                File.WriteAllText(
                    absolutePath + "SlaveiPadsPass.json",
                    JsonConvert.SerializeObject(newPass, Formatting.Indented, serializerSettings)
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
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../user/";
            try
            {
                File.Delete(absolutePath + "PortableEquipment/VideoReceivers.json");
                File.WriteAllText(
                    absolutePath + "PortableEquipment/VideoReceivers.json",
                    JsonConvert.SerializeObject(receivers, Formatting.Indented, serializerSettings)
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
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../user/";
            try
            {
                File.Delete(absolutePath + "PortableEquipment/TVs.json");
                File.WriteAllText(
                    absolutePath + "PortableEquipment/TVs.json",
                    JsonConvert.SerializeObject(tvs, Formatting.Indented, serializerSettings)
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
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../user/";
            try
            {
                File.Delete(absolutePath + "PortableEquipment/Transmitter.json");
                File.WriteAllText(
                    absolutePath + "PortableEquipment/Transmitter.json",
                    JsonConvert.SerializeObject(transmitter, Formatting.Indented, serializerSettings)
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
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../user/";
            try
            {
                File.Delete(absolutePath + "ColabScreens/VideoReceivers.json");
                File.WriteAllText(
                    absolutePath + "ColabScreens/VideoReceivers.json",
                    JsonConvert.SerializeObject(receivers, Formatting.Indented, serializerSettings)
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
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../user/DigitalSignage/";
            try
            {
                File.Delete(absolutePath + "zoneAssignment.json");
                File.WriteAllText(
                    absolutePath + "zoneAssignment.json",
                    JsonConvert.SerializeObject(signageZones, Formatting.Indented, serializerSettings)
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
