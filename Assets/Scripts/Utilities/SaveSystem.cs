using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Utilities;
using System;

namespace Utilities.SaveOperations
{
    public enum DataCategory
    {
        Profile,
        Group,
        Inventory,
        VirtualCurrency
    }

    public static class SaveSystem
    {
        public static Dictionary<DataCategory, string> FileNames = new Dictionary<DataCategory, string>();
        public static void Save<T>(T data, DataCategory c)
        {
            string name = c.ToString();
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/"
                + name + ".bruh";

            FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            formatter.Serialize(fileStream, data);

            if(!FileNames.ContainsKey(c))
            {
                FileNames.Add(c, name);
            }
            else
            {
                FileNames[c] = name;    
            }

            fileStream.Close();
        }

        public static T Load<T>(DataCategory c) where T : class
        {
            string fileName = FileNames[c];
            string path = Application.persistentDataPath + "/" +
                fileName + ".bruh";

            if (File.Exists(path))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream file = new FileStream(path, FileMode.Open);

                T data = binaryFormatter.Deserialize(file) as T;
                return data;
            }
            else
            {
                HelperFunctions.Error("File path: " + path + " does NOT exist...idiot");
                return default(T);
            }

        }
    }
}

