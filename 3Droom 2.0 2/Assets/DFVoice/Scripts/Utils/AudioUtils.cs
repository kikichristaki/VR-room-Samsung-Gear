/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	using UnityEngine;

	public enum FrequencyMode
	{
		Narrow,
		Wide,
		UltraWide
	}

    public interface IFrequencyProvider
    {
        int GetFrequency( FrequencyMode mode );
    }

	public class AudioUtils
	{
        public static IFrequencyProvider FrequencyProvider = new SpeexCodec.FrequencyProvider();
		private static FastList<float> temp = new FastList<float>();

		public static int GetFrequency( FrequencyMode mode )
		{
            return FrequencyProvider.GetFrequency( mode );
		}

		public static void Resample( BigArray<float> samples, int oldFrequency, int newFrequency )
		{
			if( oldFrequency == newFrequency ) return;

			temp.Clear();
			float ratio = (float)oldFrequency / (float)newFrequency;
			int outSample = 0;
			while( true )
			{
				int inBufferIndex = (int)( outSample++ * ratio );
				if( inBufferIndex < samples.Length )
					temp.Add( samples[ inBufferIndex ] );
				else
					break;
			}

			samples.Resize( temp.Count );
			samples.CopyFrom( temp.Items, 0, 0, temp.Count * 4 );
		}

		public static void ApplyGain( float[] samples, float gain )
		{
			for( int i = 0; i < samples.Length; i++ )
				samples[ i ] *= gain;
		}

		public static float GetMaxAmplitude( float[] samples )
		{
			float max = 0f;
			for( int i = 0; i < samples.Length; i++ )
				max = Mathf.Max( max, Mathf.Abs( samples[ i ] ) );
			return max;
		}
	}
}