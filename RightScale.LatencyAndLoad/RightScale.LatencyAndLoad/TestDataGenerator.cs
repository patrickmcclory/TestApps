using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace RightScale.LatencyAndLoad
{
    public class TestDataGenerator
    {
        private static TestDataGenerator instance;

        public int payloadMin { get; set; }
        public int payloadMax { get; set; }
        public int payloadAvg { get; set; }
        public int payloadStdDev { get; set; }
        public int pctPayloadBig { get; set; }
        public int pctPayloadSmall { get; set; }

        private TestDataGenerator()
        {
            payloadMin = int.Parse(ConfigurationManager.AppSettings["minPayload"]);
            payloadMax = int.Parse(ConfigurationManager.AppSettings["maxPayload"]);
            payloadAvg = int.Parse(ConfigurationManager.AppSettings["avgPayload"]);
            payloadStdDev = int.Parse(ConfigurationManager.AppSettings["stDevPayload"]);
            pctPayloadBig = int.Parse(ConfigurationManager.AppSettings["pctPayloadBig"]);
            pctPayloadSmall = int.Parse(ConfigurationManager.AppSettings["pctPayloadSmall"]);

            latencyMin = int.Parse(ConfigurationManager.AppSettings["minLatency"]);
            latencyMax = int.Parse(ConfigurationManager.AppSettings["maxLatency"]);
            latencyAvg = int.Parse(ConfigurationManager.AppSettings["avgLatency"]);
            latencyStdDev = int.Parse(ConfigurationManager.AppSettings["stDevLatency"]);
            latencyPctLow = int.Parse(ConfigurationManager.AppSettings["pctLatencyLow"]);
            latencyPctHigh = int.Parse(ConfigurationManager.AppSettings["pctLatencyHigh"]);
        }

        public static TestDataGenerator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TestDataGenerator();
                }
                return instance;
            }
        }


        /// <summary> 
        /// Static method returns a random byte array centered on the average size with variances for smaller and larger sets of data based on configuration
        /// </summary>
        /// <returns>random sized byte array based on configuration</returns>
        internal byte[] returnData()
        {
            byte[] retVal;

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

        public int latencyMin { get; set; }
        public int latencyMax { get; set; }
        public int latencyAvg { get; set; }
        public int latencyStdDev { get; set; }
        public int latencyPctLow { get; set; }
        public int latencyPctHigh { get; set; }
        
        /// <summary>
        /// method gets weighted random latency based on configuration
        /// </summary>
        /// <returns>integer representing the milliseconds that should be injected as latency for this request</returns>
        internal int getLatency()
        {
            int retVal = 0;

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