using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    /// <summary>
    /// G.Methods.cs is a partial class of G (G.cs).
    /// This contains static methods that can be used both in the editor (i.e. edit mode) and during runtime;
    /// a necessity for any functionality that is required before G and its Managers are instanced and fully set up.
    /// </summary>
    partial class G
    {

#region private constants

        const string _formatMagicString = "{0";

#endregion

#region New

        /// <summary>
        /// Create a new instance of the specified prefab on the specified parent (use *null* for hierarchy root).
        /// This is essentially the same as Object.Instantiate, but allows for additional functionality.
        /// </summary>
        /// <param name="prefab">Prefab (original).</param>
        /// <param name="parent">Parent.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T New<T>(T prefab, Transform parent) where T : Object
        {
            if (prefab == null)
            {
                G.U.Error("Null prefab/original supplied for new object on {0}.", parent.name);
                return null;
            }

            T clone = Object.Instantiate(prefab, parent);

            clone.name = prefab.name; //remove "(Clone)" from name

            return clone;
        }

#endregion

#region End

        /// <summary>
        /// End (destroy) the specified object.
        /// This is essentially the same as Object.Destroy, but allows for additional functionality.
        /// </summary>
        /// <param name="obj">Object.</param>
        public static void End(GameObject obj)
        {
            if (EndNull(obj)) return;
            obj.CallInterfaces<IEnd>(MarkAsEnded);
            Object.Destroy(obj);
        }

        /// <summary>
        /// End (destroy) the specified object.
        /// This is essentially the same as Object.Destroy, but allows for additional functionality.
        /// </summary>
        /// <param name="obj">Object.</param>
        public static void End(IEnd obj)
        {
            if (EndNull(obj)) return;
            MarkAsEnded(obj);
            Object.Destroy(obj.end.owner);
        }

        /// <summary>
        /// End (destroy) the specified object.
        /// This is essentially the same as Object.Destroy, but allows for additional functionality.
        /// </summary>
        /// <param name="obj">Object.</param>
        public static void End(Object obj)
        {
            if (EndNull(obj)) return;
            //
            Object.Destroy(obj);
        }

        static bool EndNull(object o)
        {
            if (o == null)
            {
                G.U.Warning("The object you wish to end is null.");
                return true;
            }
            else
            {
                return false;
            }
        }

        static void MarkAsEnded(IEnd iEnd)
        {
            iEnd.end.MarkAsEnded();
        }

#endregion

#region Err

        /// <summary>
        /// Log an error with the specified message.
        /// </summary>
        /// <param name="message">Message.</param>
        public static void Err(object message)
        {
            Debug.LogError(message);
        }

        /// <summary>
        /// Log an error with the specified message and arguments.
        /// </summary>
        /// <param name="message">Message, or Format string (if containing "{0").</param>
        /// <param name="args">Arguments, or Context (if no format - first object as UnityEngine.Object only).</param>
        public static void Err(object message, params object[] args)
        {
            var s = message.ToString();
            if (s.Contains(_formatMagicString))
            {
                Debug.LogErrorFormat(s, args);
            }
            else
            {
                Debug.LogError(message, args[0] as Object);
            }
        }

        /// <summary>
        /// Log an error with the specified context, format, and arguments.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="format">Format.</param>
        /// <param name="args">Arguments.</param>
        public static void Err(GameObject context, string format, params object[] args)
        {
            Debug.LogErrorFormat(context, format, args);
        }

        /// <summary>
        /// Log an error with the specified context, format, and arguments.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="format">Format.</param>
        /// <param name="args">Arguments.</param>
        public static void Err(MonoBehaviour context, string format, params object[] args)
        {
            Debug.LogErrorFormat(context, format, args);
        }

        /// <summary>
        /// Log an error with the specified context, format, and arguments.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="format">Format.</param>
        /// <param name="args">Arguments.</param>
        public static void Err(ScriptableObject context, string format, params object[] args)
        {
            Debug.LogErrorFormat(context, format, args);
        }

#endregion

#region Log

        static float _logLastTime;

        /// <summary>
        /// Log the time.
        /// </summary>
        public static void Log()
        {
            var t = UnityEngine.Time.realtimeSinceStartup;
            var d = t - _logLastTime;
            _logLastTime = t;
            Debug.LogFormat("{0:00.0000} seconds since last log. Current time is {1:00,000.0000}.", d, t);
        }

        /// <summary>
        /// Log the specified objects.
        /// </summary>
        /// <param name="objs">Objects.</param>
        public static void Log(params object[] objs)
        {
            Log(LogType.Log, objs);
        }

        /// <summary>
        /// Log the specified message and optional objects.
        /// </summary>
        /// <param name="message">Message, or format (if containing "{0").</param>
        /// <param name="objs">Objects.</param>
        public static void Log(string message, params object[] objs)
        {
            Log(LogType.Log, message, objs);
        }

        /// <summary>
        /// Log the specified objects using the specified log type.
        /// </summary>
        /// <param name="logType">Log type.</param>
        /// <param name="objs">Objects.</param>
        public static void Log(LogType logType, params object[] objs)
        {
            LogInner(logType, U.GetInfo(objs));
        }

        /// <summary>
        /// Log the specified message and optional objects using the specified log type.
        /// </summary>
        /// <param name="logType">Log type.</param>
        /// <param name="message">Message, or format (if containing "{0").</param>
        /// <param name="objs">Objects.</param>
        public static void Log(LogType logType, string message, params object[] objs)
        {
            if (message.Contains(_formatMagicString))
            {
                LogInnerFormat(logType, message, objs);
            }
            else
            {
                LogInner(logType, message + "; " + U.GetInfo(objs));
            }
        }

        static void LogInner(LogType logType, string message)
        {
            switch (logType)
            {
                case LogType.Assert:
                    Debug.LogAssertion(message);
                    break;
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                case LogType.Exception:
                    Debug.LogException(new System.Exception(message));
                    break;
                case LogType.Log:
                    Debug.Log(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                default:
                    G.U.Unsupported(G.instance, logType);
                    break;
            }
        }

        static void LogInnerFormat(LogType logType, string format, params object[] args)
        {
            switch (logType)
            {
                case LogType.Assert:
                    Debug.LogAssertionFormat(format, args);
                    break;
                case LogType.Error:
                    Debug.LogErrorFormat(format, args);
                    break;
                case LogType.Exception:
                    Debug.LogException(new System.Exception(string.Format(format, args)));
                    break;
                case LogType.Log:
                    Debug.LogFormat(format, args);
                    break;
                case LogType.Warning:
                    Debug.LogWarningFormat(format, args);
                    break;
                default:
                    G.U.Unsupported(G.instance, logType);
                    break;
            }
        }

#endregion

        public static T Require<T>(T thing) where T : Component
        {
            if (thing == default(T)) throw new RequireException(typeof(T));
            return thing;
        }
    }
}
