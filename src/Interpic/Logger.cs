using Interpic.Alerts;
using Interpic.Models.Extensions;
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
        internal List<string> InternalLog { get; set; } = new List<string>();

        public void OpenLog(string logfile, IStudioEnvironment environment)
        {
            try
            {
                logWriter = new StreamWriter(logfile);
                LogInfo("Opening log for interpic studio " + environment.GetStudioVersion(),"Studio");
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
                return string.Join("", InternalLog);
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
            InternalLog.Add(GetLogHeader("Info", source) + message + "\n");
            logWriter.Flush();
        }

        public void LogWarning(string message, string source = "Interpic Studio extension")
        {
            logWriter.WriteLine(GetLogHeader("Warning", source) + message);
            InternalLog.Add(GetLogHeader("Warning", source) + message + "\n");
            logWriter.Flush();
        }

        public void LogError(string message, string source = "Interpic Studio extension")
        {
            logWriter.WriteLine(GetLogHeader("Error", source) + message);
            InternalLog.Add(GetLogHeader("Error", source) + message + "\n");
            logWriter.Flush();
        }

        public void LogDebug(string message, string source = "Interpic Studio extension")
        {
            logWriter.WriteLine(GetLogHeader("Debug", source) + message);
            InternalLog.Add(GetLogHeader("Debug", source) + message + "\n");
            logWriter.Flush();
        }
    }
}
