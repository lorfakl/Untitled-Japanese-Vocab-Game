using System;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Reflection;
//using PlayFab.ClientModels;
//using PlayFab.ServerModels;
//using PlayFab;
using System.Collections;
using ProjectSpecificGlobals;
using Utilities.PlayFabHelper;

namespace ProjectSpecificGlobals
{
    public enum ContactType { GroundContact, CubeContact, BlockContact };
    public enum SceneNames { MenuScene, OpeningScene, SampleScene, ArcadeLeaderboard, ArcadeOpeningScene, ArcadeStudyScene, ArcadeGameOver, EnenraScene }

    public enum Tags { MainCanvas}

    public static class MobileSettings
    {
        public static int dragPower = 250;
        public static int multiplier = 2;
    }
}

/// <summary>
/// The utilities class is for storing the general purpose commonly used functions and data structures 
/// </summary>
namespace Utilities
{

    public static class HelperFunctions
    {
        private static string conn = "URI=file:" + Application.dataPath + "/CardDataBase.db";
        public static Color[] colors = { Color.black, Color.gray, new Color(32, 123, 11), Color.blue, Color.cyan, Color.red, Color.green, Color.magenta, Color.yellow, new Color(137, 100, 4), new Color(115, 26, 113), new Color(56, 209, 143), new Color(92, 173, 81) };

        #region Vector3 Operations

        /// <summary>
        /// Rotates a vector a specific number of degrees using Trig
        /// </summary>
        /// <param name="startingVector"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Vector3 RotateVector(Vector3 startingVector, float degree)
        {
            float rads = ConvertToRadian(degree);

            float newZ = startingVector.z * Mathf.Cos(rads) - startingVector.y * Mathf.Sin(rads);
            float newY = startingVector.z * Mathf.Sin(rads) + startingVector.y * Mathf.Cos(rads);
            


            return new Vector3(startingVector.x, newY, newZ);
        }

        public static float ConvertToRadian(float degrees)
        {
            float rads = degrees * (Mathf.PI / 180);
            return rads;
        }

        #endregion

        #region Physics Calculations

        /// <summary>
        /// Calculates every position point a projectile will take
        /// given an initial position and velocity
        /// </summary>
        /// <param name="numOfPoints"></param>
        /// <param name="timeInterval"></param>
        /// <param name="initialVel"></param>
        /// <param name="initialPos"></param>
        /// <returns></returns>
        public static List<Vector3> CalculateProjectilePath(int numOfPoints, float timeInterval, Vector3 initialVel, Vector3 initialPos)
        {
            List<Vector3> points = new List<Vector3>();

            for (float t = 0; t < numOfPoints; t += timeInterval)
            {
                Vector3 newPoint = initialPos + t * initialVel;
                newPoint.y = initialPos.y + initialVel.y * t + Physics.gravity.y * 0.5f * t * t;
                points.Add(newPoint);

            }

            return points;
        }

        /// <summary>
        /// Returns the velocity required to get from initialPos to finalPos
        /// with the specified angle of launch
        /// </summary>
        /// <param name="numOfPoints"></param>
        /// <param name="timeInterval"></param>
        /// <param name="initialPosition"></param>
        /// <param name="finalPosition"></param>
        /// <returns></returns>
        public static Vector3 CalculateProjectileVelocity2D(int numOfPoints, float timeInterval, float angle, Vector3 initialPosition, Vector3 finalPosition)
        {
            Vector3 velocity = new Vector3();
            float t = numOfPoints * timeInterval;
            velocity.x = (finalPosition.x - initialPosition.x) / (Mathf.Cos(angle) * t);
            float topYTerm = (finalPosition.y - initialPosition.y) + 0.5f * Physics.gravity.y * t * t;
            velocity.y = topYTerm / (Mathf.Sin(angle) * t);
            return velocity;
        }

        #endregion

        #region Random Mathematics Funcions

        public static float Map(float outputMin, float outputMax, float inputMin, float inputMax, float value)
        {
            if (value >= inputMax)
            {
                return outputMax;
            }
            else if (value <= inputMin)
            {
                return outputMin;
            }

            return (outputMax - outputMin) * ((value - inputMin) / (inputMax - inputMin)) + outputMin;
        }

        public static float GetPercentageOf(float percentage, float number)
        {
            percentage /= 100;
            return number - (number * percentage);
        }

        #endregion Random Mathematics Funcions

        #region Debug Utility Functions


        public static void Error(string msg)
        {
            Debug.LogError(msg);
        }

        public static void Warning<T>(T msg)
        {
            Debug.LogWarning(DateTime.Now + ": " + msg.ToString());
        }

        public static string Log<T>(T msg)
        {
            string msgStr = DateTime.Now + ": " + msg.ToString();
            
            if (!Playfab.VerboseModeEnabled)
            {
                Debug.Log(msgStr);
            }
            
            return msgStr;
        }

        public static void LogListContent<T>(string msg, List<T> list)
        {
            string contents = PrintListContent(list);
            Log(msg + contents);
        }

        public static string LogListContent<T>(List<T> list)
        {
            string contents = PrintListContent(list);
            return Log(contents);
        }

        public static void LogDictContent<Tkey, TVal>(Dictionary<Tkey, TVal> dict)
        {
            string dictContents = "";
            foreach (Tkey key in dict.Keys)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                dictContents += $"Key = {key.ToString()}, Value = {dict[key].ToString()}"+"\n";
            }
            Log(dictContents);
        }

        public static string PrintDictContent<Tkey, TVal>(IDictionary<Tkey, TVal> dict)
        {
            string dictContents = "";
            foreach (Tkey key in dict.Keys)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                dictContents += $"Key = {key.ToString()}, Value = {dict[key].ToString()}" + "\n";
            }
            return dictContents;
        }

        public static string PrintDictContent(IDictionary dict)
        {
            string dictContents = "";
            foreach (var key in dict.Keys)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                dictContents += $"Key = {key.ToString()}, Value = {dict[key].ToString()}" + "\n";
            }
            return dictContents;
        }

        public static string PrintListContent<T>(List<T> list)
        {
            string contents = "";
            if(list != null)
            {
                contents = String.Join(", ", list);
            }
            else
            {
                contents = "Supplied List is null";
            }
            
            /*foreach (T element in list)
            {
                contents += element.ToString() + " ";
            }*/

            return contents;
        }

        public static void Print(string msg)
        {
            Debug.Log(msg);
        }

        public static void CatchException(Exception e)
        {
            Debug.LogWarning(e.Source);
            Debug.LogWarning(e.Message);
            Debug.LogWarning(e.StackTrace);
            Debug.LogWarning(e.InnerException);
        }

        public static string PrintObjectProperties<T>(T src)
        {
            Type type = typeof(T);

            PropertyInfo[] propertyInfo = type.GetProperties();
            string res = "";
            foreach (PropertyInfo pInfo in propertyInfo)
            {
                string val = type.GetProperty(pInfo.Name)?.GetValue(src, null)?.ToString();
                if (!String.IsNullOrEmpty(val))
                {
                    res += pInfo.Name + ": " + val + "\n";
                }
            }

            return Log(res);
        }

        public static string PrintObjectFields<T>(T src)
        {
            Type type = typeof(T);

            FieldInfo[] fieldInfo = type.GetFields();
            string fieldContent = "";

            foreach(FieldInfo field in fieldInfo)
            {
                fieldContent += field.Name + ": " + field.GetValue(src) + "\n";
            }

            return Log(fieldContent);
        }
        #endregion

        #region Misc
        public static Vector3 GetMouseWorldPosition()
        {

            return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f));
        }

        public static Vector3 GetMouseWorldPosition(Camera cam)
        {
            return cam.ScreenToWorldPoint(Input.mousePosition);
        }


        public static TextMesh CreateWorldText(string text, Transform parent, Vector3 localpos, TextAnchor textAnchor = TextAnchor.MiddleCenter, TextAlignment textAlignment = TextAlignment.Center, int fontsize = 40)
        {
            GameObject gObj = new GameObject("World_Text", typeof(TextMesh));
            Transform transform = gObj.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localpos;
            TextMesh textMesh = gObj.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontsize;

            textMesh.color = Color.white;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = 5;
            return textMesh;
        }

        public static Color RandomColor()
        {
            return colors[UnityEngine.Random.Range(0, colors.Length)];
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T CastObject<T>(object objToCast)
        {
            return (T)objToCast;
        }

        public static IEnumerator Timer(int seconds)
        {
            HelperFunctions.Log("Waiting for " + seconds + " seconds");
            yield return new WaitForSeconds(seconds);
        }

        public static IEnumerator Timer(float seconds)
        {
            //HelperFunctions.Log("Waiting for " + seconds + " seconds");
            yield return new WaitForSeconds(seconds);
        }


        public static void Shuffle<T>(this IList<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void LoadScene(SceneNames s)
        {
            SceneManager.LoadScene(s.ToString());
        }

        public static List<int> CountDigit(int num)
        {
            List<int> digits = new List<int>();
            while (num >= 10)
            {
                digits.Add(num % 10);
                num /= 10;
            }

            digits.Add(num);
            digits.Reverse();
            return digits;
        }
        #endregion
    }
}