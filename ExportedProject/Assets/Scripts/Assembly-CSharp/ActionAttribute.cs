using System;
using UnityEngine;

public class ActionAttribute : PropertyAttribute
{
	public Type returnType;

	public Type[] paramTypes;

	public Delegate method;

	public ActionAttribute(Type returnType = null, params Type[] paramTypes)
	{
		this.returnType = ((returnType == null) ? typeof(void) : returnType);
		this.paramTypes = ((paramTypes == null) ? new Type[0] : paramTypes);
	}
}
