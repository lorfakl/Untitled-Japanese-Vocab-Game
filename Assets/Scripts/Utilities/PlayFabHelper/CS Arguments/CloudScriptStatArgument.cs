using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.PlayFabHelper.CSArguments
{
    [Serializable]
    public class CloudScriptStatArgument
    {
        [JsonProperty("statName")]
        public string statName { get; set; }

        [JsonProperty("value")]
        public string value { get; set; }

        public CloudScriptStatArgument(StatisticName n, int val)
        {
            statName = n.ToString();
            value = EncodeStatisticValue(val);
        }

        [JsonConstructor]
        public CloudScriptStatArgument(string n, int val)
        {
            statName = n;
            value = val.ToString();
        }

        private string EncodeStatisticValue(int value)
        {

            //HelperFunctions.Log("Convert to Byte Array: ");
            byte[] intBytes = BitConverter.GetBytes(value);
            string byteString = "";
            foreach (byte b in intBytes)
            {
                Console.Write(b.ToString() + ", ");
                byteString += b.ToString() + ",";
            }

            return byteString;
        }

        public int DecodeStatisticValue()
        {
            List<byte> bytes = new List<byte>();

            foreach (string b in value.Split(','))
            {
                if (!String.IsNullOrEmpty(b))
                {
                    bytes.Add(Convert.ToByte(b));
                }
            }

            //Console.WriteLine(");
            foreach (byte b in bytes)
            {
                Console.Write(b.ToString() + ", ");

            }

            byte[] properArray = bytes.ToArray();


            long i = BitConverter.ToInt64(properArray, 0);

            return (int)i;

        }

        public override string ToString()
        {
            return $"Stat Name: {statName} \n" + $"Stat Value: {value}";
        }
    }
}
