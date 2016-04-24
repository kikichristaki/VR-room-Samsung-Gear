/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	/// <summary>
	/// Represents an audio codec
	/// </summary>
	public interface IAudioCodec
	{
		/// <summary>
		/// Called when new audio data is received from the microphone
		/// </summary>
		void OnAudioAvailable( BigArray<float> rawPCM );

		/// <summary>
		/// Returns the next encoded frame, or NULL if there isn't one
		/// </summary>
		VoicePacketWrapper? GetNextEncodedFrame( int frequency );

		/// <summary>
		/// Decode the raw encoded frame into 32-bit float PCM
		/// </summary>
		BigArray<float> DecodeFrame( VoicePacketWrapper data );

		/// <summary>
		/// Generate filler data for a missing frame
		/// </summary>
		BigArray<float> GenerateMissingFrame( int frequency );
	}
}