using Newtonsoft.Json;
using System;

namespace H_and_F_Core
{
    public class GlobalTemp
    {
        public decimal globalTemp { get; set; }
    }

    public class GlobalTempControl
    {
        static decimal _upperSetpointLimit = 24.0M;
        static decimal _lowerSetpointLimit = 20.0M;

        public static decimal TempUp()
        {
            GlobalTemp globalTemp = JsonConvert.DeserializeObject<GlobalTemp>(FileOperations.loadJson("GlobalTemp"));

            if (globalTemp.globalTemp >= _upperSetpointLimit)
                globalTemp.globalTemp = _upperSetpointLimit;
            else
                globalTemp.globalTemp = Math.Round((globalTemp.globalTemp + 0.1M), 1);

            FileOperations.saveGlobalTemp(globalTemp);

            return globalTemp.globalTemp;
        }

        public static decimal TempDown()
        {
            GlobalTemp globalTemp = JsonConvert.DeserializeObject<GlobalTemp>(FileOperations.loadJson("GlobalTemp"));

            if (globalTemp.globalTemp <= _lowerSetpointLimit)
                globalTemp.globalTemp = _lowerSetpointLimit;
            else
                globalTemp.globalTemp = Math.Round((globalTemp.globalTemp - 0.1M), 1);

            FileOperations.saveGlobalTemp(globalTemp);

            return globalTemp.globalTemp;
        }
    }
}
