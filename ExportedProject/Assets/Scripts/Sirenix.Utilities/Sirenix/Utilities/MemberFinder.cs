using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Sirenix.Utilities
{
	public class MemberFinder
	{
		[Flags]
		private enum ConditionFlags
		{
			None = 0,
			IsStatic = 2,
			IsProperty = 4,
			IsInstance = 8,
			IsDeclaredOnly = 0x10,
			HasNoParamaters = 0x20,
			IsMethod = 0x40,
			IsField = 0x80,
			IsPublic = 0x100,
			IsNonPublic = 0x200
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass39_0
		{
			public bool isMethod;

			public bool isField;

			public bool isProperty;

			internal bool _003CTryGetMembers_003Eb__9(MemberInfo t)
			{
				if (!((t is MethodInfo) & isMethod) && !((t is FieldInfo) & isField))
				{
					return (t is PropertyInfo) & isProperty;
				}
				return true;
			}
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<MemberInfo, bool> _003C_003E9__39_1;

			public static Func<MethodInfo, bool> _003C_003E9__39_3;

			public static Func<Type, string> _003C_003E9__39_10;

			public static Func<Type, string> _003C_003E9__39_11;

			internal bool _003CTryGetMembers_003Eb__39_1(MemberInfo x)
			{
				if (x is MethodInfo)
				{
					return (x as MethodInfo).GetParameters().Length == 0;
				}
				return true;
			}

			internal bool _003CTryGetMembers_003Eb__39_3(MethodInfo x)
			{
				return x.GetParameters().Length == 0;
			}

			internal string _003CTryGetMembers_003Eb__39_10(Type x)
			{
				return x.GetNiceName();
			}

			internal string _003CTryGetMembers_003Eb__39_11(Type n)
			{
				return n.GetNiceName();
			}
		}

		private Type type;

		private Cache<MemberFinder> cache;

		private ConditionFlags conditionFlags;

		private string name;

		private Type returnType;

		private List<Type> paramTypes = new List<Type>();

		private bool returnTypeCanInherit;

		public MemberFinder()
		{
		}

		public MemberFinder(Type type)
		{
			Start(type, null);
		}

		public static MemberFinder Start<T>()
		{
			Cache<MemberFinder> cache = Cache<MemberFinder>.Claim();
			return cache.Value.Start(typeof(T), cache);
		}

		public static MemberFinder Start(Type type)
		{
			Cache<MemberFinder> cache = Cache<MemberFinder>.Claim();
			return cache.Value.Start(type, cache);
		}

		public MemberFinder HasNoParameters()
		{
			conditionFlags |= ConditionFlags.HasNoParamaters;
			return this;
		}

		public MemberFinder IsDeclaredOnly()
		{
			conditionFlags |= ConditionFlags.IsDeclaredOnly;
			return this;
		}

		public MemberFinder HasParameters(Type param1)
		{
			conditionFlags |= ConditionFlags.IsMethod;
			paramTypes.Add(param1);
			return this;
		}

		public MemberFinder HasParameters(Type param1, Type param2)
		{
			conditionFlags |= ConditionFlags.IsMethod;
			paramTypes.Add(param1);
			paramTypes.Add(param2);
			return this;
		}

		public MemberFinder HasParameters(Type param1, Type param2, Type param3)
		{
			conditionFlags |= ConditionFlags.IsMethod;
			paramTypes.Add(param1);
			paramTypes.Add(param2);
			paramTypes.Add(param3);
			return this;
		}

		public MemberFinder HasParameters(Type param1, Type param2, Type param3, Type param4)
		{
			conditionFlags |= ConditionFlags.IsMethod;
			paramTypes.Add(param1);
			paramTypes.Add(param2);
			paramTypes.Add(param3);
			paramTypes.Add(param4);
			return this;
		}

		public MemberFinder HasParameters<T>()
		{
			return HasParameters(typeof(T));
		}

		public MemberFinder HasParameters<T1, T2>()
		{
			return HasParameters(typeof(T1), typeof(T2));
		}

		public MemberFinder HasParameters<T1, T2, T3>()
		{
			return HasParameters(typeof(T1), typeof(T2), typeof(T3));
		}

		public MemberFinder HasParameters<T1, T2, T3, T4>()
		{
			return HasParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
		}

		public MemberFinder HasReturnType(Type returnType, bool inherit = false)
		{
			returnTypeCanInherit = inherit;
			this.returnType = returnType;
			return this;
		}

		public MemberFinder HasReturnType<T>(bool inherit = false)
		{
			return HasReturnType(typeof(T), inherit);
		}

		public MemberFinder IsFieldOrProperty()
		{
			IsField();
			IsProperty();
			return this;
		}

		public MemberFinder IsStatic()
		{
			conditionFlags |= ConditionFlags.IsStatic;
			return this;
		}

		public MemberFinder IsInstance()
		{
			conditionFlags |= ConditionFlags.IsInstance;
			return this;
		}

		public MemberFinder IsNamed(string name)
		{
			this.name = name;
			return this;
		}

		public MemberFinder IsProperty()
		{
			conditionFlags |= ConditionFlags.IsProperty;
			return this;
		}

		public MemberFinder IsMethod()
		{
			conditionFlags |= ConditionFlags.IsMethod;
			return this;
		}

		public MemberFinder IsField()
		{
			conditionFlags |= ConditionFlags.IsField;
			return this;
		}

		public MemberFinder IsPublic()
		{
			conditionFlags |= ConditionFlags.IsPublic;
			return this;
		}

		public MemberFinder IsNonPublic()
		{
			conditionFlags |= ConditionFlags.IsNonPublic;
			return this;
		}

		public MemberFinder ReturnsVoid()
		{
			conditionFlags |= ConditionFlags.IsMethod;
			return HasReturnType(typeof(void));
		}

		public T GetMember<T>() where T : MemberInfo
		{
			string errorMessage = null;
			return GetMember<T>(out errorMessage);
		}

		public T GetMember<T>(out string errorMessage) where T : MemberInfo
		{
			T memberInfo;
			TryGetMember(out memberInfo, out errorMessage);
			return memberInfo;
		}

		public MemberInfo GetMember(out string errorMessage)
		{
			MemberInfo memberInfo;
			TryGetMember(out memberInfo, out errorMessage);
			return memberInfo;
		}

		public bool TryGetMember<T>(out T memberInfo, out string errorMessage) where T : MemberInfo
		{
			MemberInfo memberInfo2;
			bool result = TryGetMember(out memberInfo2, out errorMessage);
			memberInfo = memberInfo2 as T;
			return result;
		}

		public bool TryGetMember(out MemberInfo memberInfo, out string errorMessage)
		{
			MemberInfo[] memberInfos;
			if (TryGetMembers(out memberInfos, out errorMessage))
			{
				memberInfo = memberInfos[0];
				return true;
			}
			memberInfo = null;
			return false;
		}

		public bool TryGetMembers(out MemberInfo[] memberInfos, out string errorMessage)
		{
			try
			{
				_003C_003Ec__DisplayClass39_0 _003C_003Ec__DisplayClass39_ = new _003C_003Ec__DisplayClass39_0();
				IEnumerable<MemberInfo> source = Enumerable.Empty<MemberInfo>();
				BindingFlags bindingFlags = (HasCondition(ConditionFlags.IsDeclaredOnly) ? BindingFlags.DeclaredOnly : BindingFlags.FlattenHierarchy);
				bool flag = HasCondition(ConditionFlags.HasNoParamaters);
				bool flag2 = HasCondition(ConditionFlags.IsInstance);
				bool flag3 = HasCondition(ConditionFlags.IsStatic);
				bool flag4 = HasCondition(ConditionFlags.IsPublic);
				bool flag5 = HasCondition(ConditionFlags.IsNonPublic);
				_003C_003Ec__DisplayClass39_.isMethod = HasCondition(ConditionFlags.IsMethod);
				_003C_003Ec__DisplayClass39_.isField = HasCondition(ConditionFlags.IsField);
				_003C_003Ec__DisplayClass39_.isProperty = HasCondition(ConditionFlags.IsProperty);
				if (!flag4 && !flag5)
				{
					flag4 = true;
					flag5 = true;
				}
				if (!flag3 && !flag2)
				{
					flag3 = true;
					flag2 = true;
				}
				if (!(_003C_003Ec__DisplayClass39_.isField | _003C_003Ec__DisplayClass39_.isProperty | _003C_003Ec__DisplayClass39_.isMethod))
				{
					_003C_003Ec__DisplayClass39_.isMethod = true;
					_003C_003Ec__DisplayClass39_.isField = true;
					_003C_003Ec__DisplayClass39_.isProperty = true;
				}
				if (flag2)
				{
					bindingFlags |= BindingFlags.Instance;
				}
				if (flag3)
				{
					bindingFlags |= BindingFlags.Static;
				}
				if (flag4)
				{
					bindingFlags |= BindingFlags.Public;
				}
				if (flag5)
				{
					bindingFlags |= BindingFlags.NonPublic;
				}
				if (_003C_003Ec__DisplayClass39_.isMethod & _003C_003Ec__DisplayClass39_.isField & _003C_003Ec__DisplayClass39_.isProperty)
				{
					source = ((name != null) ? type.GetAllMembers(bindingFlags).Where(_003CTryGetMembers_003Eb__39_0) : type.GetAllMembers(bindingFlags));
					if (flag)
					{
						source = source.Where(_003C_003Ec._003C_003E9__39_1 ?? (_003C_003Ec._003C_003E9__39_1 = _003C_003Ec._003C_003E9._003CTryGetMembers_003Eb__39_1));
					}
				}
				else
				{
					if (_003C_003Ec__DisplayClass39_.isMethod)
					{
						IEnumerable<MethodInfo> source2 = ((name == null) ? type.GetAllMembers<MethodInfo>(bindingFlags) : type.GetAllMembers<MethodInfo>(bindingFlags).Where(_003CTryGetMembers_003Eb__39_2));
						if (flag)
						{
							source2 = source2.Where(_003C_003Ec._003C_003E9__39_3 ?? (_003C_003Ec._003C_003E9__39_3 = _003C_003Ec._003C_003E9._003CTryGetMembers_003Eb__39_3));
						}
						else if (paramTypes.Count > 0)
						{
							source2 = source2.Where(_003CTryGetMembers_003Eb__39_4);
						}
						source = source2.OfType<MemberInfo>();
					}
					if (_003C_003Ec__DisplayClass39_.isField)
					{
						source = ((name != null) ? source.Append(type.GetAllMembers<FieldInfo>(bindingFlags).Where(_003CTryGetMembers_003Eb__39_5).Cast<MemberInfo>()) : source.Append(type.GetAllMembers<FieldInfo>(bindingFlags).Cast<MemberInfo>()));
					}
					if (_003C_003Ec__DisplayClass39_.isProperty)
					{
						source = ((name != null) ? source.Append(type.GetAllMembers<PropertyInfo>(bindingFlags).Where(_003CTryGetMembers_003Eb__39_6).Cast<MemberInfo>()) : source.Append(type.GetAllMembers<PropertyInfo>(bindingFlags).Cast<MemberInfo>()));
					}
				}
				if (returnType != null)
				{
					source = ((!returnTypeCanInherit) ? source.Where(_003CTryGetMembers_003Eb__39_8) : source.Where(_003CTryGetMembers_003Eb__39_7));
				}
				memberInfos = source.ToArray();
				if (memberInfos != null && memberInfos.Length != 0)
				{
					errorMessage = null;
					return true;
				}
				MemberInfo memberInfo = ((name == null) ? null : type.GetMember(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).FirstOrDefault(_003C_003Ec__DisplayClass39_._003CTryGetMembers_003Eb__9));
				if (memberInfo != null)
				{
					string text = (memberInfo.IsStatic() ? "Static " : "Non-static ");
					if (flag && memberInfo is MethodInfo && (memberInfo as MethodInfo).GetParameters().Length != 0)
					{
						errorMessage = text + "method " + name + " can not take parameters.";
						return false;
					}
					if (_003C_003Ec__DisplayClass39_.isMethod && paramTypes.Count > 0 && memberInfo is MethodInfo && !(memberInfo as MethodInfo).HasParamaters(paramTypes))
					{
						errorMessage = text + "method " + name + " must have the following parameters: " + string.Join(", ", paramTypes.Select(_003C_003Ec._003C_003E9__39_10 ?? (_003C_003Ec._003C_003E9__39_10 = _003C_003Ec._003C_003E9._003CTryGetMembers_003Eb__39_10)).ToArray()) + ".";
						return false;
					}
					if (returnType != null && returnType != memberInfo.GetReturnType())
					{
						if (returnTypeCanInherit)
						{
							errorMessage = text + memberInfo.MemberType.ToString().ToLower(CultureInfo.InvariantCulture) + " " + name + " must have a return type that is assignable from " + returnType.GetNiceName() + ".";
						}
						else
						{
							errorMessage = text + memberInfo.MemberType.ToString().ToLower(CultureInfo.InvariantCulture) + " " + name + " must have a return type of " + returnType.GetNiceName() + ".";
						}
						return false;
					}
				}
				int num = (_003C_003Ec__DisplayClass39_.isField ? 1 : 0) + (_003C_003Ec__DisplayClass39_.isProperty ? 1 : 0) + (_003C_003Ec__DisplayClass39_.isMethod ? 1 : 0);
				string text2 = (_003C_003Ec__DisplayClass39_.isField ? ("fields" + ((num-- <= 1) ? " " : ((num == 1) ? " or " : ", "))) : string.Empty) + (_003C_003Ec__DisplayClass39_.isProperty ? ("properties" + ((num-- <= 1) ? " " : ((num == 1) ? " or " : ", "))) : string.Empty) + (_003C_003Ec__DisplayClass39_.isMethod ? ("methods" + ((num-- <= 1) ? " " : ((num == 1) ? " or " : ", "))) : string.Empty);
				string text3 = ((flag4 == flag5) ? string.Empty : (flag4 ? "public " : "non-public ")) + ((flag3 == flag2) ? string.Empty : (flag3 ? "static " : "non-static "));
				string text4 = ((returnType == null) ? " " : ("with a return type of " + returnType.GetNiceName() + " "));
				string text5 = ((paramTypes.Count == 0) ? " " : (((text4 == " ") ? "" : "and ") + "with the parameter signature (" + string.Join(", ", paramTypes.Select(_003C_003Ec._003C_003E9__39_11 ?? (_003C_003Ec._003C_003E9__39_11 = _003C_003Ec._003C_003E9._003CTryGetMembers_003Eb__39_11)).ToArray()) + ") "));
				if (name == null)
				{
					errorMessage = "No " + text3 + text2 + text4 + text5 + "was found in " + type.GetNiceName() + ".";
					return false;
				}
				errorMessage = "No " + text3 + text2 + "named " + name + " " + text4 + text5 + "was found in " + type.GetNiceName() + ".";
				return false;
			}
			finally
			{
				Cache<MemberFinder>.Release(cache);
			}
		}

		private MemberFinder Start(Type type, Cache<MemberFinder> cache)
		{
			this.type = type;
			this.cache = cache;
			Reset();
			return this;
		}

		private void Reset()
		{
			returnType = null;
			name = null;
			conditionFlags = ConditionFlags.None;
			paramTypes.Clear();
		}

		private bool HasCondition(ConditionFlags flag)
		{
			return (conditionFlags & flag) == flag;
		}

		[CompilerGenerated]
		private bool _003CTryGetMembers_003Eb__39_0(MemberInfo n)
		{
			return n.Name == name;
		}

		[CompilerGenerated]
		private bool _003CTryGetMembers_003Eb__39_2(MethodInfo x)
		{
			return x.Name == name;
		}

		[CompilerGenerated]
		private bool _003CTryGetMembers_003Eb__39_4(MethodInfo x)
		{
			return x.HasParamaters(paramTypes);
		}

		[CompilerGenerated]
		private bool _003CTryGetMembers_003Eb__39_5(FieldInfo n)
		{
			return n.Name == name;
		}

		[CompilerGenerated]
		private bool _003CTryGetMembers_003Eb__39_6(PropertyInfo n)
		{
			return n.Name == name;
		}

		[CompilerGenerated]
		private bool _003CTryGetMembers_003Eb__39_7(MemberInfo x)
		{
			return x.GetReturnType().InheritsFrom(returnType);
		}

		[CompilerGenerated]
		private bool _003CTryGetMembers_003Eb__39_8(MemberInfo x)
		{
			return x.GetReturnType() == returnType;
		}
	}
}
