﻿using UnityEngine;

namespace KRG
{
	/// <summary>
	/// G.U.cs is a partial class of G (G.cs).
	/// </summary>
	partial class G
	{
		/// <summary>
		/// U: The Utilities Class (G.U.cs).
		/// This contains static methods that can be used both in the editor (i.e. edit mode) and during runtime;
		/// a necessity for any functionality that is required before G and its Managers are instanced and fully set up.
		/// </summary>
		public static class U
		{
			//=====================//
			//                     //
			//  C O N S T A N T S  //
			//                     //
			//=====================//

			const string FORMAT_MAGIC_STRING = "{0";


			//=========//
			//         //
			//  N E W  // (End, Err, Log, Require)
			//         //
			//=========//

			// TODO: Organize these methods and remove redundancies.


			//=========//
			//         //
			//  O L D  //
			//         //
			//=========//

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


			/// <summary>
			/// End (destroy) the specified object.
			/// This is essentially the same as Object.Destroy, but allows for additional functionality.
			/// </summary>
			/// <param name="obj">Object.</param>
			public static void End(GameObject obj)
			{
				if (EndNull(obj)) return;
				Destroy(obj);
				//TODO: the following does not work properly; fix or remove
				//obj.CallInterfaces<IEnd>(CheckEnded);
			}

			/// <summary>
			/// End (destroy) the specified object.
			/// This is essentially the same as Object.Destroy, but allows for additional functionality.
			/// </summary>
			/// <param name="obj">Object.</param>
			public static void End(IEnd obj)
			{
				if (EndNull(obj)) return;
				Destroy((Object)obj);
				CheckEnded(obj);
			}

			/// <summary>
			/// End (destroy) the specified object.
			/// This is essentially the same as Object.Destroy, but allows for additional functionality.
			/// </summary>
			/// <param name="obj">Object.</param>
			public static void End(Object obj)
			{
				if (EndNull(obj)) return;
				Destroy(obj);
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

			static void CheckEnded(IEnd iEnd)
			{
				if (!iEnd.end.wasInvoked)
				{
					Err("{0} has been destroyed without calling my_end.Invoke() in the OnDestroy method.", iEnd.ToString());
				}
			}


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
				if (s.Contains(FORMAT_MAGIC_STRING))
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


			static void ErrorOrException(string s, bool throwException)
			{
				if (throwException)
				{
					throw new System.Exception(s);
				}
				else
				{
					Error(s);
				}
			}

			public static string GetInfo(params object[] objs)
			{
				int len;
				object o;
				string s = "";
				if (objs == null)
				{
					s = "<NULL LITERAL>";
				}
				else
				{
					len = objs.Length;
					if (len == 0)
					{
						s = "<ZERO PARAMS>";
					}
					else
					{
						for (int i = 0; i < len; i++)
						{
							o = objs[i];
							if (i > 0) s += "; ";
							s += string.Format("[#{0}] {1}", i, o);
							if (o == null)
							{
								s += "<NULL PARAM>";
							}
							else
							{
								s += ", type " + o.GetType();
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
									s += ", id " + ((Object)o).GetInstanceID();
								}
								catch (System.Exception ex)
								{
								}
#pragma warning restore 0168
							}
						}
					}
				}
				s += "; [*] frame " + Time.frameCount + ", sec " + Time.realtimeSinceStartup;
				return s;
			}

			static bool SourceExists(Object source, System.Type t, bool throwException)
			{
				if (IsNull(source))
				{
					string s = string.Format("An Object must exist in order to require the {0} Component.", t);
					ErrorOrException(s, throwException);
					return false;
				}
				return true;
			}


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
				if (message.Contains(FORMAT_MAGIC_STRING))
				{
					Debug.AssertFormat(condition, message, objs);
				}
				else
				{
					Debug.Assert(condition, message + "; " + GetInfo(objs));
				}
				return condition;
			}


			/// <summary>
			/// Log an error with the specified message and optional objects.
			/// </summary>
			/// <param name="message">Message, or format (if containing "{0").</param>
			/// <param name="objs">Objects.</param>
			public static void Error(string message, params object[] objs)
			{
				if (message.Contains(FORMAT_MAGIC_STRING))
				{
					Debug.LogErrorFormat(message, objs);
					//basically the same as G.Err(...)
				}
				else
				{
					Debug.LogError(message + "; " + GetInfo(objs));
					//TODO: migrate this functionality to G.Err(...)
				}
			}


			/// <summary>
			/// Guarantee that the specified Component type T exists on the specified source Component's GameObject.
			/// If it doesn't exist, it will be created.
			/// </summary>
			/// <param name="source">Source.</param>
			/// <typeparam name="T">The 1st type parameter.</typeparam>
			public static T Guarantee<T>(Component source) where T : Component
			{
				//TODO: if called via ExecuteInEditMode, need call to UnityEditor.Undo.RecordObject
				return source.GetComponent<T>() ?? source.gameObject.AddComponent<T>();
			}

			/// <summary>
			/// Guarantee that the specified Component type T exists on the specified source GameObject.
			/// If it doesn't exist, it will be created.
			/// </summary>
			/// <param name="source">Source.</param>
			/// <typeparam name="T">The 1st type parameter.</typeparam>
			public static T Guarantee<T>(GameObject source) where T : Component
			{
				//TODO: if called via ExecuteInEditMode, need call to UnityEditor.Undo.RecordObject
				return source.GetComponent<T>() ?? source.AddComponent<T>();
			}


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
			/// REQUIRE COMPONENT on SPECIFIED COMPONENT'S GAME OBJECT:
			/// Require the specified Component type to exist on the specified source Component's GameObject.
			/// </summary>
			/// <param name="source">Source Component.</param>
			/// <param name="throwException">If set to <c>true</c> throw exception.</param>
			/// <typeparam name="T">The required Component type.</typeparam>
			public static T Require<T>(Component source, bool throwException = true) where T : Component
			{
				if (!SourceExists(source, typeof(T), throwException)) return null;
				T comp = source.GetComponent<T>();
				if (IsNull(comp))
				{
					string s = string.Format("A {0} Component must exist on the {1}'s {2} GameObject.",
								   typeof(T), source.GetType(), source.name);
					ErrorOrException(s, throwException);
					return null;
				}
				return comp;
			}

			/// <summary>
			/// REQUIRE COMPONENT on SPECIFIED GAME OBJECT:
			/// Require the specified Component type to exist on the specified source GameObject.
			/// </summary>
			/// <param name="source">Source GameObject.</param>
			/// <param name="throwException">If set to <c>true</c> throw exception.</param>
			/// <typeparam name="T">The required Component type.</typeparam>
			public static T Require<T>(GameObject source, bool throwException = true) where T : Component
			{
				if (!SourceExists(source, typeof(T), throwException)) return null;
				T comp = source.GetComponent<T>();
				if (IsNull(comp))
				{
					string s = string.Format("A {0} Component must exist on the {1} GameObject.",
								   typeof(T), source.name);
					ErrorOrException(s, throwException);
					return null;
				}
				return comp;
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
					string s = string.Format("{0} or more {1} Components must exist on the {2}'s {3} GameObject."
							   + " But it has {4} of them.", minCount, typeof(T), source.GetType(), source.name, len);
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
					string s = string.Format("{0} or more {1} Components must exist on the {2} GameObject."
							   + " But it has {4} of them.", minCount, typeof(T), source.name, len);
					ErrorOrException(s, throwException);
					return null;
				}
				return comps;
			}


			/// <summary>
			/// Log a warning with the specified message and optional objects.
			/// </summary>
			/// <param name="message">Message, or format (if containing "{0").</param>
			/// <param name="objs">Objects.</param>
			public static void Warning(string message, params object[] objs)
			{
				if (message.Contains(FORMAT_MAGIC_STRING))
				{
					Debug.LogWarningFormat(message, objs);
				}
				else
				{
					Debug.LogWarning(message + "; " + GetInfo(objs));
				}
			}


			/// <summary>
			/// Logs an error regarding an unsupported condition on the specified source.
			/// </summary>
			/// <param name="source">Source.</param>
			public static void Unsupported(Component source)
			{
				Debug.LogErrorFormat("Unsupported condition for {0} Component on {1} GameObject.",
					source.GetType(),
					source.name);
			}

			/// <summary>
			/// Logs an error regarding an unsupported condition on the specified source.
			/// </summary>
			/// <param name="source">Source.</param>
			/// <param name="message">Message.</param>
			public static void Unsupported(Component source, string message)
			{
				Debug.LogErrorFormat("Unsupported condition for {0} Component on {1} GameObject: {2}.",
					source.GetType(),
					source.name,
					message);
			}

			/// <summary>
			/// Logs an error regarding an unsupported enum on the specified source.
			/// </summary>
			/// <param name="source">Source.</param>
			/// <param name="unsupportedEnum">Unsupported enum.</param>
			public static void Unsupported(Component source, System.Enum unsupportedEnum)
			{
				Debug.LogErrorFormat("Unsupported {0}: {1} for {2} Component on {3} GameObject.",
					unsupportedEnum.GetType(),
					unsupportedEnum,
					source.GetType(),
					source.name);
			}

			/// <summary>
			/// Logs an error regarding an unsupported enum on the specified source.
			/// </summary>
			/// <param name="source">Source.</param>
			/// <param name="unsupportedEnum">Unsupported enum.</param>
			public static void Unsupported(object source, System.Enum unsupportedEnum)
			{
				Debug.LogErrorFormat("Unsupported {0}: {1} for {2} object.",
					unsupportedEnum.GetType(),
					unsupportedEnum,
					source.GetType());
			}
		}
	}
}
