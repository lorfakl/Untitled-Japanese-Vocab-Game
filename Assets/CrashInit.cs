using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Import Firebase and Crashlytics
using Firebase;
using Firebase.Crashlytics;

namespace Utilities.Logging
{
    public class CrashInit: MonoBehaviour
    {
        private static bool _isCrashlyticsInitialized = false;

        public static bool IsCrashlyticsInitialized
        {
            get { return _isCrashlyticsInitialized; }
        }

        private void Awake()
        {
            if (!_isCrashlyticsInitialized)
            {
                Initialize();

            }
        }

        // Use this for initialization
        private static void Initialize()
        {
            // Initialize Firebase
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    // Crashlytics will use the DefaultInstance, as well;
                    // this ensures that Crashlytics is initialized.
                    Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;

                    // When this property is set to true, Crashlytics will report all
                    // uncaught exceptions as fatal events. This is the recommended behavior.
                    Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                    _isCrashlyticsInitialized = true;
                    FirebaseApp.LogLevel = Firebase.LogLevel.Verbose;
                    // Set a flag here for indicating that your project is ready to use Firebase.
                    Debug.Log("Crashilytics initialized");
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }

    }
}
