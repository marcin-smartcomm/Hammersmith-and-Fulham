using System;
using System.Collections.Generic;
using System.IO.BACnet;
using System.Runtime.InteropServices;

namespace H_and_F_Room_Controller
{
    public class BACnetComms
    {
        BacnetClient bacnetClient;
        BacnetClient.IamHandler handler;
        List<BacNode> DevicesList = new List<BacNode>();
        int _controllerObjectID;

        public BACnetComms(int controllerObjectID)
        {
            StartActivity();
            _controllerObjectID = controllerObjectID;
        }
        void StartActivity()
        {
            bacnetClient = new BacnetClient(new BacnetIpUdpProtocolTransport(0xBAC0, false));

            handler = new BacnetClient.IamHandler(handler_OnIam);

            bacnetClient.Start();
            bacnetClient.OnIam += handler;

            bacnetClient.WhoIs();
        }
        public void StopActivity()
        {
            bacnetClient.OnIam -= handler;
        }
        void handler_OnIam(BacnetClient sender, BacnetAddress adr, uint device_id, uint max_apdu, BacnetSegmentations segmentation, ushort vendor_id)
        {
            if (ControlSystem.debugEnabled)
            ConsoleLogger.WriteLine("I am " + device_id);

            lock (DevicesList)
            {
                // Device already registred ?
                foreach (BacNode bn in DevicesList)
                    if (bn.getAdd(device_id) != null) return;   // Yes

                // Not already in the list
                DevicesList.Add(new BacNode(adr, device_id));   // add it
            }
        }

        public float SetNewSetpointValue(uint analogValueObjectID, decimal newSetpoint)
        {
            if (ControlSystem.debugEnabled)
                ConsoleLogger.WriteLine($"BACNET: Writing {newSetpoint} to {_controllerObjectID}:AV{analogValueObjectID}");

            bool ret;
            BacnetValue currentSetpointValue;

            ret = ReadScalarValue(_controllerObjectID, new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_VALUE, analogValueObjectID), BacnetPropertyIds.PROP_PRESENT_VALUE, out currentSetpointValue);

            if (ret)
            {
                BacnetValue newValue = new BacnetValue(Convert.ToSingle(newSetpoint));

                ret = WriteScalarValue(_controllerObjectID, new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_VALUE, analogValueObjectID), BacnetPropertyIds.PROP_PRESENT_VALUE, newValue);

                if (ControlSystem.debugEnabled)
                    ConsoleLogger.WriteLine("Writing Temp feedback : " + ret.ToString());
                return float.Parse(newValue.Value.ToString());
            }

            return float.Parse(currentSetpointValue.Value.ToString());
        }
        public decimal ReadSetpointValue(uint analogValueObjectID)
        {
            return ReadPresenAnalogValue(analogValueObjectID);
        }
        public decimal ReadSpaceTempValue(uint analogValueObjectID)
        {
            return ReadPresenAnalogValue(analogValueObjectID);
        }
        public int ReadCo2Value(uint analogValueObjectID)
        {
            return int.Parse(ReadPresenAnalogValue(analogValueObjectID).ToString());
        }
        public int SetNewOccupancyMode(uint MSVObjectID, int newMode)
        {
            if (ControlSystem.debugEnabled)
                ConsoleLogger.WriteLine($"BACNET: Writing {newMode} to {_controllerObjectID}:MSV{MSVObjectID}");

            bool ret;
            BacnetValue currentSetpointValue;

            ret = ReadScalarValue(_controllerObjectID, new BacnetObjectId(BacnetObjectTypes.OBJECT_MULTI_STATE_VALUE, MSVObjectID), BacnetPropertyIds.PROP_PRESENT_VALUE, out currentSetpointValue);

            if (ret)
            {
                BacnetValue newValue = new BacnetValue(Convert.ToSingle(newMode));
                ret = WriteScalarValue(_controllerObjectID, new BacnetObjectId(BacnetObjectTypes.OBJECT_MULTI_STATE_VALUE, MSVObjectID), BacnetPropertyIds.PROP_PRESENT_VALUE, newValue);
                if (ControlSystem.debugEnabled)
                    ConsoleLogger.WriteLine("Writing Temp feedback : " + ret.ToString());
                return int.Parse(newValue.Value.ToString());
            }

            return int.Parse(currentSetpointValue.Value.ToString());
        }

        public decimal ReadPresenAnalogValue(uint analogValueObjectID)
        {
            bool ret;
            BacnetValue currentSetpointValue;

            ret = ReadScalarValue(_controllerObjectID, new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_VALUE, analogValueObjectID), BacnetPropertyIds.PROP_PRESENT_VALUE, out currentSetpointValue);

            return decimal.Parse(currentSetpointValue.Value.ToString());
        }

        bool ReadScalarValue(int device_id, BacnetObjectId BacnetObjet, BacnetPropertyIds Propriete, out BacnetValue Value)
        {
            BacnetAddress adr;
            IList<BacnetValue> NoScalarValue;

            Value = new BacnetValue(null);

            // Looking for the device
            adr = DeviceAddr((uint)device_id);
            if (adr == null) {if (ControlSystem.debugEnabled) ConsoleLogger.WriteLine("not found"); return false; }  // not found

            // Property Read
            if (bacnetClient.ReadPropertyRequest(adr, BacnetObjet, Propriete, out NoScalarValue) == false)
                return false;

            Value = NoScalarValue[0];
            return true;
        }
        bool WriteScalarValue(int device_id, BacnetObjectId BacnetObjet, BacnetPropertyIds Propriete, BacnetValue Value)
        {
            BacnetAddress adr;

            // Looking for the device
            adr = DeviceAddr((uint)device_id);
            if (adr == null) return false;  // not found

            // Property Write
            BacnetValue[] NoScalarValue = { Value };
            if (bacnetClient.WritePropertyRequest(adr, BacnetObjet, Propriete, NoScalarValue) == false)
                return false;

            return true;
        }

        BacnetAddress DeviceAddr(uint device_id)
        {
            BacnetAddress ret;

            lock (DevicesList)
            {
                foreach (BacNode bn in DevicesList)
                {
                    ret = bn.getAdd(device_id);
                    if (ret != null) return ret;
                }
                // not in the list
                return null;
            }
        }
        class BacNode
        {
            BacnetAddress adr;
            uint device_id;

            public BacNode(BacnetAddress adr, uint device_id)
            {
                this.adr = adr;
                this.device_id = device_id;
            }

            public BacnetAddress getAdd(uint device_id)
            {
                if (this.device_id == device_id)
                    return adr;
                else
                    return null;
            }
        }
    }
}
