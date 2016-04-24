/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	public struct VoicePacketWrapper
	{
		/// <summary>
		/// The index of this voice packet (used to detect lost frames)
		/// </summary>
		public ulong Index;

		/// <summary>
		/// The frequency (audio frequency = freqID * 1000)
		/// </summary>
		public byte Frequency;

		/// <summary>
		/// The raw data which was sent
		/// </summary>
		public byte[] RawData;

		private byte[] tempHeaderData;

		public VoicePacketWrapper( ulong Index, int Frequency, byte[] RawData )
		{
			tempHeaderData = null;

			this.Index = Index;
			this.Frequency = (byte)( Frequency / 1000 );
			this.RawData = RawData;
		}

		public VoicePacketWrapper( ulong Index, byte Frequency, byte[] RawData )
		{
			tempHeaderData = null;

			this.Index = Index;
			this.Frequency = Frequency;
			this.RawData = RawData;
		}

		public VoicePacketWrapper( byte[] headers, byte[] rawData )
		{
			tempHeaderData = null;

			this.Index = System.BitConverter.ToUInt64( headers, 0 );
			this.Frequency = headers[ 8 ];
			this.RawData = rawData;
		}

		public byte[] ObtainHeaders()
		{
			tempHeaderData = TempArray<byte>.Obtain( 9 ); // 8 bytes for ulong + 1 byte

			// extract bytes from ulong
			byte b0 = (byte)( Index & 0x00000000000000ff );
			byte b1 = (byte)( ( Index & 0x000000000000ff00 ) >> 8 );
			byte b2 = (byte)( ( Index & 0x0000000000ff0000 ) >> 16 );
			byte b3 = (byte)( ( Index & 0x00000000ff000000 ) >> 24 );
			byte b4 = (byte)( ( Index & 0x000000ff00000000 ) >> 32 );
			byte b5 = (byte)( ( Index & 0x0000ff0000000000 ) >> 40 );
			byte b6 = (byte)( ( Index & 0x00ff000000000000 ) >> 48 );
			byte b7 = (byte)( ( Index & 0xff00000000000000 ) >> 56 );

			tempHeaderData[ 0 ] = b0;
			tempHeaderData[ 1 ] = b1;
			tempHeaderData[ 2 ] = b2;
			tempHeaderData[ 3 ] = b3;
			tempHeaderData[ 4 ] = b4;
			tempHeaderData[ 5 ] = b5;
			tempHeaderData[ 6 ] = b6;
			tempHeaderData[ 7 ] = b7;

			tempHeaderData[ 8 ] = Frequency;

			return tempHeaderData;
		}

		public void ReleaseHeaders()
		{
			TempArray<byte>.Release( tempHeaderData );
		}
	}
}