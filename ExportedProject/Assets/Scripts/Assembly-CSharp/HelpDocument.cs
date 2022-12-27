using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Doc", menuName = "HelpDocument")]
public class HelpDocument : ScriptableObject
{
	public string title = "Title";

	[ShowInInspector]
	[MultiLineProperty(20)]
	public string breadText = "bread text";
}
