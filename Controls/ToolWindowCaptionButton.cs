using System.Drawing;
using System.Windows.Forms;

namespace BinEdit.Controls
{
	public enum CaptionButtonType
	{
		Button,
		CheckButton
	}

	public class ToolWindowCaptionButton
	{
		private Rectangle _bounds;
		private bool _highlight;
		private bool _checked;

		public ToolWindowCaptionButton(ToolWindow parent, CaptionButtonType buttonType)
		{
			Parent = parent;
			ButtonType = buttonType;
			_bounds = Rectangle.Empty;
		}

		public CaptionButtonType ButtonType { get; private set; }

		public ToolWindow Parent { get; set; }

		public Rectangle Bounds { get { return _bounds; } }

		public bool Highlight
		{
			get { return _highlight; }
			set
			{
				_highlight = value;
				Parent.Invalidate(_bounds);
			}
		}

		public bool Checked
		{
			get { return _checked; }
			set
			{
				_checked = value;
				Parent.Invalidate(_bounds);
			}
		}

		public ContextMenu ContextMenu { get; set; }

		/// <summary>
		/// Image size must be 15 x 15 for best visual effect.
		/// Image list shall contain images fot this event:
		/// Index		Description
		/// -----		-----------------------------------
		///		0		Normal - Image for no highlight, no checked, no parent focus
		///		1		highlighted - Image for Highlighted
		///		2		Focus - Image for parent Focus
		///		3		Focus Highlighted - Image Highlighted + parent focus
		///		4		Checked - Image for checked state.
		/// </summary>
		public Image[] ImageList { get; set; }

		public virtual void SetBounds(int left, int top, int width, int height)
		{
			_bounds.X = left;
			_bounds.Y = top;
			_bounds.Width = width;
			_bounds.Height = height;
		}

		public virtual Image GetImage()
		{
			if (ImageList == null || ImageList.Length != 5)
				return null;

			if (Checked)
				return ImageList[4];
			var index = 0;

			if (Parent.IsFocused) index += 2;
			if (Highlight) index++;

			return ImageList[index];
		}
	}
}
