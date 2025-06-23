using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Handles crash reporting and exception management.
    /// </summary>
    public class CrashReport : MonoBehaviour
    {
        [SerializeField] private bool logExceptionsToFile = true;
        [SerializeField] private bool sendCrashReports = true;
        [SerializeField] private string crashReportDirectory = "CrashReports";

        private static CrashReport instance;
        private static List<ExceptionEntry> recentExceptions = new List<ExceptionEntry>();
        private const int MaxStoredExceptions = 10;

        /// <summary>
        /// Struct to store exception information with timestamp.
        /// </summary>
        private struct ExceptionEntry
        {
            public DateTime Timestamp { get; }
            public Exception Exception { get; }
            public bool WasSilent { get; }

            public ExceptionEntry(Exception exception, bool wasSilent)
            {
                Timestamp = DateTime.Now;
                Exception = exception;
                WasSilent = wasSilent;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            // Set up global exception handler
            Application.logMessageReceived += HandleLogMessage;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= HandleLogMessage;

            if (instance == this)
            {
                instance = null;
            }
        }

        /// <summary>
        /// Handles Unity log messages and captures exceptions.
        /// </summary>
        private void HandleLogMessage(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                // Unity's internal exception handling already captured this
                // We just need to make sure it's in our record
                if (!string.IsNullOrEmpty(stackTrace))
                {
                    // Create a basic exception to represent this
                    var exception = new Exception(logString + "\n" + stackTrace);
                    AddExceptionToRecord(exception, false);
                }
            }
        }

        /// <summary>
        /// Adds an exception to the internal record.
        /// </summary>
        private static void AddExceptionToRecord(Exception exception, bool silent)
        {
            recentExceptions.Add(new ExceptionEntry(exception, silent));

            // Keep only the most recent exceptions
            if (recentExceptions.Count > MaxStoredExceptions)
            {
                recentExceptions.RemoveAt(0);
            }
        }

        /// <summary>
        /// Triggers exception handling for the given exception.
        /// </summary>
        /// <param name="exception">The exception to process</param>
        /// <param name="silent">If true, suppresses visual feedback about the exception</param>
        public static void TriggerException(Exception exception, bool silent = false)
        {
            if (exception == null)
                return;

            // Log to Unity console if not silent
            if (!silent)
            {
                Debug.LogException(exception);
            }

            // Add to our internal record
            AddExceptionToRecord(exception, silent);

            // If we have an instance, use its configuration to handle the exception
            if (instance != null)
            {
                if (instance.logExceptionsToFile)
                {
                    LogExceptionToFile(exception, silent);
                }

                if (instance.sendCrashReports && !silent)
                {
                    // This would be implemented with your crash reporting service
                    SendCrashReport(exception);
                }
            }
        }

        /// <summary>
        /// Logs an exception to a file.
        /// </summary>
        private static void LogExceptionToFile(Exception exception, bool silent)
        {
            if (instance == null || string.IsNullOrEmpty(instance.crashReportDirectory))
                return;

            try
            {
                string directory = Path.Combine(Application.persistentDataPath, instance.crashReportDirectory);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string filename = $"crash_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string filePath = Path.Combine(directory, filename);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Crash Report - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"Silent: {silent}");
                sb.AppendLine($"Application Version: {Application.version}");
                sb.AppendLine($"Unity Version: {Application.unityVersion}");
                sb.AppendLine($"Platform: {Application.platform}");
                sb.AppendLine($"System Language: {Application.systemLanguage}");
                sb.AppendLine($"Device Model: {SystemInfo.deviceModel}");
                sb.AppendLine($"Device Name: {SystemInfo.deviceName}");
                sb.AppendLine($"OS: {SystemInfo.operatingSystem}");
                sb.AppendLine($"CPU: {SystemInfo.processorType}");
                sb.AppendLine($"Memory: {SystemInfo.systemMemorySize} MB");
                sb.AppendLine($"Graphics: {SystemInfo.graphicsDeviceName}");
                sb.AppendLine("\n------ Exception Details ------");
                sb.AppendLine($"Type: {exception.GetType().FullName}");
                sb.AppendLine($"Message: {exception.Message}");
                sb.AppendLine($"Stack Trace: {exception.StackTrace}");

                // Include inner exception details if available
                var innerException = exception.InnerException;
                if (innerException != null)
                {
                    sb.AppendLine("\n------ Inner Exception ------");
                    sb.AppendLine($"Type: {innerException.GetType().FullName}");
                    sb.AppendLine($"Message: {innerException.Message}");
                    sb.AppendLine($"Stack Trace: {innerException.StackTrace}");
                }

                File.WriteAllText(filePath, sb.ToString());
            }
            catch (Exception ex)
            {
                // Don't use Debug.LogException here to avoid potential infinite recursion
                Debug.LogError($"Failed to write crash report to file: {ex.Message}");
            }
        }

        /// <summary>
        /// Sends a crash report to a remote service.
        /// </summary>
        private static void SendCrashReport(Exception exception)
        {
            // This would be implemented with your crash reporting service
            // For example, using a service like Firebase Crashlytics, Sentry, etc.
            // TODO: Implement crash reporting integration
        }

        /// <summary>
        /// Gets a list of recent exceptions for debugging purposes.
        /// </summary>
        public static List<Exception> GetRecentExceptions()
        {
            List<Exception> exceptions = new List<Exception>();
            foreach (var entry in recentExceptions)
            {
                exceptions.Add(entry.Exception);
            }
            return exceptions;
        }
    }
}