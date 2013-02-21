using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Configuration;
using System.Threading;

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

            byte[] responseData = TestDataGenerator.Instance.returnData();

            Response.Write(System.Text.Encoding.UTF8.GetString(responseData));
            int latency = TestDataGenerator.Instance.getLatency();

            DateTime targetEnd = start.AddMilliseconds(latency);

            DateTime end = DateTime.Now;
            TimeSpan span = end - start;

            if (span.TotalMilliseconds > 0)
                Thread.Sleep(span);

            int ms = (int)span.TotalMilliseconds;
            
            System.Diagnostics.Trace.WriteLine(latency.ToString() + "|" + ms.ToString() + "|" + responseData.Length.ToString());

            Response.Flush();
            Response.End();
        }



    }
}