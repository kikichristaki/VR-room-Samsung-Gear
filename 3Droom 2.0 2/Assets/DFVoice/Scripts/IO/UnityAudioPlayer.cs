/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	using UnityEngine;

	/// <summary>
	/// Implements an audio player which writes to a streaming audio clip
	/// </summary>
	[AddComponentMenu( "DFVoice/Unity Audio Player" )]
	[RequireComponent( typeof( AudioSource ) )]
	public class UnityAudioPlayer : MonoBehaviour, IAudioPlayer
	{
		public bool PlayingSound
		{
			get
			{
				return GetComponent<AudioSource>().isPlaying;
			}
		}

		public bool IsThreeDimensional = false;

		public bool Equalize = false;
		public float EqualizeSpeed = 1f;
		public float TargetEqualizeVolume = 0.75f;
		public float MaxEqualization = 5f;

		private int frequency = 16000;

		private int writeHead = 0;
		private int totalWritten = 0;
		private AudioClip playClip;

		private int delayForFrames = 0;
		private int lastTime = 0;
		private int played = 0;

		private float currentGain = 1f;
		private float targetGain = 1f;

		void Start()
		{
			playClip = AudioClip.Create( "vc", frequency * 10, 1, frequency, false );

			// backwards compatibility
			if( GetComponent<AudioSource>() == null )
				gameObject.AddComponent<AudioSource>();

			GetComponent<AudioSource>().clip = playClip;
			GetComponent<AudioSource>().Stop();
			GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().spatialBlend = IsThreeDimensional ? 1f : 0f;
		}

		void Update()
		{
			if( GetComponent<AudioSource>().isPlaying )
			{
				if( lastTime > GetComponent<AudioSource>().timeSamples )
				{
					played += GetComponent<AudioSource>().clip.samples;
				}

				lastTime = GetComponent<AudioSource>().timeSamples;

				currentGain = Mathf.MoveTowards( currentGain, targetGain, Time.deltaTime * EqualizeSpeed );

				if( played + GetComponent<AudioSource>().timeSamples >= totalWritten )
				{
					GetComponent<AudioSource>().Pause();
					delayForFrames = 2;
				}
			}
		}

		void OnDestroy()
		{
			Destroy( GetComponent<AudioSource>().clip );
		}

		public void SetSampleRate( int sampleRate )
		{
			if( GetComponent<AudioSource>() == null ) return;
			
			if( GetComponent<AudioSource>().clip != null && GetComponent<AudioSource>().clip.frequency == sampleRate ) return;

			this.frequency = sampleRate;

			if( GetComponent<AudioSource>().clip != null )
				Destroy( GetComponent<AudioSource>().clip );
			
			playClip = AudioClip.Create( "vc", frequency * 10, 1, frequency, false );
			GetComponent<AudioSource>().clip = playClip;
			GetComponent<AudioSource>().Stop();
			GetComponent<AudioSource>().loop = true;

			writeHead = 0;
			totalWritten = 0;
			delayForFrames = 0;
			lastTime = 0;
			played = 0;
		}

		public void BufferAudio( BigArray<float> audioData )
		{
			if( GetComponent<AudioSource>() == null ) return;

			float[] temp = TempArray<float>.Obtain( audioData.Length );
			audioData.CopyTo( 0, temp, 0, audioData.Length * 4 );

			if( Equalize )
			{
				float maxAmp = AudioUtils.GetMaxAmplitude( temp );
				targetGain = TargetEqualizeVolume / maxAmp;

				if( targetGain > MaxEqualization )
					targetGain = MaxEqualization;

				if( targetGain < currentGain )
				{
					currentGain = targetGain;
				}

				AudioUtils.ApplyGain( temp, currentGain );
			}

			playClip.SetData( temp, writeHead );
			TempArray<float>.Release( temp );

			writeHead += audioData.Length;
			totalWritten += audioData.Length;
			writeHead %= playClip.samples;

			if( !GetComponent<AudioSource>().isPlaying )
			{
				delayForFrames--;
				if( delayForFrames <= 0 )
				{
					GetComponent<AudioSource>().Play();
				}
			}
		}
	}
}