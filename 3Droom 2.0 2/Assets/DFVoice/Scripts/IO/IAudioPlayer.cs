/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	/// <summary>
	/// Interface for classes which can play received audio
	/// </summary>
	public interface IAudioPlayer
	{
		bool PlayingSound { get; }
		void SetSampleRate( int sampleRate );
		void BufferAudio( BigArray<float> audioData );
	}
}