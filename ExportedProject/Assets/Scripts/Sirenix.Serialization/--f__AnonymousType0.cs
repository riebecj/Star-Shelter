using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[CompilerGenerated]
internal sealed class _003C_003Ef__AnonymousType0<_003CType_003Ej__TPar, _003CAttr_003Ej__TPar, _003CSerializedType_003Ej__TPar>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly _003CType_003Ej__TPar _003CType_003Ei__Field;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly _003CAttr_003Ej__TPar _003CAttr_003Ei__Field;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly _003CSerializedType_003Ej__TPar _003CSerializedType_003Ei__Field;

	public _003CType_003Ej__TPar Type
	{
		get
		{
			return _003CType_003Ei__Field;
		}
	}

	public _003CAttr_003Ej__TPar Attr
	{
		get
		{
			return _003CAttr_003Ei__Field;
		}
	}

	public _003CSerializedType_003Ej__TPar SerializedType
	{
		get
		{
			return _003CSerializedType_003Ei__Field;
		}
	}

	[DebuggerHidden]
	public _003C_003Ef__AnonymousType0(_003CType_003Ej__TPar Type, _003CAttr_003Ej__TPar Attr, _003CSerializedType_003Ej__TPar SerializedType)
	{
		_003CType_003Ei__Field = Type;
		_003CAttr_003Ei__Field = Attr;
		_003CSerializedType_003Ei__Field = SerializedType;
	}

	[DebuggerHidden]
	public override bool Equals(object value)
	{
		_003C_003Ef__AnonymousType0<_003CType_003Ej__TPar, _003CAttr_003Ej__TPar, _003CSerializedType_003Ej__TPar> anon = value as _003C_003Ef__AnonymousType0<_003CType_003Ej__TPar, _003CAttr_003Ej__TPar, _003CSerializedType_003Ej__TPar>;
		if (anon != null && EqualityComparer<_003CType_003Ej__TPar>.Default.Equals(_003CType_003Ei__Field, anon._003CType_003Ei__Field) && EqualityComparer<_003CAttr_003Ej__TPar>.Default.Equals(_003CAttr_003Ei__Field, anon._003CAttr_003Ei__Field))
		{
			return EqualityComparer<_003CSerializedType_003Ej__TPar>.Default.Equals(_003CSerializedType_003Ei__Field, anon._003CSerializedType_003Ei__Field);
		}
		return false;
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return ((171429106 * -1521134295 + EqualityComparer<_003CType_003Ej__TPar>.Default.GetHashCode(_003CType_003Ei__Field)) * -1521134295 + EqualityComparer<_003CAttr_003Ej__TPar>.Default.GetHashCode(_003CAttr_003Ei__Field)) * -1521134295 + EqualityComparer<_003CSerializedType_003Ej__TPar>.Default.GetHashCode(_003CSerializedType_003Ei__Field);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		object[] array = new object[3];
		_003CType_003Ej__TPar val = _003CType_003Ei__Field;
		ref _003CType_003Ej__TPar reference = ref val;
		_003CType_003Ej__TPar val2 = default(_003CType_003Ej__TPar);
		object obj;
		if (val2 == null)
		{
			val2 = reference;
			reference = ref val2;
			if (val2 == null)
			{
				obj = null;
				goto IL_0046;
			}
		}
		obj = reference.ToString();
		goto IL_0046;
		IL_0081:
		object obj2;
		array[1] = obj2;
		_003CSerializedType_003Ej__TPar val3 = _003CSerializedType_003Ei__Field;
		ref _003CSerializedType_003Ej__TPar reference2 = ref val3;
		_003CSerializedType_003Ej__TPar val4 = default(_003CSerializedType_003Ej__TPar);
		object obj3;
		if (val4 == null)
		{
			val4 = reference2;
			reference2 = ref val4;
			if (val4 == null)
			{
				obj3 = null;
				goto IL_00c0;
			}
		}
		obj3 = reference2.ToString();
		goto IL_00c0;
		IL_00c0:
		array[2] = obj3;
		return string.Format(null, "{{ Type = {0}, Attr = {1}, SerializedType = {2} }}", array);
		IL_0046:
		array[0] = obj;
		_003CAttr_003Ej__TPar val5 = _003CAttr_003Ei__Field;
		ref _003CAttr_003Ej__TPar reference3 = ref val5;
		_003CAttr_003Ej__TPar val6 = default(_003CAttr_003Ej__TPar);
		if (val6 == null)
		{
			val6 = reference3;
			reference3 = ref val6;
			if (val6 == null)
			{
				obj2 = null;
				goto IL_0081;
			}
		}
		obj2 = reference3.ToString();
		goto IL_0081;
	}
}
