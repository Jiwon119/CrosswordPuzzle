using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace CrosswordPuzzle
{
    class LogHelp
    {
        public enum LogTarget
        {
            File, Database, EventLog
        }

        public abstract class LogBase
        {
            public abstract void Log(string message);
        }

        public class FileLogger : LogBase
        {
            public string filePath = @"D:\IDGLog.txt";
            public override void Log(string message)
            {
                using (StreamWriter streamWriter = new StreamWriter(filePath))
                {
                    streamWriter.WriteLine(message);
                    streamWriter.Close();
                }
            }
        }

        public class DBLogger : LogBase
        {
            string connectionString = string.Empty;
            public override void Log(string message)
            {
                //Code to log data to the database
            }
        }

        public class EventLogger : LogBase
        {
            public override void Log(string message)
            {
                EventLog eventLog = new EventLog("");
                eventLog.Source = "IDGEventLog";
                eventLog.WriteEntry(message);
            }
        }

        public static class LogHelper
        {
            private static LogBase logger = null;
            public static void Log(LogTarget target, string message)
            {
                switch (target)
                {
                    case LogTarget.File:
                        logger = new FileLogger();
                        logger.Log(message);
                        break;
                    case LogTarget.Database:
                        logger = new DBLogger();
                        logger.Log(message);
                        break;
                    case LogTarget.EventLog:
                        logger = new EventLogger();
                        logger.Log(message);
                        break;
                    default:
                        return;
                }
            }
        }
    }
}