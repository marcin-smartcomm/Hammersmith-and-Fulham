using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace H_and_F_Core
{
    public class SSE_Server
    {
        ControlSystem cs;
        static List<Tuple<HttpListenerContext, string>> _eventListeners;

        public SSE_Server(ControlSystem cs)
        {
            try
            {
                this.cs = cs;
                _eventListeners = new List<Tuple<HttpListenerContext, string>>();
                EventListenerAsync();
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("Problem in SSE_Server Constructor: " + ex.Message);
            }
        }

        public async void EventListenerAsync()
        {
            try
            {
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add("http://*:50001/api/events/");
                listener.Start();

                ConsoleLogger.WriteLine("Event Server Started...");

                while (true)
                {
                    try
                    {
                        //Await Client Request
                        HttpListenerContext context = await listener.GetContextAsync();
                        await Task.Run(() => ProcessEventRequestAsync(context));
                        ConsoleLogger.WriteLine("New event subscription: " + context.Request.RemoteEndPoint.Address.ToString());
                    }
                    catch (HttpListenerException) { break; }
                    catch (InvalidOperationException) { break; }
                }

                listener.Stop();
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine($"{ex.Message}");
            }
        }

        public void DisconnectFromStream(string IPAddress)
        {
            foreach (Tuple<HttpListenerContext, string> entry in _eventListeners)
            {
                if (entry.Item2 == IPAddress)
                {
                    ConsoleLogger.WriteLine("_eventListener IP: " + entry.Item2 + " || disconnect request IP: " + IPAddress + " --DELETING--");
                    _eventListeners.Remove(entry);
                    DisconnectFromStream(IPAddress);
                    break;
                }
                else
                {
                    ConsoleLogger.WriteLine("_eventListener IP: " + entry.Item2 + " || disconnect request IP: " + IPAddress);
                }
            }
        }

        public static void UpdateAllConnected(string infoChanged)
        {
            var inactiveListeners = new List<Tuple<HttpListenerContext, string>>();

            foreach (Tuple<HttpListenerContext, string> entry in _eventListeners)
            {
                string message = "data: NEWINFO:" + infoChanged + "\n\n";
                byte[] messageBytes = ASCIIEncoding.ASCII.GetBytes(message);

                try
                {
                    entry.Item1.Response.OutputStream.WriteAsync(messageBytes, 0, messageBytes.Length);
                    entry.Item1.Response.OutputStream.FlushAsync();
                }
                catch (Exception ex)
                {
                    ConsoleLogger.WriteLine("Could not send event data to: " + entry.Item1.Request.UserHostAddress + ", Reason: \n" + ex.Message);
                    inactiveListeners.Add(entry);
                }
            }

            foreach(var inactiveListener in inactiveListeners)
                _eventListeners.Remove(inactiveListener);
        }

        public void SendTimeToAllConnected(DateAndTime newDateAndTime)
        {
            string dataToSend = newDateAndTime.currentMinute + ":" + newDateAndTime.currentHour + ":" + newDateAndTime.DayOfWeek + ":" + newDateAndTime.currentDay + ":" + newDateAndTime.currentMonth + ":" + newDateAndTime.currentYear;

            foreach (Tuple<HttpListenerContext, string> entry in _eventListeners)
            {
                string message = "data: CORETIME:" + dataToSend + "\n\n";
                byte[] messageBytes = ASCIIEncoding.ASCII.GetBytes(message);

                try
                {
                    entry.Item1.Response.OutputStream.WriteAsync(messageBytes, 0, messageBytes.Length);
                    entry.Item1.Response.OutputStream.FlushAsync();
                }
                catch (Exception ex)
                {
                    ConsoleLogger.WriteLine("Could not send event data to: " + entry.Item1.Request.UserHostAddress + ", Reason: \n" + ex.Message);
                    _eventListeners.Remove(entry);
                    break;
                }
            }
        }

        void ProcessEventRequestAsync(HttpListenerContext context)
        {
            try
            {
                string IP = context.Request.RemoteEndPoint.Address.ToString();

                context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Content-Type", "text/event-stream");
                context.Response.Headers.Add("Cache-Control", "no-cache");
                context.Response.Headers.Add("Connection", "keep-alive");

                byte[] responseBytes = Encoding.UTF8.GetBytes("\nevent: update\n\n");
                context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                context.Response.OutputStream.FlushAsync();

                _eventListeners.Add(new Tuple<HttpListenerContext, string>(context, IP));

                if (_eventListeners.Count > 1)
                    CheckForDuplicates();

                ConsoleLogger.WriteLine(_eventListeners.Count + " Event Listeners: ");
                foreach (Tuple<HttpListenerContext, string> entry in _eventListeners)
                {
                    ConsoleLogger.WriteLine("IP: " + entry.Item2);
                }
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("Bad request..." + ex.Message);
            }
        }

        void CheckForDuplicates()
        {
            Tuple<HttpListenerContext, string> itemToCheck = _eventListeners.Last();

            for (int i = 0; i < _eventListeners.Count - 1; i++)
            {
                if (_eventListeners[i].Item2 == itemToCheck.Item2)
                    _eventListeners.RemoveAt(i);
            }
        }
    }
}