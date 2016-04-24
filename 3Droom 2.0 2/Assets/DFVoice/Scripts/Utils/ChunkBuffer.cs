/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	/// <summary>
	/// Helper class for separating input microphone data into chunks of a certain number of samples
	/// </summary>
	public class ChunkBuffer
	{
		//private List<float> samples = new List<float>();
		private FastList<float> samples = new FastList<float>();

		/// <summary>
		/// Add incoming samples to the buffer
		/// </summary>
		public void AddSamples( BigArray<float> incomingSamples )
		{
			int writeIndex = samples.Count * 4;
			int newLength = samples.Count + incomingSamples.Length;
			int writeBytes = incomingSamples.Length * 4;
			samples.EnsureCapacity( newLength );
			samples.ForceCount( newLength );
			incomingSamples.CopyTo( 0, samples.Items, writeIndex, writeBytes );

			//for( int i = 0; i < incomingSamples.Length; i++ )
			//{
			//	samples[ writeIndex + i ] = incomingSamples[ i ];
			//}
		}

		/// <summary>
		/// Retrieve a chunk of the given size and fill destination array with samples
		/// </summary>
		/// <returns>True if a chunk is available, false otherwise</returns>
		public bool RetrieveChunk( float[] destination )
		{
			if( samples.Count < destination.Length ) return false;
			for( int i = 0; i < destination.Length; i++ )
			{
				destination[ i ] = samples[ i ];
			}
			samples.RemoveRange( 0, destination.Length );
			return true;
		}
	}
}