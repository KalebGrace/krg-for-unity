using System.Text;
using UnityEngine;

namespace _0G.Legacy
{
    /// <summary>
    /// G.U.cs is a partial class of G (G.cs).
    /// </summary>
    partial class G
    {
        /// <summary>
        /// U: The Utilities Class (G.U.cs).
        /// This contains static methods and properties that can be used in edit mode as well as during runtime;
        /// a necessity for any functionality that is required before G and its Managers are instanced and fully set up.
        /// </summary>
        public static class U
        {
            // CONSTANTS

            public const string DEBUG_MODE_KEY = "0G.Legacy.DebugMode";
            public const string EDITOR_MUSIC_KEY = "0G.Legacy.EditorMusic";

            private const string FORMAT_MAGIC_STRING = "{0";

            // STATIC FIELDS

            private static StringBuilder s_InfoStringBuilder = new StringBuilder();

            // COMPOUND PROPERTIES

            private static GameObjectBody s_EditModePlayerCharacter;

            public static GameObjectBody EditModePlayerCharacter
            {
                get
                {
                    if (s_EditModePlayerCharacter == null)
                    {
                        foreach (var body in FindObjectsOfType<GameObjectBody>())
                        {
                            if (body.IsPlayerCharacter)
                            {
                                s_EditModePlayerCharacter = body;

                                break;
                            }
                        }
                    }

                    return s_EditModePlayerCharacter;
                }
            }

            // SHORTCUT PROPERTIES

#if UNITY_EDITOR
            public static bool IsDebugMode => UnityEditor.EditorPrefs.GetBool(DEBUG_MODE_KEY, false);
            public static bool IsEditorMusicEnabled => UnityEditor.EditorPrefs.GetBool(EDITOR_MUSIC_KEY, true);
#else
            public static bool IsDebugMode => false;
            public static bool IsEditorMusicEnabled => true;
#endif

            // PLAY MODE? EDIT MODE?

            public static bool IsPlayMode(Object obj) // Is Unity running in play mode or built application?
            {
                return Application.IsPlaying(obj);
            }

            public static bool IsPlayMode() // Same, but if no UnityEngine object.
            {
                return Application.isPlaying;
            }

            public static bool IsEditMode(Object obj) // Is Unity running in edit mode?
            {
                return !Application.IsPlaying(obj);
            }

            public static bool IsEditMode() // Same, but if no UnityEngine object.
            {
                return !Application.isPlaying;
            }

            // IF

            public static void If(bool condition, System.Action onTrue, System.Action onFalse)
            {
                if (condition)
                {
                    onTrue?.Invoke();
                }
                else
                {
                    onFalse?.Invoke();
                }
            }

            // NEW

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
                    Err("Null prefab/original supplied for new object on {0}.", parent.name);
                    return null;
                }

                T clone = Instantiate(prefab, parent);

                clone.name = prefab.name; //remove "(Clone)" from name

                return clone;
            }

            // ERR

            /// <summary>
            /// Log an error with the specified message and optional arguments.
            /// </summary>
            /// <param name="message">Message, or Format string (if containing "{0").</param>
            /// <param name="args">Arguments, with first object as Context (UnityEngine.Object only).</param>
            public static void Err(string message, params object[] args)
            {
                if (message.Contains(FORMAT_MAGIC_STRING))
                {
                    message = string.Format(message, args);
                }

                message += "\n" + GetInfo(args);

                if (args.Length > 0)
                {
                    Debug.LogError(message, args[0] as Object);
                }
                else
                {
                    Debug.LogError(message);
                }
            }

            // WARN

            /// <summary>
            /// Log a warning with the specified message and optional arguments.
            /// </summary>
            /// <param name="message">Message, or Format string (if containing "{0").</param>
            /// <param name="args">Arguments, with first object as Context (UnityEngine.Object only).</param>
            public static void Warn(string message, params object[] args)
            {
                if (message.Contains(FORMAT_MAGIC_STRING))
                {
                    message = string.Format(message, args);
                }

                message += "\n" + GetInfo(args);

                if (args.Length > 0)
                {
                    Debug.LogWarning(message, args[0] as Object);
                }
                else
                {
                    Debug.LogWarning(message);
                }
            }

            /// <summary>
            /// Same as Warn(), but meant for unfinished programming tasks.
            /// </summary>
            public static void Todo(string message, params object[] args)
            {
                Warn("TODO: " + message, args);
            }

            // LOG

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
                if (message.Contains(FORMAT_MAGIC_STRING))
                {
                    LogInnerFormat(logType, message, objs);
                }
                else
                {
                    LogInner(logType, message + "\n" + U.GetInfo(objs));
                }
            }

            // HELPER METHODS

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
                        Unsupported(G.instance, logType);
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
                        Unsupported(G.instance, logType);
                        break;
                }
            }

            public static T Require<T>(T thing) where T : Component
            {
                if (thing == default(T)) throw new RequireException(typeof(T));
                return thing;
            }

            internal static void ErrorOrException(string s, bool throwException)
            {
                if (throwException)
                {
                    throw new System.Exception(s);
                }
                else
                {
                    Err(s);
                }
            }

            public static string GetInfo(params object[] objs)
            {
                int len;
                object o;
                s_InfoStringBuilder.Clear();
                if (objs == null)
                {
                    s_InfoStringBuilder.Append("<NULL LITERAL>");
                }
                else
                {
                    len = objs.Length;
                    if (len == 0)
                    {
                        s_InfoStringBuilder.Append("<ZERO PARAMS>");
                    }
                    else
                    {
                        for (int i = 0; i < len; i++)
                        {
                            o = objs[i];
                            if (i > 0)
                            {
                                s_InfoStringBuilder.Append("\n");
                            }
                            s_InfoStringBuilder.Append("[#");
                            s_InfoStringBuilder.Append(i);
                            s_InfoStringBuilder.Append("] ");
                            s_InfoStringBuilder.Append(o);
                            if (o == null)
                            {
                                s_InfoStringBuilder.Append("<NULL PARAM>");
                            }
                            else
                            {
                                s_InfoStringBuilder.Append(", type ");
                                s_InfoStringBuilder.Append(o.GetType());
#pragma warning disable 0168
                                //as of Unity 5.5.0f3...
                                //the following is the only way to GetInstanceID for a destroyed UnityEngine.Object,
                                //since the UnityEngine.Object will evaluate to null
                                //even though it still technically exists
                                //NOTE: you cannot get "name" from a null
                                //UnityEngine.Object (at least not for a Component)
                                //TODO: using the knowledge from IsNull(), see if this can be fixed
                                try
                                {
                                    s_InfoStringBuilder.Append(", id ");
                                    s_InfoStringBuilder.Append(((Object) o).GetInstanceID());
                                }
                                catch (System.Exception ex) { }
#pragma warning restore 0168
                            }
                        }
                    }
                }
                s_InfoStringBuilder.Append("\n[*] frame ");
                s_InfoStringBuilder.Append(Time.frameCount);
                s_InfoStringBuilder.Append(", sec ");
                s_InfoStringBuilder.Append(Time.realtimeSinceStartup);
                return s_InfoStringBuilder.ToString();
            }

            internal static bool SourceExists(Object source, System.Type t, bool throwException)
            {
                if (IsNull(source))
                {
                    string s = string.Format("An Object must exist in order to require the {0} Component.", t);
                    ErrorOrException(s, throwException);
                    return false;
                }
                return true;
            }

            // ASSERT

            /// <summary>
            /// Assert the specified condition.
            /// </summary>
            /// <param name="condition">The condition.</param>
            /// <returns>If the condition was asserted to be true.</returns>
            public static bool Assert(bool condition)
            {
                Debug.Assert(condition);
                return condition;
            }

            /// <summary>
            /// Assert the specified condition (w/ options).
            /// </summary>
            /// <param name="condition">The condition.</param>
            /// <param name="message">Message, or format (if containing "{0").</param>
            /// <param name="objs">Objects.</param>
            /// <returns>If the condition was asserted to be true.</returns>
            public static bool Assert(bool condition, string message, params object[] objs)
            {
                if (condition) return true;

                if (message.Contains(FORMAT_MAGIC_STRING))
                {
                    Debug.AssertFormat(condition, message, objs);
                }
                else
                {
                    Debug.Assert(condition, message + "\n" + GetInfo(objs));
                }
                return condition;
            }

            // GUARANTEE

            /// <summary>
            /// Guarantee that the specified Component type T exists on the specified source GameObject.
            /// If it doesn't exist, it will be created.
            /// </summary>
            /// <param name="source">Source.</param>
            /// <typeparam name="T">The 1st type parameter.</typeparam>
            public static T Guarantee<T>(GameObject source) where T : Component
            {
                // TODO: if called via ExecuteAlways, need call to UnityEditor.Undo.RecordObject
                var c = source.GetComponent<T>();
                // NOTE: null-coalescing operator does not work properly for this
                if (c == null)
                {
                    c = source.AddComponent<T>();
                }
                return c;
            }

            public static T Guarantee<T>(Component source) where T : Component // Same, but for component source.
            {
                return Guarantee<T>(source.gameObject);
            }

            // IS NULL ?

            /// <summary>
            /// Determines if the specified object is null.
            /// This is necessary for some cases where a missing Unity object reference does not "== null".
            /// </summary>
            /// <returns><c>true</c> if the specified object is null; otherwise, <c>false</c>.</returns>
            /// <param name="obj">Object.</param>
            public static bool IsNull(object obj)
            {
                return obj == null || obj.Equals(null);
            }

            // PREVENT

            /// <summary>
            /// Prevent the specified condition.
            /// </summary>
            /// <param name="condition">The condition.</param>
            /// <returns>If the condition was prevented (asserted to be false).</returns>
            public static bool Prevent(bool condition)
            {
                return Assert(!condition);
            }

            /// <summary>
            /// Prevent the specified condition (w/ options).
            /// </summary>
            /// <param name="condition">The condition.</param>
            /// <param name="message">Message, or format (if containing "{0").</param>
            /// <param name="objs">Objects.</param>
            /// <returns>If the condition was prevented (asserted to be false).</returns>
            public static bool Prevent(bool condition, string message, params object[] objs)
            {
                return Assert(!condition, message, objs);
            }

            // REQUIRE

            /// <summary>
            /// REQUIRE NON-NULL OBJECT:
            /// Require the specified object to be assigned to this Component (etc.),
            /// and log an error / throw an exception if the object is null.
            /// Message format: "A {name} must be assigned to {owner}."
            /// </summary>
            /// <param name="obj">Object.</param>
            /// <param name="name">Descriptive name of the object used for logging purposes.</param>
            /// <param name="owner">Descriptive name of the owner used for logging purposes.</param>
            /// <param name="throwException">If set to <c>true</c> throw exception.</param>
            public static bool Require(
                object obj,
                string name,
                string owner = "this Component",
                bool throwException = true
            )
            {
                return Require(obj, name, null, owner, throwException);
            }

            /// <summary>
            /// REQUIRE NON-NULL OBJECT on SPECIFIED COMPONENT/OBJECT:
            /// Require the specified object to be assigned to this Component (or object; provided in source),
            /// and log an error / throw an exception if the object is null.
            /// Message format: "A {name} must be assigned to {owner} ({source name})."
            /// </summary>
            /// <param name="obj">Object.</param>
            /// <param name="name">Descriptive name of the object used for logging purposes.</param>
            /// <param name="source">Source Component, GameObject, or other UnityEngine.Object (the owner).</param>
            /// <param name="owner">Descriptive name of the owner used for logging purposes.</param>
            /// <param name="throwException">If set to <c>true</c> throw exception.</param>
            public static bool Require(
                object obj,
                string name,
                UnityEngine.Object source,
                string owner = "this Component",
                bool throwException = true
            )
            {
                if (IsNull(obj))
                {
                    string s;
                    if (source == null)
                    {
                        s = string.Format("A {0} must be assigned to {1}.", name, owner);
                    }
                    else
                    {
                        s = string.Format("A {0} must be assigned to {1} ({2}).", name, owner, source.name);
                    }
                    ErrorOrException(s, throwException);
                    return false;
                }
                return true;
            }

            /// <summary>
            /// Require a minimum count of specified Component type to exist
            /// on the specified source Component's GameObject.
            /// </summary>
            /// <param name="source">Source Component.</param>
            /// <param name="minCount">Minimum count of specified Component type.</param>
            /// <param name="throwException">If set to <c>true</c> throw exception.</param>
            /// <typeparam name="T">The required Component type.</typeparam>
            public static T[] Require<T>(
                Component source,
                int minCount,
                bool throwException = true
            ) where T : Component
            {
                if (!SourceExists(source, typeof(T), throwException)) return null;
                T[] comps = source.GetComponents<T>();
                int len = IsNull(comps) ? 0 : comps.Length;
                if (len < minCount)
                {
                    string s = string.Format("{0} or more {1} Components must exist on the {2}'s {3} GameObject." +
                        " But it has {4} of them.", minCount, typeof(T), source.GetType(), source.name, len);
                    ErrorOrException(s, throwException);
                    return null;
                }
                return comps;
            }

            /// <summary>
            /// Require a minimum count of specified Component type to exist
            /// on the specified source GameObject.
            /// </summary>
            /// <param name="source">Source GameObject.</param>
            /// <param name="minCount">Minimum count of specified Component type.</param>
            /// <param name="throwException">If set to <c>true</c> throw exception.</param>
            /// <typeparam name="T">The required Component type.</typeparam>
            public static T[] Require<T>(
                GameObject source,
                int minCount,
                bool throwException = true
            ) where T : Component
            {
                if (!SourceExists(source, typeof(T), throwException)) return null;
                T[] comps = source.GetComponents<T>();
                int len = IsNull(comps) ? 0 : comps.Length;
                if (len < minCount)
                {
                    string s = string.Format("{0} or more {1} Components must exist on the {2} GameObject." +
                        " But it has {4} of them.", minCount, typeof(T), source.name, len);
                    ErrorOrException(s, throwException);
                    return null;
                }
                return comps;
            }

            // UNSUPPORTED

            /// <summary>
            /// Logs an error regarding an unsupported condition on the specified source.
            /// </summary>
            /// <param name="source">Source.</param>
            public static void Unsupported(Component source)
            {
                string message = string.Format("Unsupported condition for {0} Component on {1} GameObject.",
                    source.GetType(),
                    source.name);

                Err(message, source);
            }

            /// <summary>
            /// Logs an error regarding an unsupported condition on the specified source.
            /// </summary>
            /// <param name="source">Source.</param>
            /// <param name="originalMessage">Message.</param>
            public static void Unsupported(Component source, string originalMessage)
            {
                string message = string.Format("Unsupported condition for {0} Component on {1} GameObject: {2}.",
                    source.GetType(),
                    source.name,
                    originalMessage);

                Err(message, source);
            }

            /// <summary>
            /// Logs an error regarding an unsupported enum on the specified source.
            /// </summary>
            /// <param name="source">Source.</param>
            /// <param name="unsupportedEnum">Unsupported enum.</param>
            public static void Unsupported(Component source, System.Enum unsupportedEnum)
            {
                string message = string.Format("Unsupported {0}: {1} for {2} Component on {3} GameObject.",
                    unsupportedEnum.GetType(),
                    unsupportedEnum,
                    source.GetType(),
                    source.name);

                Err(message, source);
            }

            /// <summary>
            /// Logs an error regarding an unsupported enum on the specified source.
            /// </summary>
            /// <param name="source">Source.</param>
            /// <param name="unsupportedEnum">Unsupported enum.</param>
            public static void Unsupported(object source, System.Enum unsupportedEnum)
            {
                string message = string.Format("Unsupported {0}: {1} for {2} object.",
                    unsupportedEnum.GetType(),
                    unsupportedEnum,
                    source.GetType());

                Err(message, source);
            }
        }
    }
}