/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	using UnityEngine;

	/// <summary>
	/// Event handler for when new audio data becomes available
	/// </summary>
	public delegate void AudioBufferReadyHandler( BigArray<float> newData, int frequency );

	/// <summary>
	/// Base class for audio input devices
	/// </summary>
	public abstract class AudioInputDeviceBase : MonoBehaviour
	{
		/// <summary>
		/// Called when new audio data becomes available
		/// </summary>
		public event AudioBufferReadyHandler OnAudioBufferReady;

		/// <summary>
		/// Start recording audio data
		/// </summary>
		public abstract void StartRecording();

		/// <summary>
		/// Stop recording audio data
		/// </summary>
		public abstract void StopRecording();

		protected void bufferReady( BigArray<float> newData, int frequency )
		{
			if( OnAudioBufferReady != null )
				OnAudioBufferReady( newData, frequency );
		}
	}
}