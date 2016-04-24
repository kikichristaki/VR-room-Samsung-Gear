/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	using System.Collections.Generic;

	/// <summary>
	/// Helper class for when you need a fast way to iterate voice controllers in the scene
	/// </summary>
	public class VoiceControllerCollection<T> where T : VoiceControllerBase
	{
		public static List<T> VoiceControllers = new List<T>();

		public static void RegisterVoiceController( T controller )
		{
			VoiceControllers.Add( controller );
		}

		public static void UnregisterVoiceController( T controller )
		{
			VoiceControllers.Remove( controller );
		}
	}
}