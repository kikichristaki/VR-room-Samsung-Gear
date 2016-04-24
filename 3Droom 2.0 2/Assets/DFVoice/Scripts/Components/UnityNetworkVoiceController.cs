/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	using UnityEngine;

	public class UnityNetworkVoiceController : VoiceControllerBase
	{
		public override bool IsLocal
		{
			get { return GetComponent<NetworkView>().isMine; }
		}

		protected override void Awake()
		{
			base.Awake();

			VoiceControllerCollection<UnityNetworkVoiceController>.RegisterVoiceController( this );
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			VoiceControllerCollection<UnityNetworkVoiceController>.UnregisterVoiceController( this );
		}

		protected override void OnAudioDataEncoded( VoicePacketWrapper encodedFrame )
		{
			byte[] headers = encodedFrame.ObtainHeaders();
			GetComponent<NetworkView>().RPC( "vc", RPCMode.All, headers, encodedFrame.RawData );
			encodedFrame.ReleaseHeaders();
		}

		[PunRPC]
		void vc( byte[] headers, byte[] rawData )
		{
			VoicePacketWrapper packet = new VoicePacketWrapper( headers, rawData );
			ReceiveAudioData( packet );
		}
	}
}