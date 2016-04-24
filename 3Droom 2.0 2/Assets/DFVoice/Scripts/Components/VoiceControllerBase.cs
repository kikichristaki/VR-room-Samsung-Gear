/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	using UnityEngine;
	using System.Collections.Generic;

	public abstract class VoiceControllerBase : MonoBehaviour
	{
		/// <summary>
		/// Gets whether this voice controller belongs to the local client or not
		/// </summary>
		public abstract bool IsLocal
		{
			get;
		}

		/// <summary>
		/// Gets the audio input device attached to this voice controller
		/// </summary>
		public AudioInputDeviceBase AudioInputDevice
		{
			get
			{
				return this.microphone;
			}
		}

		/// <summary>
		/// Gets the audio output device attached to this voice controller
		/// </summary>
		public IAudioPlayer AudioOutputDevice
		{
			get
			{
				return this.speaker;
			}
		}

		/// <summary>
		/// If true, play back received audio even if this belongs to the local client
		/// </summary>
		public bool DebugAudio = false;

		/// <summary>
		/// If true, voice controller won't decode and play back received frames
		/// </summary>
		public bool Mute = false;

		protected AudioInputDeviceBase microphone;
		protected IAudioPlayer speaker;
		protected IAudioCodec codec;

		protected ulong nextFrameIndex = 0;
		protected ulong nextExpectedIndex = 0;

		protected virtual void Awake()
		{
			codec = GetCodec();

			microphone = GetComponent<AudioInputDeviceBase>();
			speaker = GetComponent( typeof( IAudioPlayer ) ) as IAudioPlayer;

			if( microphone == null )
			{
				Debug.LogError( "No audio input component attached to speaker", this );
				return;
			}

			if( speaker == null )
			{
				Debug.LogError( "No audio output component attached to speaker", this );
				return;
			}

			if( IsLocal )
			{
				microphone.OnAudioBufferReady += this.OnMicrophoneDataReady;
				microphone.StartRecording();
			}
		}

		protected virtual void OnDestroy()
		{
			if( IsLocal && microphone != null )
			{
				microphone.OnAudioBufferReady -= this.OnMicrophoneDataReady;
				microphone.StopRecording();
			}
		}

		/// <summary>
		/// Called when a frame of audio is encoded and ready to send
		/// </summary>
		protected virtual void OnAudioDataEncoded( VoicePacketWrapper encodedFrame )
		{
			// TODO: send audio over network
		}

		/// <summary>
		/// Create a new codec
		/// </summary>
		protected virtual IAudioCodec GetCodec()
		{
            AudioUtils.FrequencyProvider = new SpeexCodec.FrequencyProvider();
            return new SpeexCodec( true );
		}

		/// <summary>
		/// If you need to skip receiving a frame, you should call this function so it can advance the next expected index counter
		/// </summary>
		protected void SkipFrame()
		{
			nextExpectedIndex++;
		}

		/// <summary>
		/// Decode and play back received audio data
		/// </summary>
		protected virtual void ReceiveAudioData( VoicePacketWrapper encodedFrame )
		{
			if( !IsLocal || DebugAudio )
			{
				// discard old samples
				if( encodedFrame.Index < nextExpectedIndex ) return;

				// voice controller is muted - don't bother decoding or buffering audio data
				if( Mute )
				{
					nextExpectedIndex = encodedFrame.Index + 1;
					return;
				}

				speaker.SetSampleRate( encodedFrame.Frequency * 1000 );

				// some frames were lost, generate filler data for them
				// unless the speaker isn't playing any sound, in which case filler data will only delay the stream further
				// OR unless nextExpectedIndex is zero, implying that we haven't received any frames yet
				if( nextExpectedIndex != 0 && encodedFrame.Index != nextExpectedIndex && speaker.PlayingSound )
				{
					int numMissingFrames = (int)( encodedFrame.Index - nextExpectedIndex );

					for( int i = 0; i < numMissingFrames; i++ )
					{
						BigArray<float> filler = codec.GenerateMissingFrame( encodedFrame.Frequency );
						speaker.BufferAudio( filler );
					}
				}

				BigArray<float> decoded = codec.DecodeFrame( encodedFrame );
				speaker.BufferAudio( decoded );

				nextExpectedIndex = encodedFrame.Index + 1;
			}
		}

		/// <summary>
		/// Called when new audio is available from the microphone
		/// </summary>
		protected virtual void OnMicrophoneDataReady( BigArray<float> newData, int frequency )
		{
			if( !IsLocal ) return;

			codec.OnAudioAvailable( newData );

			VoicePacketWrapper? enc = codec.GetNextEncodedFrame( frequency );
			while( enc.HasValue )
			{
				// assign index
				VoicePacketWrapper packet = enc.Value;
				packet.Index = nextFrameIndex++;
				enc = packet;

				OnAudioDataEncoded( enc.Value );
				enc = codec.GetNextEncodedFrame( frequency );
			}
		}
	}
}