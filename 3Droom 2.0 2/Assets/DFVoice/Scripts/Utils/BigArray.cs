/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	public class BigArray<T>
	{
		private T[] items;
		private int count;

		public BigArray( int capacity, int count )
		{
			items = new T[ capacity ];
			this.count = count;
		}

		public T this[ int index ]
		{
			get
			{
				if( index >= count )
					throw new System.IndexOutOfRangeException();

				return items[ index ];
			}
			set
			{
				if( index >= count )
					throw new System.IndexOutOfRangeException();

				items[ index ] = value;
			}
		}

		public int Length
		{
			get
			{
				return count;
			}
		}

		public T[] Items
		{
			get
			{
				return items;
			}
		}

		public void Resize( int newSize )
		{
			this.count = newSize;
			if( this.items.Length < newSize )
			{
				System.Array.Resize<T>( ref this.items, newSize * 2 );
			}
		}

		public void CopyTo( int startIndex, BigArray<T> destination, int destIndex, int count )
		{
			System.Buffer.BlockCopy( items, startIndex, destination.items, destIndex, count );
		}

		public void CopyTo( int startIndex, T[] destination, int destIndex, int count )
		{
			System.Buffer.BlockCopy( items, startIndex, destination, destIndex, count );
		}

		public void CopyFrom( T[] source, int sourceIndex, int destIndex, int count )
		{
			System.Buffer.BlockCopy( source, sourceIndex, this.items, destIndex, count );
		}
	}
}