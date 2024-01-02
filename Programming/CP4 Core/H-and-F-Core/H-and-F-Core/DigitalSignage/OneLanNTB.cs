using System;
using System.Text;
using System.Net;
using System.IO;

namespace H_and_F_Core
{
    public class NTBPost
    {
        HttpWebRequest PrepareRequestHeaders(string ip)
        {
            string url = string.Format($"http://{ip}/XML/");

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "text/xml";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 1000;

            Uri uri = new Uri(url);
            CredentialCache cc = new CredentialCache();
            cc.Add(uri, "Digest", new NetworkCredential("remote", "9999"));

            httpWebRequest.Credentials = cc;

            return httpWebRequest;
        }

        public void SendCommand(string ip, string rs232Command)
        {
            Console.WriteLine("Trying to send a command to NTB");
            var httpWebRequest = PrepareRequestHeaders(ip);

            try
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\n <transmit_rs232_data>" + rs232Command + "</transmit_rs232_data>";
                    streamWriter.Write(json);
                    Console.WriteLine($"Sending Command to NTB ({ip}): " + rs232Command);
                }
            }catch(Exception ex)
            {
                ConsoleLogger.WriteLine("Exception in NTBPost.SendCommand(): \n" + ex);
            }
        }

        public string ReceiveCommand(string ip)
        {
            Console.WriteLine("Trying to send a command to NTB");
            var httpWebRequest = PrepareRequestHeaders(ip);
            string dataInBuffer = "";

            try
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\n <get_rs232_buffered_data clear_buffer=\"yes\"/>";
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd(); ;

                    int startposition = result.IndexOf("<rs232_buffered_data>") + 21;
                    int length = result.IndexOf("</rs232_buffered_data>") - startposition;
                    dataInBuffer = Encoding.ASCII.GetString(ConvertToByteArray(result.Substring(startposition, length)));

                    Console.WriteLine($"NTB ({ip}) RS232 Buffer: " + dataInBuffer);
                }
            }catch(Exception ex)
            {
                ConsoleLogger.WriteLine("Exception in NTBPost.ReceiveCommand(): \n" + ex);
            }
            return dataInBuffer;
        }

        byte[] ConvertToByteArray(string hexString)
        {
            byte[] raw = new byte[hexString.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return raw;
        }
    }
}