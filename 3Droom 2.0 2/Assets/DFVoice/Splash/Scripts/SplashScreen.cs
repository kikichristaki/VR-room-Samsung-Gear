/* Copyright (C) 2014 DaikonForge */

using UnityEngine;
using System.Collections;

namespace DaikonForge
{
	public class SplashScreen : MonoBehaviour
	{
		public float SplashDuration = 3.5f;
		public string LoadScene = "";

		IEnumerator Start()
		{
			if( string.IsNullOrEmpty( LoadScene ) )
			{
				Debug.LogWarning( "SplashScreen: Load scene not set" );
			}
			else
			{
				yield return new WaitForSeconds( SplashDuration );
				Application.LoadLevel( LoadScene );
			}
		}
	}
}