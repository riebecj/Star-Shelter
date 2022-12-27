using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Sirenix.Utilities;
using UnityEngine;

namespace Sirenix.Serialization
{
	public sealed class PrefabModification
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<MemberInfo, bool> _003C_003E9__13_0;

			public static Func<MemberInfo, bool> _003C_003E9__16_0;

			internal bool _003CGetInstanceOfStep_003Eb__13_0(MemberInfo n)
			{
				if (!(n is FieldInfo))
				{
					return n is PropertyInfo;
				}
				return true;
			}

			internal bool _003CTrySetInstanceOfStep_003Eb__16_0(MemberInfo n)
			{
				if (!(n is FieldInfo))
				{
					return n is PropertyInfo;
				}
				return true;
			}
		}

		public PrefabModificationType ModificationType;

		public string Path;

		public List<string> ReferencePaths;

		public object ModifiedValue;

		public int NewLength;

		public object[] DictionaryKeysAdded;

		public object[] DictionaryKeysRemoved;

		public void Apply(UnityEngine.Object unityObject)
		{
			if (ModificationType == PrefabModificationType.Value)
			{
				ApplyValue(unityObject);
				return;
			}
			if (ModificationType == PrefabModificationType.ListLength)
			{
				ApplyListLength(unityObject);
				return;
			}
			if (ModificationType == PrefabModificationType.Dictionary)
			{
				ApplyDictionaryModifications(unityObject);
				return;
			}
			throw new NotImplementedException(ModificationType.ToString());
		}

		private void ApplyValue(UnityEngine.Object unityObject)
		{
			Type type = null;
			if (ModifiedValue != null)
			{
				type = ModifiedValue.GetType();
			}
			if (type != null && ReferencePaths != null && ReferencePaths.Count > 0)
			{
				for (int i = 0; i < ReferencePaths.Count; i++)
				{
					string path = ReferencePaths[i];
					try
					{
						object instanceFromPath = GetInstanceFromPath(path, unityObject);
						if (instanceFromPath != null && instanceFromPath.GetType() == type)
						{
							ModifiedValue = instanceFromPath;
							break;
						}
					}
					catch (Exception)
					{
					}
				}
			}
			SetInstanceToPath(Path, unityObject, ModifiedValue);
		}

		private void ApplyListLength(UnityEngine.Object unityObject)
		{
			object instanceFromPath = GetInstanceFromPath(Path, unityObject);
			if (instanceFromPath == null)
			{
				return;
			}
			Type type = instanceFromPath.GetType();
			if (type.IsArray)
			{
				Array array = (Array)instanceFromPath;
				if (NewLength != array.Length)
				{
					Array array2 = Array.CreateInstance(type.GetElementType(), NewLength);
					if (NewLength > array.Length)
					{
						Array.Copy(array, 0, array2, 0, array.Length);
						ReplaceAllReferencesInGraph(unityObject, array, array2);
					}
					else
					{
						Array.Copy(array, 0, array2, 0, array2.Length);
						ReplaceAllReferencesInGraph(unityObject, array, array2);
					}
				}
			}
			else if (typeof(IList).IsAssignableFrom(type))
			{
				IList list = (IList)instanceFromPath;
				Type type2 = (type.ImplementsOpenGenericInterface(typeof(IList<>)) ? type.GetArgumentsOfInheritedOpenGenericInterface(typeof(IList<>))[0] : null);
				bool flag = type2 != null && type2.IsValueType;
				int num = 0;
				while (list.Count < NewLength)
				{
					if (flag)
					{
						list.Add(Activator.CreateInstance(type2));
					}
					else
					{
						list.Add(null);
					}
					num++;
				}
				while (list.Count > NewLength)
				{
					list.RemoveAt(list.Count - 1);
				}
			}
			else
			{
				if (!type.ImplementsOpenGenericInterface(typeof(IList<>)))
				{
					return;
				}
				Type type3 = type.GetArgumentsOfInheritedOpenGenericInterface(typeof(IList<>))[0];
				Type type4 = typeof(ICollection<>).MakeGenericType(type3);
				bool isValueType = type3.IsValueType;
				PropertyInfo property = type4.GetProperty("Count");
				int num2 = (int)property.GetValue(instanceFromPath, null);
				if (num2 < NewLength)
				{
					int num3 = NewLength - num2;
					MethodInfo method = type4.GetMethod("Add");
					for (int i = 0; i < num3; i++)
					{
						if (isValueType)
						{
							method.Invoke(instanceFromPath, new object[1] { Activator.CreateInstance(type3) });
						}
						else
						{
							method.Invoke(instanceFromPath, new object[1]);
						}
						num2++;
					}
				}
				else if (num2 > NewLength)
				{
					int num4 = num2 - NewLength;
					Type type5 = typeof(IList<>).MakeGenericType(type3);
					MethodInfo method2 = type5.GetMethod("RemoveAt");
					for (int j = 0; j < num4; j++)
					{
						method2.Invoke(instanceFromPath, new object[1] { num2 - (num4 + 1) });
					}
				}
			}
		}

		private void ApplyDictionaryModifications(UnityEngine.Object unityObject)
		{
			object instanceFromPath = GetInstanceFromPath(Path, unityObject);
			if (instanceFromPath == null)
			{
				return;
			}
			Type type = instanceFromPath.GetType();
			if (!type.ImplementsOpenGenericInterface(typeof(IDictionary<, >)))
			{
				return;
			}
			Type[] argumentsOfInheritedOpenGenericInterface = type.GetArgumentsOfInheritedOpenGenericInterface(typeof(IDictionary<, >));
			Type type2 = typeof(IDictionary<, >).MakeGenericType(argumentsOfInheritedOpenGenericInterface);
			if (DictionaryKeysRemoved != null && DictionaryKeysRemoved.Length != 0)
			{
				MethodInfo method = type2.GetMethod("Remove");
				object[] array = new object[1];
				for (int i = 0; i < DictionaryKeysRemoved.Length; i++)
				{
					array[0] = DictionaryKeysRemoved[i];
					method.Invoke(instanceFromPath, array);
				}
			}
			if (DictionaryKeysAdded != null && DictionaryKeysAdded.Length != 0)
			{
				MethodInfo method2 = type2.GetMethod("set_Item");
				object[] array2 = new object[2]
				{
					null,
					argumentsOfInheritedOpenGenericInterface[1].IsValueType ? Activator.CreateInstance(argumentsOfInheritedOpenGenericInterface[1]) : null
				};
				for (int j = 0; j < DictionaryKeysAdded.Length; j++)
				{
					array2[0] = DictionaryKeysAdded[j];
					method2.Invoke(instanceFromPath, array2);
				}
			}
		}

		private static void ReplaceAllReferencesInGraph(object graph, object oldReference, object newReference, HashSet<object> processedReferences = null)
		{
			if (processedReferences == null)
			{
				processedReferences = new HashSet<object>(ReferenceEqualityComparer<object>.Default);
			}
			processedReferences.Add(graph);
			if (graph.GetType().IsArray)
			{
				Array array = (Array)graph;
				for (int i = 0; i < array.Length; i++)
				{
					object obj = array.GetValue(i);
					if (obj != null)
					{
						if (obj == oldReference)
						{
							array.SetValue(newReference, i);
							obj = newReference;
						}
						if (!processedReferences.Contains(obj))
						{
							ReplaceAllReferencesInGraph(obj, oldReference, newReference, processedReferences);
						}
					}
				}
				return;
			}
			MemberInfo[] serializableMembers = FormatterUtilities.GetSerializableMembers(graph.GetType(), SerializationPolicies.Everything);
			for (int j = 0; j < serializableMembers.Length; j++)
			{
				FieldInfo fieldInfo = (FieldInfo)serializableMembers[j];
				if (fieldInfo.FieldType.IsPrimitive || fieldInfo.FieldType == typeof(SerializationData) || fieldInfo.FieldType == typeof(string))
				{
					continue;
				}
				object obj2 = fieldInfo.GetValue(graph);
				if (obj2 == null)
				{
					continue;
				}
				Type type = obj2.GetType();
				if (!type.IsPrimitive && type != typeof(SerializationData) && type != typeof(string))
				{
					if (obj2 == oldReference)
					{
						fieldInfo.SetValue(graph, newReference);
						obj2 = newReference;
					}
					if (!processedReferences.Contains(obj2))
					{
						ReplaceAllReferencesInGraph(obj2, oldReference, newReference, processedReferences);
					}
				}
			}
		}

		private static object GetInstanceFromPath(string path, object instance)
		{
			string[] array = path.Split('.');
			object obj = instance;
			for (int i = 0; i < array.Length; i++)
			{
				obj = GetInstanceOfStep(array[i], obj);
				if (obj == null)
				{
					return null;
				}
			}
			return obj;
		}

		private static object GetInstanceOfStep(string step, object instance)
		{
			Type type = instance.GetType();
			if (step.StartsWith("[", StringComparison.InvariantCulture) && step.EndsWith("]", StringComparison.InvariantCulture))
			{
				string s = step.Substring(1, step.Length - 2);
				int result;
				if (!int.TryParse(s, out result))
				{
					throw new ArgumentException("Couldn't parse an index from the path step '" + step + "'.");
				}
				if (type.IsArray)
				{
					Array array = (Array)instance;
					if (result < 0 || result >= array.Length)
					{
						return null;
					}
					return array.GetValue(result);
				}
				if (typeof(IList).IsAssignableFrom(type))
				{
					IList list = (IList)instance;
					if (result < 0 || result >= list.Count)
					{
						return null;
					}
					return list[result];
				}
				if (type.ImplementsOpenGenericInterface(typeof(IList<>)))
				{
					Type type2 = type.GetArgumentsOfInheritedOpenGenericInterface(typeof(IList<>))[0];
					Type type3 = typeof(IList<>).MakeGenericType(type2);
					MethodInfo method = type3.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					try
					{
						return method.Invoke(instance, new object[1] { result });
					}
					catch (Exception)
					{
						return null;
					}
				}
			}
			else if (step.StartsWith("{", StringComparison.InvariantCultureIgnoreCase) && step.EndsWith("}", StringComparison.InvariantCultureIgnoreCase))
			{
				if (type.ImplementsOpenGenericInterface(typeof(IDictionary<, >)))
				{
					Type[] argumentsOfInheritedOpenGenericInterface = type.GetArgumentsOfInheritedOpenGenericInterface(typeof(IDictionary<, >));
					object dictionaryKeyValue = DictionaryKeyUtility.GetDictionaryKeyValue(step, argumentsOfInheritedOpenGenericInterface[0]);
					Type type4 = typeof(IDictionary<, >).MakeGenericType(argumentsOfInheritedOpenGenericInterface);
					MethodInfo method2 = type4.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					try
					{
						return method2.Invoke(instance, new object[1] { dictionaryKeyValue });
					}
					catch (Exception)
					{
						return null;
					}
				}
			}
			else
			{
				string text = null;
				int num = step.IndexOf('+');
				if (num >= 0)
				{
					text = step.Substring(0, num);
					step = step.Substring(num + 1);
				}
				IEnumerable<MemberInfo> enumerable = type.GetAllMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(_003C_003Ec._003C_003E9__13_0 ?? (_003C_003Ec._003C_003E9__13_0 = _003C_003Ec._003C_003E9._003CGetInstanceOfStep_003Eb__13_0));
				foreach (MemberInfo item in enumerable)
				{
					if (item.Name == step && (text == null || !(item.DeclaringType.Name != text)))
					{
						return item.GetMemberValue(instance);
					}
				}
			}
			return null;
		}

		private static void SetInstanceToPath(string path, object instance, object value)
		{
			string[] steps = path.Split('.');
			bool setParentInstance;
			SetInstanceToPath(path, steps, 0, instance, value, out setParentInstance);
		}

		private static void SetInstanceToPath(string path, string[] steps, int index, object instance, object value, out bool setParentInstance)
		{
			setParentInstance = false;
			if (index < steps.Length - 1)
			{
				object instanceOfStep = GetInstanceOfStep(steps[index], instance);
				if (instanceOfStep != null)
				{
					SetInstanceToPath(path, steps, index + 1, instanceOfStep, value, out setParentInstance);
					if (setParentInstance)
					{
						TrySetInstanceOfStep(steps[index], instance, instanceOfStep, out setParentInstance);
					}
				}
			}
			else
			{
				TrySetInstanceOfStep(steps[index], instance, value, out setParentInstance);
			}
		}

		private static bool TrySetInstanceOfStep(string step, object instance, object value, out bool setParentInstance)
		{
			setParentInstance = false;
			try
			{
				Type type = instance.GetType();
				if (step.StartsWith("[", StringComparison.InvariantCulture) && step.EndsWith("]", StringComparison.InvariantCulture))
				{
					string s = step.Substring(1, step.Length - 2);
					int result;
					if (!int.TryParse(s, out result))
					{
						throw new ArgumentException("Couldn't parse an index from the path step '" + step + "'.");
					}
					if (type.IsArray)
					{
						Array array = (Array)instance;
						if (result < 0 || result >= array.Length)
						{
							return false;
						}
						array.SetValue(value, result);
						return true;
					}
					if (typeof(IList).IsAssignableFrom(type))
					{
						IList list = (IList)instance;
						if (result < 0 || result >= list.Count)
						{
							return false;
						}
						list[result] = value;
						return true;
					}
					if (type.ImplementsOpenGenericInterface(typeof(IList<>)))
					{
						Type type2 = type.GetArgumentsOfInheritedOpenGenericInterface(typeof(IList<>))[0];
						Type type3 = typeof(IList<>).MakeGenericType(type2);
						MethodInfo method = type3.GetMethod("set_Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						method.Invoke(instance, new object[2] { result, value });
						return true;
					}
				}
				else if (step.StartsWith("{", StringComparison.InvariantCulture) && step.EndsWith("}", StringComparison.InvariantCulture))
				{
					if (type.ImplementsOpenGenericInterface(typeof(IDictionary<, >)))
					{
						Type[] argumentsOfInheritedOpenGenericInterface = type.GetArgumentsOfInheritedOpenGenericInterface(typeof(IDictionary<, >));
						object dictionaryKeyValue = DictionaryKeyUtility.GetDictionaryKeyValue(step, argumentsOfInheritedOpenGenericInterface[0]);
						Type type4 = typeof(IDictionary<, >).MakeGenericType(argumentsOfInheritedOpenGenericInterface);
						MethodInfo method2 = type4.GetMethod("ContainsKey", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						MethodInfo method3 = type4.GetMethod("set_Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						if (!(bool)method2.Invoke(instance, new object[1] { dictionaryKeyValue }))
						{
							return false;
						}
						method3.Invoke(instance, new object[2] { dictionaryKeyValue, value });
					}
				}
				else
				{
					string text = null;
					int num = step.IndexOf('+');
					if (num >= 0)
					{
						text = step.Substring(0, num);
						step = step.Substring(num + 1);
					}
					IEnumerable<MemberInfo> enumerable = type.GetAllMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(_003C_003Ec._003C_003E9__16_0 ?? (_003C_003Ec._003C_003E9__16_0 = _003C_003Ec._003C_003E9._003CTrySetInstanceOfStep_003Eb__16_0));
					foreach (MemberInfo item in enumerable)
					{
						if (item.Name == step && (text == null || !(item.DeclaringType.Name != text)))
						{
							item.SetMemberValue(instance, value);
							if (instance.GetType().IsValueType)
							{
								setParentInstance = true;
							}
							return true;
						}
					}
				}
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
