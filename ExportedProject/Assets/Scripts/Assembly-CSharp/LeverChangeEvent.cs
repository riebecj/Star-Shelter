using System;
using UnityEngine.Events;

[Serializable]
public class LeverChangeEvent : UnityEvent<VRLever, float, float>
{
}
