using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace MPS.Common.Log
{
    static public class LogHelper
    {
        static private object _mutex = new object();
        static private string LogDir
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
            }
        }

        static private string LogFile
        {
            get
            {
                string filename = string.Format("{0}-{1}-{2}_log.xml", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                return Path.Combine(LogDir, filename);
            }
        }


        static public void WriteLog(Exception ex)
        {
            WriteLog(LogMode.Error, CreateErrorLog(ex));
        }
        static public void WriteLog(LogMode logmode, string content)
        {
            try
            {
                lock (_mutex)
                {
                    XmlDocument doc = LoadLogFile();
                    XmlNode logentryNode = CreateLogEntryNode(doc);
                    logentryNode.SelectSingleNode("Host").InnerText = string.Join(",", System.Net.Dns.GetHostName());
                    logentryNode.SelectSingleNode("mode").InnerText = logmode.ToString();
                    logentryNode.SelectSingleNode("content").InnerText = content;
                    logentryNode.SelectSingleNode("date").InnerText = DateTime.Now.ToString();
                    doc.Save(LogFile);
                }
            }
            catch
            {

            }
        }
        static public void WriteLog(LogMode logmode,string path, string content)
        {
            try
            {
                lock (_mutex)
                {
                    XmlDocument doc = LoadLogFile(path);
                    XmlNode logentryNode = CreateLogEntryNode(doc);
                    logentryNode.SelectSingleNode("Host").InnerText = string.Join(",", System.Net.Dns.GetHostName());
                    logentryNode.SelectSingleNode("mode").InnerText = logmode.ToString();
                    logentryNode.SelectSingleNode("content").InnerText = content;
                    logentryNode.SelectSingleNode("date").InnerText = DateTime.Now.ToString();
                    string filename = string.Format("{0}-{1}-{2}_log.xml", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                    var filepath = Path.Combine(path, filename);
                    doc.Save(filepath);
                }
            }
            catch
            {

            }
        }
        static private XmlNode CreateLogEntryNode(XmlDocument doc)
        {
            XmlNode logentryNode = doc.CreateElement("entry");
            doc.FirstChild.AppendChild(logentryNode);

            XmlNode hostNode = doc.CreateElement("Host");
            logentryNode.AppendChild(hostNode);

            XmlNode modeNode = doc.CreateElement("mode");
            logentryNode.AppendChild(modeNode);
            XmlNode contentNode = doc.CreateElement("content");
            logentryNode.AppendChild(contentNode);
            XmlNode dateNode = doc.CreateElement("date");
            logentryNode.AppendChild(dateNode);

            return logentryNode;
        }
        static private XmlDocument LoadLogFile()
        {
            XmlDocument doc = new XmlDocument();
            if (!System.IO.Directory.Exists(LogDir))
            {
                System.IO.Directory.CreateDirectory(LogDir);
            }
            if (System.IO.File.Exists(LogFile))
            {
                doc.Load(LogFile);
            }
            if (doc.FirstChild == null)
            {
                XmlNode rootNode = doc.CreateElement("log");
                doc.AppendChild(rootNode);
            }
            return doc;
        }

        static private XmlDocument LoadLogFile(string logpath)
        {
            XmlDocument doc = new XmlDocument();
            if (!System.IO.Directory.Exists(logpath))
            {
                System.IO.Directory.CreateDirectory(logpath);
            }
            string filename = string.Format("{0}-{1}-{2}_log.xml", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
           var filepath= Path.Combine(logpath, filename);
            if (System.IO.File.Exists(filepath))
            {
                doc.Load(LogFile);
            }
            if (doc.FirstChild == null)
            {
                XmlNode rootNode = doc.CreateElement("log");
                doc.AppendChild(rootNode);
            }
            return doc;
        }
        public static Exception GetRawException(System.Exception e)
        {
            if (e.InnerException == null)
            {
                return e;
            }
            return GetRawException(e.InnerException);
        }

        private static string CreateErrorLog(Exception exception)
        {
            System.Exception ex = GetRawException(exception);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("Exception Message:{0}\r\n", ex.Message);
            sb.AppendFormat("Exception Source:{0}\r\n", ex.Source);
            sb.AppendFormat("Exception StackTrace:{0}\r\n", ex.StackTrace);

            return sb.ToString();
        }
    }
    public enum LogMode
    {
        Error,
        Warning,
        Info
    }
}
