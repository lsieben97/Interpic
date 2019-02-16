using Interpic.Alerts;
using Interpic.Extensions;
using Interpic.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic
{
    public class Logger : ILogger
    {
        private StreamWriter logWriter;

        public void OpenLog(string logfile, IStudioEnvironment environment)
        {
            try
            {
                logWriter = new StreamWriter(logfile);
                LogInfo("Opening log for interpic studio " + environment.GetStudioVersion());
            }
            catch(Exception ex)
            {
                ErrorAlert.Show("Could not create/open log file. Details:\n" + ex.Message);
            }
        }
        
        public bool IsOpen()
        {
            return logWriter != null ? logWriter.BaseStream.CanWrite : false;
        }
        public string GetLog()
        {
            try
            {
                logWriter.Flush();
                return new StreamReader(logWriter.BaseStream).ReadToEnd();
            }
            catch(Exception)
            {
                return null;
            }
        }

        private string GetLogHeader(string severity, string source)
        {
            return DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "\t\t[" + severity + "]\t\t(" + source + ")\t\t";
        }

        public void LogInfo(string message, string source = "Interpic Studio extension")
        {
            logWriter.WriteLine(GetLogHeader("Info", source) + message);
        }

        public void LogWarning(string message, string source = "Interpic Studio extension")
        {
            logWriter.WriteLine(GetLogHeader("Warning", source) + message);
        }

        public void LogError(string message, string source = "Interpic Studio extension")
        {
            logWriter.WriteLine(GetLogHeader("Error", source) + message);
        }

        public void LogDebug(string message, string source = "Interpic Studio extension")
        {
            logWriter.WriteLine(GetLogHeader("Debug", source) + message);
        }
    }
}
