using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Configuration;

namespace RightScale.LatencyAndLoad
{
    public partial class Default : System.Web.UI.Page
    {
        public Default()
        {
            System.Diagnostics.Trace.AutoFlush = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;

            byte[] responseData = returnData();

            Response.Write(System.Text.Encoding.UTF8.GetString(responseData));
            Response.Flush();
            int latency = getLatency();
            DateTime targetEnd = start.AddMilliseconds(latency);

            while (targetEnd > DateTime.Now)
            {
                //loop it!
            }
            DateTime end = DateTime.Now;
            TimeSpan span = end - start;
            int ms = (int)span.TotalMilliseconds;
            
            System.Diagnostics.Trace.WriteLine(latency.ToString() + "|" + ms.ToString() + "|" + responseData.Length.ToString());

            Response.End();
        }


        /// <summary> 
        /// Static method returns a random byte array centered on the average size with variances for smaller and larger sets of data based on configuration
        /// </summary>
        /// <returns>random sized byte array based on configuration</returns>
        private byte[] returnData()
        {
            byte[] retVal;

            int payloadMin = int.Parse(ConfigurationManager.AppSettings["minPayload"]);
            int payloadMax = int.Parse(ConfigurationManager.AppSettings["maxPayload"]);
            int payloadAvg = int.Parse(ConfigurationManager.AppSettings["avgPayload"]);
            int payloadStdDev = int.Parse(ConfigurationManager.AppSettings["stDevPayload"]);
            int pctPayloadBig = int.Parse(ConfigurationManager.AppSettings["pctPayloadBig"]);
            int pctPayloadSmall = int.Parse(ConfigurationManager.AppSettings["pctPayloadSmall"]);

            Random rndTester = new Random();

            bool isBig = false;
            bool isSmall = false;

            if (rndTester.Next(0, 100) < pctPayloadBig)
            {
                isBig = true;
            }
            else if (rndTester.Next(0, 100) < pctPayloadSmall)
            {
                isSmall = true;
            }

            int payloadSize = 0;

            if (!isBig && !isSmall)
            {
                payloadSize = rndTester.Next(payloadAvg - payloadStdDev, payloadAvg + payloadStdDev);
            }
            else if (isBig)
            {
                payloadSize = rndTester.Next(payloadAvg + payloadStdDev, payloadMax);
            }
            else if (isSmall)
            {
                payloadSize = rndTester.Next(payloadMin, payloadAvg - payloadStdDev);
            }
            else
            {
                throw new Exception("this scenario should be impossible");
            }

            retVal = new byte[payloadSize];

            rndTester.NextBytes(retVal);

            return retVal;
        }

        /// <summary>
        /// method gets weighted random latency based on configuration
        /// </summary>
        /// <returns>integer representing the milliseconds that should be injected as latency for this request</returns>
        private int getLatency()
        {
            int retVal = 0;

            int latencyMin = int.Parse(ConfigurationManager.AppSettings["minLatency"]);
            int latencyMax = int.Parse(ConfigurationManager.AppSettings["maxLatency"]);
            int latencyAvg = int.Parse(ConfigurationManager.AppSettings["avgLatency"]);
            int latencyStdDev = int.Parse(ConfigurationManager.AppSettings["stDevLatency"]);
            int latencyPctLow = int.Parse(ConfigurationManager.AppSettings["pctLatencyLow"]);
            int latencyPctHigh = int.Parse(ConfigurationManager.AppSettings["pctLatencyHigh"]);

            bool isLow = false;
            bool isHigh = false;

            Random rndTester = new Random();

            if (rndTester.Next(0, 100) < latencyPctHigh)
            {
                isHigh = true;
            }
            else if (rndTester.Next(0, 100) < latencyPctLow)
            {
                isLow = true;
            }

            if (!isHigh && !isLow)
            {
                retVal = rndTester.Next(latencyAvg - latencyStdDev, latencyAvg + latencyStdDev);
            }
            else if (isHigh)
            {
                retVal = rndTester.Next(latencyAvg + latencyStdDev, latencyMax);
            }
            else if (isLow)
            {
                retVal = rndTester.Next(latencyMin, latencyAvg - latencyStdDev);
            }

            if (retVal == 0)
            {
                throw new ArgumentException("Latency cannot be 0... you can't move faster than the speed of light :)");
            }

            return retVal;
        }
    }
}