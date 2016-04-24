/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	using System.Collections.Generic;

	/// <summary>
	/// Helper class for when you need a way to index voice controllers by some key
	/// </summary>
	public class VoiceControllerKeyedCollection<K,T> where T : VoiceControllerBase
	{
		protected static Dictionary<K, T> voiceControllers = new Dictionary<K, T>();

		public static void RegisterVoiceController( K key, T controller )
		{
			voiceControllers.Add( key, controller );
		}

		public static void UnregisterVoiceController( K key )
		{
			voiceControllers.Remove( key );
		}

		public static T GetVoiceController( K key )
		{
			if( !voiceControllers.ContainsKey( key ) )
			{
				return null;
			}
			return voiceControllers[ key ];
		}
	}
}