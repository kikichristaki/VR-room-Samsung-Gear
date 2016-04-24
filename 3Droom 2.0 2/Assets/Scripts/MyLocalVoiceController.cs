using UnityEngine;
using System.Collections;

using DaikonForge.VoIP;
using System;

public class MyLocalVoiceController : VoiceControllerBase
{
    public PhotonView photonView;

    private DateTime lastTalking = DateTime.Now.AddMinutes(-1);

    public override bool IsLocal
    {
        get { return photonView.isMine; }
    }

    protected override void OnAudioDataEncoded( VoicePacketWrapper encodedFrame )
    { 
        byte[] headers = encodedFrame.ObtainHeaders();
        GetComponent<PhotonView>().RPC("vc", PhotonTargets.Others, headers, encodedFrame.RawData);
        encodedFrame.ReleaseHeaders();
        
        lastTalking = DateTime.Now;
    }

    [PunRPC]
    void vc( byte[] headers, byte[] rawData )
    {
        if (!GetComponent<AudioSource>().enabled) return;

        VoicePacketWrapper packet = new VoicePacketWrapper( headers, rawData );
        ReceiveAudioData( packet );
    }

    public bool isTalking ()
    {
        return DateTime.Now.CompareTo(lastTalking.AddMilliseconds(100)) == -1;
    }
}