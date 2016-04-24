/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	using UnityEngine;

	public class TestLocalVoiceController : VoiceControllerBase
	{
		public float PacketLoss = 0.1f;

		protected override void OnAudioDataEncoded( VoicePacketWrapper encodedFrame )
		{
			if( Random.Range( 0f, 1f ) <= PacketLoss )
			{
				return;
			}

			ReceiveAudioData( encodedFrame );
		}

		public override bool IsLocal
		{
			get { return true; }
		}
	}
}