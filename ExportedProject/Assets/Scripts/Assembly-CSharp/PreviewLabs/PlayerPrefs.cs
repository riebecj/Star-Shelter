using System;
using System.Collections;
using System.IO;
using System.Text;
using Steamworks;
using UnityEngine;

namespace PreviewLabs
{
	public static class PlayerPrefs
	{
		public static Hashtable playerPrefsHashtable;

		private static bool hashTableChanged;

		private static string serializedOutput;

		private static string serializedInput;

		private const string PARAMETERS_SEPERATOR = ";";

		private const string KEY_VALUE_SEPERATOR = ":";

		public static int saveSlot;

		internal static string fileName;

		static PlayerPrefs()
		{
			playerPrefsHashtable = new Hashtable();
			hashTableChanged = false;
			serializedOutput = string.Empty;
			serializedInput = string.Empty;
			fileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/StarShelter/PlayerPrefs" + saveSlot + ".txt";
			LoadIn();
		}

		public static void UpdateSaveSlot(int _saveSlot)
		{
			saveSlot = _saveSlot;
			fileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/StarShelter/PlayerPrefs" + saveSlot + ".txt";
		}

		public static void DeleteSaveSlot(int _saveSlot)
		{
			fileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/StarShelter/PlayerPrefs" + _saveSlot + ".txt";
			File.Delete(fileName);
			if (SteamManager.Initialized && SteamRemoteStorage.IsCloudEnabledForApp())
			{
				SteamRemoteStorage.FileDelete("StarShelterSave" + _saveSlot);
			}
			fileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/StarShelter/PlayerPrefs" + _saveSlot + ".txt";
			saveSlot = _saveSlot;
			DeleteAll();
		}

		public static void LoadIn()
		{
			StreamReader streamReader = null;
			if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/StarShelter"))
			{
				DirectoryInfo directoryInfo = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/StarShelter");
			}
			if (File.Exists(fileName))
			{
				streamReader = new StreamReader(fileName);
				serializedInput = streamReader.ReadLine();
				Deserialize();
				streamReader.Close();
			}
			else if (SteamManager.Initialized && SteamRemoteStorage.IsCloudEnabledForApp())
			{
				byte[] array = new byte[1000];
				int count = SteamRemoteStorage.FileRead("StarShelterSave" + saveSlot, array, array.Length);
				serializedInput = Encoding.UTF8.GetString(array, 0, count);
				Deserialize();
				Serialize();
				StreamWriter streamWriter = null;
				streamWriter = File.CreateText(fileName);
				streamWriter.WriteLine(serializedOutput);
				streamWriter.Close();
			}
		}

		public static bool HasKey(string key)
		{
			return playerPrefsHashtable.ContainsKey(key);
		}

		public static void SetString(string key, string value)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}
			hashTableChanged = true;
		}

		public static void SetInt(string key, int value)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}
			hashTableChanged = true;
		}

		public static void SetFloat(string key, float value)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}
			hashTableChanged = true;
		}

		public static void SetBool(string key, bool value)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			}
			else
			{
				playerPrefsHashtable[key] = value;
			}
			hashTableChanged = true;
		}

		public static string GetString(string key)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return playerPrefsHashtable[key].ToString();
			}
			return null;
		}

		public static string GetString(string key, string defaultValue)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return playerPrefsHashtable[key].ToString();
			}
			playerPrefsHashtable.Add(key, defaultValue);
			hashTableChanged = true;
			return defaultValue;
		}

		public static int GetInt(string key)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (int)playerPrefsHashtable[key];
			}
			return 0;
		}

		public static int GetInt(string key, int defaultValue)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (int)playerPrefsHashtable[key];
			}
			playerPrefsHashtable.Add(key, defaultValue);
			hashTableChanged = true;
			return defaultValue;
		}

		public static float GetFloat(string key)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (float)playerPrefsHashtable[key];
			}
			return 0f;
		}

		public static float GetFloat(string key, float defaultValue)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (float)playerPrefsHashtable[key];
			}
			playerPrefsHashtable.Add(key, defaultValue);
			hashTableChanged = true;
			return defaultValue;
		}

		public static bool GetBool(string key)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (bool)playerPrefsHashtable[key];
			}
			return false;
		}

		public static bool GetBool(string key, bool defaultValue)
		{
			if (playerPrefsHashtable.ContainsKey(key))
			{
				return (bool)playerPrefsHashtable[key];
			}
			playerPrefsHashtable.Add(key, defaultValue);
			hashTableChanged = true;
			return defaultValue;
		}

		public static void DeleteKey(string key)
		{
			playerPrefsHashtable.Remove(key);
			hashTableChanged = true;
		}

		public static void DeleteAll()
		{
			playerPrefsHashtable.Clear();
			hashTableChanged = true;
		}

		public static void Flush()
		{
			if (hashTableChanged)
			{
				Serialize();
				StreamWriter streamWriter = null;
				streamWriter = File.CreateText(fileName);
				if (streamWriter == null)
				{
					Debug.LogWarning("PlayerPrefs::Flush() opening file for writing failed: " + fileName);
				}
				streamWriter.WriteLine(serializedOutput);
				streamWriter.Close();
				if (SteamManager.Initialized && SteamRemoteStorage.IsCloudEnabledForApp())
				{
					byte[] array = new byte[Encoding.UTF8.GetByteCount(serializedOutput)];
					Encoding.UTF8.GetBytes(serializedOutput, 0, serializedOutput.Length, array, 0);
					bool flag = SteamRemoteStorage.FileWrite("StarShelterSave" + saveSlot, array, array.Length);
				}
				serializedOutput = string.Empty;
			}
		}

		private static void Serialize()
		{
			IDictionaryEnumerator enumerator = playerPrefsHashtable.GetEnumerator();
			StringBuilder stringBuilder = new StringBuilder(null, 1000000);
			string text = " ";
			string value = " ; ";
			string value2 = " : ";
			while (enumerator.MoveNext())
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(value);
				}
				if (enumerator.Current != null && enumerator.Value != null && enumerator.Key != null)
				{
					stringBuilder.Append(EscapeNonSeperators(enumerator.Key.ToString()));
					stringBuilder.Append(value2);
					stringBuilder.Append(EscapeNonSeperators(enumerator.Value.ToString()));
					stringBuilder.Append(value2);
					stringBuilder.Append(enumerator.Value.GetType());
				}
			}
			serializedOutput = stringBuilder.ToString();
		}

		private static void Deserialize()
		{
			if (serializedInput == null)
			{
				return;
			}
			string[] array = serializedInput.Split(new string[1] { " ; " }, StringSplitOptions.None);
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Split(new string[1] { " : " }, StringSplitOptions.None);
				if (array3.Length == 3)
				{
					playerPrefsHashtable.Add(DeEscapeNonSeperators(array3[0]), GetTypeValue(array3[2], DeEscapeNonSeperators(array3[1])));
				}
				else
				{
					Debug.LogWarning("PlayerPrefs::Deserialize() parameterContent has " + array3.Length + " elements");
				}
			}
		}

		public static void Paste(int copiedSlot, int newSlot)
		{
			if (serializedInput == null)
			{
				return;
			}
			playerPrefsHashtable.Clear();
			LoadIn();
			string[] array = serializedInput.Split(new string[1] { " ; " }, StringSplitOptions.None);
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Split(new string[1] { " : " }, StringSplitOptions.None);
				if (array3.Length == 3 && DeEscapeNonSeperators(array3[0]).EndsWith(copiedSlot.ToString()))
				{
					string text2 = DeEscapeNonSeperators(array3[0]);
					text2 = text2.Remove(text2.Length - 1, 1) + newSlot;
					playerPrefsHashtable.Add(text2, GetTypeValue(array3[2], DeEscapeNonSeperators(array3[1])));
				}
			}
			hashTableChanged = true;
			Flush();
		}

		private static string EscapeNonSeperators(string inputToEscape)
		{
			inputToEscape = inputToEscape.Replace(":", "\\:");
			inputToEscape = inputToEscape.Replace(";", "\\;");
			return inputToEscape;
		}

		private static string DeEscapeNonSeperators(string inputToDeEscape)
		{
			inputToDeEscape = inputToDeEscape.Replace("\\:", ":");
			inputToDeEscape = inputToDeEscape.Replace("\\;", ";");
			return inputToDeEscape;
		}

		public static object GetTypeValue(string typeName, string value)
		{
			switch (typeName)
			{
			case "System.String":
				return value.ToString();
			case "System.Int32":
				return Convert.ToInt32(value);
			case "System.Boolean":
				return Convert.ToBoolean(value);
			case "System.Single":
				return Convert.ToSingle(value);
			default:
				Debug.LogError("Unsupported type: " + typeName);
				return null;
			}
		}

		public static byte[] GetBytes(string str)
		{
			byte[] array = new byte[str.Length * 2];
			Buffer.BlockCopy(str.ToCharArray(), 0, array, 0, array.Length);
			return array;
		}

		public static string GetString(byte[] bytes)
		{
			char[] array = new char[bytes.Length / 2];
			Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
			return new string(array);
		}
	}
}
