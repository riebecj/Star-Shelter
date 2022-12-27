using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
	public class VRTK_PolicyList : MonoBehaviour
	{
		public enum OperationTypes
		{
			Ignore = 0,
			Include = 1
		}

		public enum CheckTypes
		{
			Tag = 1,
			Script = 2,
			Layer = 4
		}

		[Tooltip("The operation to apply on the list of identifiers.")]
		public OperationTypes operation;

		[Tooltip("The element type on the game object to check against.")]
		public CheckTypes checkType = CheckTypes.Tag;

		[Tooltip("A list of identifiers to check for against the given check type (either tag or script).")]
		public List<string> identifiers = new List<string> { string.Empty };

		public virtual bool Find(GameObject obj)
		{
			if (operation == OperationTypes.Ignore)
			{
				return TypeCheck(obj, true);
			}
			return TypeCheck(obj, false);
		}

		public static bool Check(GameObject obj, VRTK_PolicyList list)
		{
			if ((bool)list)
			{
				return list.Find(obj);
			}
			return false;
		}

		protected virtual bool ScriptCheck(GameObject obj, bool returnState)
		{
			foreach (string identifier in identifiers)
			{
				if ((bool)obj.GetComponent(identifier))
				{
					return returnState;
				}
			}
			return !returnState;
		}

		protected virtual bool TagCheck(GameObject obj, bool returnState)
		{
			if (returnState)
			{
				return identifiers.Contains(obj.tag);
			}
			return !identifiers.Contains(obj.tag);
		}

		protected virtual bool LayerCheck(GameObject obj, bool returnState)
		{
			if (returnState)
			{
				return identifiers.Contains(LayerMask.LayerToName(obj.layer));
			}
			return !identifiers.Contains(LayerMask.LayerToName(obj.layer));
		}

		protected virtual bool TypeCheck(GameObject obj, bool returnState)
		{
			int num = 0;
			if ((checkType & CheckTypes.Tag) != 0)
			{
				num++;
			}
			if ((checkType & CheckTypes.Script) != 0)
			{
				num += 2;
			}
			if ((checkType & CheckTypes.Layer) != 0)
			{
				num += 4;
			}
			switch (num)
			{
			case 1:
				return TagCheck(obj, returnState);
			case 2:
				return ScriptCheck(obj, returnState);
			case 3:
				if ((returnState && TagCheck(obj, returnState)) || (!returnState && !TagCheck(obj, returnState)))
				{
					return returnState;
				}
				if ((returnState && ScriptCheck(obj, returnState)) || (!returnState && !ScriptCheck(obj, returnState)))
				{
					return returnState;
				}
				break;
			case 4:
				return LayerCheck(obj, returnState);
			case 5:
				if ((returnState && TagCheck(obj, returnState)) || (!returnState && !TagCheck(obj, returnState)))
				{
					return returnState;
				}
				if ((returnState && LayerCheck(obj, returnState)) || (!returnState && !LayerCheck(obj, returnState)))
				{
					return returnState;
				}
				break;
			case 6:
				if ((returnState && ScriptCheck(obj, returnState)) || (!returnState && !ScriptCheck(obj, returnState)))
				{
					return returnState;
				}
				if ((returnState && LayerCheck(obj, returnState)) || (!returnState && !LayerCheck(obj, returnState)))
				{
					return returnState;
				}
				break;
			case 7:
				if ((returnState && TagCheck(obj, returnState)) || (!returnState && !TagCheck(obj, returnState)))
				{
					return returnState;
				}
				if ((returnState && ScriptCheck(obj, returnState)) || (!returnState && !ScriptCheck(obj, returnState)))
				{
					return returnState;
				}
				if ((returnState && LayerCheck(obj, returnState)) || (!returnState && !LayerCheck(obj, returnState)))
				{
					return returnState;
				}
				break;
			}
			return !returnState;
		}
	}
}
