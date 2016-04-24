/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	using System.Collections.Generic;

	/// <summary>
	/// Helper class for working with pooled arrays
	/// </summary>
	public class TempArray<T>
	{
		private static Dictionary<int, Queue<T[]>> pool = new Dictionary<int, Queue<T[]>>();

		/// <summary>
		/// Obtain an array of the given length
		/// </summary>
		public static T[] Obtain( int length )
		{
			Queue<T[]> pool = getPool( length );
			if( pool.Count > 0 )
			{
				return pool.Dequeue();
			}
			return new T[ length ];
		}

		/// <summary>
		/// Release an array back to the pool
		/// </summary>
		public static void Release( T[] array )
		{
			pool[ array.Length ].Enqueue( array );
		}

		private static Queue<T[]> getPool( int length )
		{
			if( !pool.ContainsKey( length ) )
			{
				pool[ length ] = new Queue<T[]>();
			}
			return pool[ length ];
		}
	}
}