using System;
using MidiJack;
using UnityEngine;

namespace Reaktion
{
	[Serializable]
	public class Remote
	{
		public enum Control
		{
			Off = 0,
			MidiKnob = 1,
			MidiNote = 2,
			InputAxis = 3
		}

		[SerializeField]
		private Control _control;

		[SerializeField]
		private MidiChannel _midiChannel = MidiChannel.All;

		[SerializeField]
		private int _knobIndex = 2;

		[SerializeField]
		private int _noteNumber = 40;

		[SerializeField]
		private string _inputAxis = "Jump";

		[SerializeField]
		private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		private float _defaultLevel;

		private float _level;

		public float level
		{
			get
			{
				return _level;
			}
		}

		public void Reset(float defaultLevel)
		{
			_defaultLevel = defaultLevel;
			Update();
		}

		public void Update()
		{
			if (_control == Control.Off)
			{
				_level = _defaultLevel;
			}
			else if (_control == Control.MidiKnob)
			{
				_level = MidiMaster.GetKnob(_midiChannel, _knobIndex, _defaultLevel);
			}
			else if (_control == Control.MidiNote)
			{
				_level = MidiMaster.GetKey(_midiChannel, _noteNumber);
			}
			else if (string.IsNullOrEmpty(_inputAxis))
			{
				_level = _defaultLevel;
			}
			else
			{
				_level = Input.GetAxis(_inputAxis);
			}
			_level = _curve.Evaluate(_level);
		}
	}
}
