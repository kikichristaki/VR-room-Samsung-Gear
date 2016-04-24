/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	using UnityEngine;

	[AddComponentMenu( "DFVoice/Microphone Input Device" )]
	public class MicrophoneInputDevice : AudioInputDeviceBase
	{
		public static string DefaultMicrophone = null;

		/// <summary>
		/// Gets the device that is actively being used to record audio
		/// </summary>
		public string ActiveDevice
		{
			get
			{
				return device;
			}
		}

		public int ChunkSize = 640;
		public FrequencyMode Mode = FrequencyMode.Wide;

		public KeyCode PushToTalk = KeyCode.None;

		public float AmplitudeThreshold = 0f;

		private AudioClip recordedAudio;
		private int prevReadPosition = 0;
		private BigArray<float> resampleBuffer;

		private string device = null;
		private int recordingFrequency;

		private float pushToTalkTimer = 0f;

		public override void StartRecording()
		{
			if( !Application.HasUserAuthorization( UserAuthorization.Microphone ) )
			{
				Debug.LogWarning( "StartRecording(): Webplayer microphone access denied" );
				return;
			}

			device = DefaultMicrophone;

			prevReadPosition = 0;

			this.recordingFrequency = AudioUtils.GetFrequency( Mode );

			int min, max;
			Microphone.GetDeviceCaps( device, out min, out max );

			if( max == 0 ) max = 48000;
			//if( max == 0 ) max = 16000;
			
			int frequency = Mathf.Clamp( this.recordingFrequency, min, max );

			resampleBuffer = new BigArray<float>( ChunkSize, 0 );
			recordedAudio = Microphone.Start( device, true, 5, frequency );
		}

		public override void StopRecording()
		{
			Microphone.End( device );
		}

		/// <summary>
		/// Switch over to a new device
		/// </summary>
		public void ChangeMicrophoneDevice( string newDevice )
		{
			StopRecording();

			DefaultMicrophone = newDevice;

			StartRecording();
		}

		void Update()
		{
			if( !Microphone.IsRecording( device ) || recordedAudio == null )
			{
				return;
			}

			float[] tempArray = TempArray<float>.Obtain( ChunkSize );

			// in case of recompile
			if( resampleBuffer == null )
			{
				resampleBuffer = new BigArray<float>( ChunkSize, 0 );
			}

			int readPosition = Microphone.GetPosition( device );

			if( readPosition >= ( prevReadPosition + ChunkSize ) )
			{
				while( readPosition >= ( prevReadPosition + ChunkSize ) )
				{
					if( canTalk() )
					{
						recordedAudio.GetData( tempArray, prevReadPosition );
						if( exceedsVolumeThreshold( tempArray ) )
						{
							resample( tempArray );
							bufferReady( resampleBuffer, this.recordingFrequency );
						}
					}

					prevReadPosition += ChunkSize;
				}
			}
			else if( prevReadPosition > readPosition )
			{
				var endReadPos = readPosition + recordedAudio.samples;
				var diff = endReadPos - prevReadPosition;
				while( diff >= ChunkSize )
				{
					if( canTalk() )
					{
						recordedAudio.GetData( tempArray, prevReadPosition );
						if( exceedsVolumeThreshold( tempArray ) )
						{
							resample( tempArray );
							bufferReady( resampleBuffer, this.recordingFrequency );
						}
					}

					prevReadPosition += ChunkSize;
					if( prevReadPosition >= recordedAudio.samples )
					{
						prevReadPosition -= recordedAudio.samples;
						break;
					}

					endReadPos = readPosition + recordedAudio.samples;
					diff = endReadPos - prevReadPosition;
				}
			}

			TempArray<float>.Release( tempArray );
		}

		bool exceedsVolumeThreshold( float[] data )
		{
			if( AmplitudeThreshold == 0f )
				return true;

			var max = Mathf.Max( data );
			return max >= AmplitudeThreshold;
		}

		void resample( float[] tempArray )
		{
			//resampleBuffer = new BigArray<float>( tempArray.Length, tempArray.Length );
			resampleBuffer.Resize( tempArray.Length );
			resampleBuffer.CopyFrom( tempArray, 0, 0, tempArray.Length * 4 );
			//Debug.Log( "Resampling from: " + recordedAudio.frequency + ", to: " + this.recordingFrequency );
			AudioUtils.Resample( resampleBuffer, recordedAudio.frequency, this.recordingFrequency );
		}

		bool canTalk()
		{
			if( PushToTalk == KeyCode.None ) return true;
			if( Input.GetKey( PushToTalk ) ) pushToTalkTimer = 0.2f;

			pushToTalkTimer -= Time.deltaTime;
			return pushToTalkTimer > 0f;
		}
	}
}