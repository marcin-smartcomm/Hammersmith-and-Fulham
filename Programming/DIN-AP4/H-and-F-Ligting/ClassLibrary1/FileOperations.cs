using Newtonsoft.Json;
using System;
using System.IO;

namespace H_and_F_Lighting
{
    public static class FileOperations
    {
        public static string loadAreasInfo()
        {
            try
            {
                string absolutePath = @"../../user/";
                StreamReader sr = new StreamReader(absolutePath + "AreaList.json");

                string json = sr.ReadToEnd();
                sr.Close();

                return json;
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.loadAreasInfo()\n" + ex.ToString());
                return null;
            }
        }
        public static string loadScenes()
        {
            try
            {
                string absolutePath = @"../../user/";
                StreamReader sr = new StreamReader(absolutePath + "ScenesAvailable.json");

                string json = sr.ReadToEnd();
                sr.Close();

                return json;
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.loadScenes()\n" + ex.ToString());
                return null;
            }
        }
        public static void saveSceneInfo(ScenesList newList)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../../user/";
            try
            {
                File.Delete(absolutePath + "ScenesAvailable.json");
                File.WriteAllText(
                    absolutePath + "ScenesAvailable.json",
                    JsonConvert.SerializeObject(newList, Formatting.Indented, serializerSettings)
                    );
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveUserInfo(): " + ex.ToString());
            }
        }
        public static void saveAreaInfo(AreaList newList)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string absolutePath = @"../../user/";
            try
            {
                File.Delete(absolutePath + "AreaList.json");
                File.WriteAllText(
                    absolutePath + "AreaList.json",
                    JsonConvert.SerializeObject(newList, Formatting.Indented, serializerSettings)
                    );
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("issue in fileManager.saveUserInfo(): " + ex.ToString());
            }
        }
    }
}
