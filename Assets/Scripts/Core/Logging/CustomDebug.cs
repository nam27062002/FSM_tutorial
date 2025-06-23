#if UNITY_EDITOR
#define USE_CUSTOM_LOG
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
namespace Core
{
    /// <summary>
    /// Categories for log messages to help with filtering and organization.
    /// </summary>
    public enum LogCategory
    {
        None = 0,
        Streaming,
        Audio,
        AI,
        Quest,
        Gameplay,
        UI,
        Editor,
        Animation,
        FX,
        LevelDesign,
        Art,
        LoadSave,
        Engine,
        Cinematic,
        CineRecorder,
        RuntimeInstantiate
    }

    /// <summary>
    /// Additional flags that can be applied to log messages.
    /// </summary>
    [Flags]
    public enum LogFlags
    {
        None = 0, 
        DataError = 1 << 0
    }
    
    public static class CustomDebug
    {
        /// <summary>
        /// Represents the severity level of a log message.
        /// </summary>
        public enum LogSeverity
        {
            /// <summary>Informational message</summary>
            Info,
            /// <summary>Warning message</summary>
            Warning,
            /// <summary>Error message</summary>
            Error
        }
        
        /// <summary>
        /// Logs an informational message with the specified user roles and category.
        /// </summary>
        /// <param name="roles">The user roles this message is relevant to</param>
        /// <param name="category">The category of the log message</param>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void Log(UserRole roles, LogCategory category, string message, Object context = null)
        {
            InternalLog(roles, category, LogSeverity.Info, message, context, LogFlags.None);
        }

        /// <summary>
        /// Logs an informational message with the specified user roles.
        /// </summary>
        /// <param name="roles">The user roles this message is relevant to</param>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void Log(UserRole roles, string message, Object context = null)
        {
            Log(roles, LogCategory.None, message, context);
        }

        /// <summary>
        /// Logs an informational message with the specified category.
        /// </summary>
        /// <param name="category">The category of the log message</param>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void Log(LogCategory category, string message, Object context = null)
        {
            Log(UserRole.All, category, message, context);
        }

        /// <summary>
        /// Logs an informational message visible to all user roles.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void Log(string message, Object context = null)
        {
            Log(UserRole.All, LogCategory.None, message, context);
        }
        
        /// <summary>
        /// Logs a warning message with the specified user roles and category.
        /// </summary>
        /// <param name="roles">The user roles this message is relevant to</param>
        /// <param name="category">The category of the log message</param>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void LogWarning(UserRole roles, LogCategory category, string message, Object context = null)
        {
            InternalLog(roles, category, LogSeverity.Warning, message, context, LogFlags.None);
        }

        /// <summary>
        /// Logs a warning message with the specified user roles.
        /// </summary>
        /// <param name="roles">The user roles this message is relevant to</param>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void LogWarning(UserRole roles, string message, Object context = null)
        {
            LogWarning(roles, LogCategory.None, message, context);
        }

        /// <summary>
        /// Logs a warning message with the specified category.
        /// </summary>
        /// <param name="category">The category of the log message</param>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void LogWarning(LogCategory category, string message, Object context = null)
        {
            LogWarning(UserRole.All, category, message, context);
        }

        /// <summary>
        /// Logs a warning message visible to all user roles.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void LogWarning(string message, Object context = null)
        {
            LogWarning(UserRole.All, LogCategory.None, message, context);
        }
        
        /// <summary>
        /// Logs an error message with the specified user roles and category.
        /// </summary>
        /// <param name="roles">The user roles this message is relevant to</param>
        /// <param name="category">The category of the log message</param>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void LogError(UserRole roles, LogCategory category, string message, Object context = null)
        {
            InternalLog(roles, category, LogSeverity.Error, message, context, LogFlags.None);
        }

        /// <summary>
        /// Logs an error message with the specified user roles.
        /// </summary>
        /// <param name="roles">The user roles this message is relevant to</param>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void LogError(UserRole roles, string message, Object context = null)
        {
            LogError(roles, LogCategory.None, message, context);
        }

        /// <summary>
        /// Logs an error message with the specified category.
        /// </summary>
        /// <param name="category">The category of the log message</param>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void LogError(LogCategory category, string message, Object context = null)
        {
            LogError(UserRole.All, category, message, context);
        }

        /// <summary>
        /// Logs an error message visible to all user roles.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void LogError(string message, Object context = null)
        {
            LogError(UserRole.All, LogCategory.None, message, context);
        }
        
        /// <summary>
        /// Logs a data error message with the specified user roles and category.
        /// </summary>
        /// <param name="roles">The user roles this message is relevant to</param>
        /// <param name="category">The category of the log message</param>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void LogDataError(UserRole roles, LogCategory category, string message, Object context = null)
        {
            InternalLog(roles, category, LogSeverity.Error, message, context, LogFlags.DataError);
        }

        /// <summary>
        /// Logs a data warning message with the specified user roles and category.
        /// </summary>
        /// <param name="roles">The user roles this message is relevant to</param>
        /// <param name="category">The category of the log message</param>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void LogDataWarning(UserRole roles, LogCategory category, string message, Object context = null)
        {
            InternalLog(roles, category, LogSeverity.Warning, message, context, LogFlags.DataError);
        }

        /// <summary>
        /// Logs a data error message with the specified user roles.
        /// </summary>
        /// <param name="roles">The user roles this message is relevant to</param>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void LogDataError(UserRole roles, string message, Object context = null)
        {
            LogDataError(roles, LogCategory.None, message, context);
        }

        /// <summary>
        /// Logs a data error message with the specified category.
        /// </summary>
        /// <param name="category">The category of the log message</param>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void LogDataError(LogCategory category, string message, Object context = null)
        {
            LogDataError(UserRole.All, category, message, context);
        }

        /// <summary>
        /// Logs a data error message visible to all user roles.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void LogDataError(string message, Object context = null)
        {
            LogDataError(UserRole.All, LogCategory.None, message, context);
        }
        
        /// <summary>
        /// Logs an exception and triggers the crash reporting system.
        /// </summary>
        /// <param name="exception">The exception to log</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void LogException(Exception exception)
        {
            CrashReport.TriggerException(exception);
        }

        /// <summary>
        /// Logs a message using the preferred logging system available.
        /// This method will try to use the primary logging system first,
        /// falling back to alternative logging systems if available.
        /// </summary>
        /// <param name="message">The message to log</param>
        [Conditional("USE_CUSTOM_LOG")]
        [Conditional("USE_ALKAWALOGGER")]
        public static void LogFinal(string message)
        {
#if USE_CUSTOM_LOG  // Primary logging system
            Log(message);
#elif USE_ALKAWALOGGER  // Fallback logging system
            AlkawaLogger.Log(message);
#endif
        }


        /// <summary>
        /// Internal method that handles the actual logging functionality.
        /// </summary>
        /// <param name="roles">The user roles this message is relevant to</param>
        /// <param name="category">The category of the log message</param>
        /// <param name="severity">The severity level of the message</param>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        /// <param name="flags">Additional flags for the log message</param>
        [Conditional("USE_CUSTOM_LOG")]
        private static void InternalLog(UserRole roles, LogCategory category, LogSeverity severity, string message, Object context, LogFlags flags)
        {
            string formattedMessage = $"{(flags != LogFlags.None ? $"[{flags}] " : "")}{(category != LogCategory.None ? $"[{category}] " : "")}{message}";

            switch (severity)
            {
                case LogSeverity.Info:
                    Debug.Log(formattedMessage, context);
                    break;
                case LogSeverity.Warning:
                    Debug.LogWarning(formattedMessage, context);
                    break;
                case LogSeverity.Error:
                    Debug.LogError(formattedMessage, context);
                    break;
            }

            LogToFile(severity, formattedMessage);
        }
        
        /// <summary>
        /// Writes log messages to file destinations if any are configured.
        /// </summary>
        /// <param name="severity">The severity level of the message</param>
        /// <param name="message">The formatted message to log to file</param>
        [Conditional("USE_CUSTOM_LOG")]
        private static void LogToFile(LogSeverity severity, string message)
        {
#if UNITY_EDITOR && USE_CUSTOM_LOG
            if (fileDestinations.Count == 0)
                return;

            foreach (var destination in fileDestinations)
            {
                try
                {
                    File.AppendAllText(destination, $"[{severity}][{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}{Environment.NewLine}");
                }
                catch(Exception ex)
                {
                    Debug.LogError($"Failed to write to log file: {ex.Message}");
                }
            }
#endif
        }

#if USE_CUSTOM_LOG
        /// <summary>
        /// Stack of log file destinations.
        /// </summary>
        private static readonly Stack<string> fileDestinations = new Stack<string>();

        /// <summary>
        /// Adds a new file destination for log output.
        /// </summary>
        /// <param name="destination">Path to the log file</param>
        [Conditional("USE_CUSTOM_LOG")]
        public static void PushFileOutput(string destination)
        {
            if (string.IsNullOrEmpty(destination))
            {
                Debug.LogWarning("Attempted to push an empty or null log file destination");
                return;
            }

            // Ensure the directory exists
            string directory = Path.GetDirectoryName(destination);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to create log directory: {ex.Message}");
                    return;
                }
            }

            fileDestinations.Push(destination);
        }

        /// <summary>
        /// Removes the most recently added file destination from the stack.
        /// </summary>
        [Conditional("USE_CUSTOM_LOG")]
        public static void PopFileOutput()
        {
            if (fileDestinations.Count > 0)
            {
                fileDestinations.Pop();
            }
            else
            {
                Debug.LogWarning("Attempted to pop from an empty file destination stack");
            }
        }

        /// <summary>
        /// Clears all file destinations.
        /// </summary>
        [Conditional("USE_CUSTOM_LOG")]
        public static void ClearFileOutputs()
        {
            fileDestinations.Clear();
        }
#endif //USE_CUSTOM_LOG
    }
}
