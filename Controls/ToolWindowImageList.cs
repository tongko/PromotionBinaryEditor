using System;
using System.Collections.Generic;
using System.Drawing;

namespace BinEdit.Controls
{
	public enum ImageType
	{
		Normal,
		Hover,
		Focus,
		FocusHover,
		Clieked
	}

	public class ToolWindowImageList : IEnumerable<Image>
	{
		#region Fields

		private const int MaxImages = 5;

		private readonly Image[] _images;
		private int _version;

		#endregion


		#region Contrsuctors

		public ToolWindowImageList()
		{
			_images = new Image[MaxImages];
			_version = 0;
		}

		#endregion


		#region Properties

		public Image this[ImageType index]
		{
			get { return _images[(int)index]; }
			set
			{
				_images[(int)index] = value;
				_version++;
			}
		}

		#endregion


		#region Methdods

		protected virtual IEnumerator<Image> GetEnumerator()
		{
			return new ImageListEnumerator(this);
		}

		#endregion


		#region Interface Imeplementation

		IEnumerator<Image> IEnumerable<Image>.GetEnumerator()
		{
			return GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion


		#region Sub Classes

		private class ImageListEnumerator : IEnumerator<Image>
		{
			private readonly ToolWindowImageList _list;
			private int _index;
			private int _version;
			private Image _current;

			internal ImageListEnumerator(ToolWindowImageList list)
			{
				_list = list;
				_index = 0;
				_version = list._version;
				_current = null;
			}

			public Image Current
			{
				get { return _current; }
			}

			Image IEnumerator<Image>.Current
			{
				get
				{
					if (_index == 0 || _index > MaxImages)
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");

					return _current;
				}
			}

			public void Dispose()
			{
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					if (_index == 0 || _index > MaxImages)
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");

					return _current;
				}
			}

			public bool MoveNext()
			{
				var list = _list;
				if (_version != list._version || _index > 5)
					return MoveNextRare();

				_current = list._images[_index];
				++_index;
				return true;
			}

			private bool MoveNextRare()
			{
				if (_version != _list._version)
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

				_index = MaxImages + 1;
				_current = null;
				return false;

			}

			void System.Collections.IEnumerator.Reset()
			{
				if (_version != _list._version)
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				_index = 0;
				_current = null;
			}
		}

		#endregion
	}
}
