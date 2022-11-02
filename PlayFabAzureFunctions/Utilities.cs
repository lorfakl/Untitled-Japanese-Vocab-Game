using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Utilities
{
    public enum ProficiencyLevels
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8
    }

    public static class HelperFunctions
    {
        public static ILogger Logger
        {
            get;
            set;
        }

        static Dictionary<DateTime, string> debugDict = new Dictionary<DateTime, string>();

        public static Dictionary<DateTime, string> FunctionProgressData
        {
            get {return debugDict;}
        }

        public static void Error(string msg)
        {
            Logger.LogError(msg);
            debugDict.Add(DateTime.UtcNow, msg.ToString());
        }

        public static void Log<T>(T msg)
        {
            Logger.LogDebug(DateTime.UtcNow + ": " + msg.ToString());
            debugDict.Add(DateTime.UtcNow, msg.ToString());
        }

        public static void LogListContent<T>(string msg, List<T> list)
        {
            string contents = PrintListContent(list);
            Log(msg + contents);
        }

        public static void LogListContent<T>(List<T> list)
        {
            string contents = PrintListContent(list);
            Log(contents);
        }

        public static void LogDictContent<Tkey, TVal>(Dictionary<Tkey, TVal> dict)
        {
            string dictContents = "";
            foreach (Tkey key in dict.Keys)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                dictContents += string.Format("Key = {0}, Value = {1}", key.ToString(), dict[key]) + "\n";
            }
            Log(dictContents);
        }

        public static string PrintListContent<T>(List<T> list)
        {
            string contents = "";
            foreach (T element in list)
            {
                contents += element.ToString() + " ";
            }

            return contents;
        }

        public static void Print(string msg)
        {
            Log(msg);
        }

        public static void CatchException(Exception e)
        {
            string err = "Source: " + e.Source + "\n" + " Message: " + e.Message + "\n" + " StackTrace: " + e.StackTrace + "\n" + " Inner Exception" + e.InnerException.ToString();
            Error(err);
        }

        public static void PrintObjectProperties<T>(T src)
        {
            Type type = typeof(T);

            PropertyInfo[] propertyInfo = type.GetProperties();

            foreach (PropertyInfo pInfo in propertyInfo)
            {
                string val = type.GetProperty(pInfo.Name)?.GetValue(src, null)?.ToString();
                if (!String.IsNullOrEmpty(val))
                {
                    Print(pInfo.Name + ": " + val);
                }
            }
        }
    
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}