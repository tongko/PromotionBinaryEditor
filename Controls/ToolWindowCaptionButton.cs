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
		public ToolWindowCaptionButton(ToolWindow parent, CaptionButtonType buttonType)
		{
			Parent = parent;
			ButtonType = buttonType;
		}

		public CaptionButtonType ButtonType { get; private set; }

		public ToolWindow Parent { get; set; }

		public Rectangle Bounds { get; set; }

		public bool Highlight { get; set; }

		public bool Checked { get; set; }

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
		public Bitmap[] ImageList { get; set; }

		public bool ContainsPoint(Point point)
		{
			return Bounds.Contains(point);
		}

		public virtual Bitmap GetImage()
		{
			if (ImageList == null || ImageList.Length != 5)
				return null;

			if (Checked)
				return ImageList[4];
			var index = 0;

			if (Parent.Focus()) index += 2;
			if (Highlight) index++;

			return ImageList[index];
		}
	}
}
