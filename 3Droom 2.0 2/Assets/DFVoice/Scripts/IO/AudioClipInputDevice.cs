/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
    using UnityEngine;
    using System.Collections;

    public class AudioClipInputDevice : AudioInputDeviceBase
    {
        public AudioClip testClip;
        public FrequencyMode ResampleFrequency = FrequencyMode.Wide;

        public override void StartRecording()
        {
            float[] data = new float[ testClip.samples * testClip.channels ];
            testClip.GetData( data, 0 );

            BigArray<float> d = new BigArray<float>( data.Length, 0 );
            d.Resize( data.Length );
            d.CopyFrom( data, 0, 0, data.Length * 4 );

            //AudioUtils.Resample( d, testClip.frequency, AudioUtils.GetFrequency( ResampleFrequency ) );

            //bufferReady( d, AudioUtils.GetFrequency( ResampleFrequency ) );
            StartCoroutine( yieldChunks( d, testClip.frequency, 1f ) );
        }

        private IEnumerator yieldChunks( BigArray<float> data, int chunkSize, float chunkDuration )
        {
            int readHead = 0;

            while( readHead < data.Length )
            {
                int remainder = chunkSize;
                if( readHead + chunkSize >= data.Length )
                {
                    remainder = data.Length - readHead;
                }

                BigArray<float> temp = new BigArray<float>( remainder, 0 );
                temp.Resize( remainder );
                temp.CopyFrom( data.Items, readHead * 4, 0, remainder * 4 );
                AudioUtils.Resample( temp, testClip.frequency, AudioUtils.GetFrequency( ResampleFrequency ) );

                bufferReady( temp, AudioUtils.GetFrequency( ResampleFrequency ) );

                readHead += remainder;

                yield return new WaitForSeconds( chunkDuration );
            }
        }

        public override void StopRecording()
        {
        }
    }
}