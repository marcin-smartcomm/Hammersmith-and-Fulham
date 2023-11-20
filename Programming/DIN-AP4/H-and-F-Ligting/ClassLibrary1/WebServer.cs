using System.Net;
using System.Text;
using System;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace H_and_F_Lighting
{
    class WebServer
    {
        ControlSystem _controlSystem;

        public WebServer(ControlSystem cs)
        {
            try
            {
                _controlSystem = cs;
                ListenAsync();
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("Problem in WebServer Constructor: " + ex.Message);
            }
        }

        public async void ListenAsync()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://*:50000/api/");
            listener.Start();

            ConsoleLogger.WriteLine("Server Started...");

            while (true)
            {
                try
                {
                    //Await Client Request
                    HttpListenerContext context = await listener.GetContextAsync();
                    await Task.Run(() => ProcessRequestAsync(context));
                }
                catch (HttpListenerException) { break; }
                catch (InvalidOperationException) { break; }
            }

            listener.Stop();
        }

        async void ProcessRequestAsync(HttpListenerContext context)
        {
            try
            {
                //Respond to Request
                string response = "";
                string incomingRequest = context.Request.RawUrl;
                ConsoleLogger.WriteLine("Request Coming on " + context.Request.RawUrl + " || from: " + context.Request.RemoteEndPoint.Address.ToString());

                if (incomingRequest.Contains("/SetNewScene"))
                {
                    string lightingAreaNumber = incomingRequest.Split('?')[1].Split(':')[0];
                    string newScene = incomingRequest.Split('?')[1].Split(':')[1].Replace("%20", " ");

                    _controlSystem.SetNewScene(newScene, ushort.Parse(lightingAreaNumber));

                    response = "{ \"Acknowledge\": \"true\" }";
                }

                if (incomingRequest.Contains("/GetCurrentScene"))
                {
                    string lightingAreaNumber = incomingRequest.Split('?')[1];

                    response = _controlSystem.GetCurrentLightScene(ushort.Parse(lightingAreaNumber));
                    ConsoleLogger.WriteLine(response);
                }

                context.Response.ContentLength64 = Encoding.UTF8.GetByteCount(response);
                context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                context.Response.AddHeader("Access-Control-Allow-Methods", "*");
                context.Response.AddHeader("Access-Control-Allow-Headers", "*");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.OK;

                using (Stream s = context.Response.OutputStream)
                using (StreamWriter writer = new StreamWriter(s))
                    await writer.WriteAsync(response);
            }
            catch (Exception ex) { ConsoleLogger.WriteLine("Bad Request: " + ex.Message); }
        }
    }
}