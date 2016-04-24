/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	using System.Collections.Generic;

	public class SpeexCodec : IAudioCodec
    {
        public class FrequencyProvider : IFrequencyProvider
        {
            public int GetFrequency( FrequencyMode mode )
            {
                switch( mode )
                {
                    case FrequencyMode.Narrow:
                        return 8000;
                    case FrequencyMode.Wide:
                        return 16000;
                    case FrequencyMode.UltraWide:
                        return 32000;
                    default:
                        return 16000;
                }
            }
        }

		private class codecWrapper
		{
			public NSpeex.SpeexEncoder encoder;
			public NSpeex.SpeexDecoder decoder;

			public codecWrapper( NSpeex.BandMode mode, bool vbr )
			{
				encoder = new NSpeex.SpeexEncoder( mode );
				decoder = new NSpeex.SpeexDecoder( mode, false );

				encoder.VBR = vbr;
				encoder.Quality = 5;
			}
		}

		private Dictionary<int, codecWrapper> encoders;

		private Dictionary<int, int> frameSizes = new Dictionary<int, int>()
		{
			{ 8000, 160 },
			{ 16000, 320 },
			{ 32000, 640 },
		};

		private ChunkBuffer chunkBuffer;
		private BigArray<float> tempOutputArray;
		private VoicePacketWrapper tempPacketWrapper;

		public SpeexCodec( bool VBR )
		{
			encoders = new Dictionary<int, codecWrapper>()
			{
				{ 8000, new codecWrapper( NSpeex.BandMode.Narrow, VBR ) },
				{ 16000, new codecWrapper( NSpeex.BandMode.Wide, VBR ) },
				{ 32000, new codecWrapper( NSpeex.BandMode.UltraWide, VBR ) },
			};

			chunkBuffer = new ChunkBuffer();
			tempOutputArray = new BigArray<float>( 1024, 0 );
			tempPacketWrapper = new VoicePacketWrapper( 0, 16, new byte[ 0 ] );
		}

		public void OnAudioAvailable( BigArray<float> rawPCM )
		{
			chunkBuffer.AddSamples( rawPCM );
		}

		public VoicePacketWrapper? GetNextEncodedFrame( int frequency )
		{
			int frameSize = frameSizes[ frequency ];
			codecWrapper codec = encoders[ frequency ];

			float[] chunk = TempArray<float>.Obtain( frameSize );
			bool chunkAvailable = chunkBuffer.RetrieveChunk( chunk );
			if( !chunkAvailable )
			{
				TempArray<float>.Release( chunk );
				return null;
			}

			tempPacketWrapper = new VoicePacketWrapper();
			tempPacketWrapper.Frequency = (byte)( frequency / 1000 );

			short[] audio16bit = TempArray<short>.Obtain( frameSize );
			for( int i = 0; i < frameSize; i++ )
			{
				float val = chunk[ i ] * short.MaxValue;
				audio16bit[ i ] = (short)val;
			}
			TempArray<float>.Release( chunk );

			byte[] buffer = TempArray<byte>.Obtain( audio16bit.Length * 2 );

			int encoded = codec.encoder.Encode( audio16bit, 0, frameSize, buffer, 0, buffer.Length );

			TempArray<short>.Release( audio16bit );

			tempPacketWrapper.RawData = new byte[ encoded ];

			System.Buffer.BlockCopy( buffer, 0, tempPacketWrapper.RawData, 0, encoded );

			TempArray<byte>.Release( buffer );

			return tempPacketWrapper;
		}

		public BigArray<float> DecodeFrame( VoicePacketWrapper data )
		{
			int frameSize = frameSizes[ data.Frequency * 1000 ];
			codecWrapper codec = encoders[ data.Frequency * 1000 ];

			short[] decodedFrame16Bit = TempArray<short>.Obtain( frameSize * 4 );
			int decoded = codec.decoder.Decode( data.RawData, 0, data.RawData.Length, decodedFrame16Bit, 0, false );

			if( tempOutputArray.Length != decoded ) tempOutputArray.Resize( decoded );

			for( int i = 0; i < decoded; i++ )
			{
				float val = (float)decodedFrame16Bit[ i ];
				val /= short.MaxValue;
				tempOutputArray[ i ] = val;
			}

			TempArray<short>.Release( decodedFrame16Bit );

			return tempOutputArray;
		}

		public BigArray<float> GenerateMissingFrame( int frequency )
		{
			int frameSize = frameSizes[ frequency * 1000 ];
			codecWrapper codec = encoders[ frequency * 1000 ];

			short[] decodedFrame16Bit = TempArray<short>.Obtain( frameSize * 4 );
			int decoded = codec.decoder.Decode( null, 0, 0, decodedFrame16Bit, 0, true );

			if( tempOutputArray.Length != decoded ) tempOutputArray.Resize( decoded );

			for( int i = 0; i < decoded; i++ )
			{
				float val = (float)decodedFrame16Bit[ i ];
				val /= short.MaxValue;
				tempOutputArray[ i ] = val;
			}

			TempArray<short>.Release( decodedFrame16Bit );

			return tempOutputArray;
		}
	}
}