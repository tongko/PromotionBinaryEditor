using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BinEdit.Controls
{
	public class ToolWindowCaption
	{
		private Rectangle[] _boundses;

		private string _text;
		private List<ToolWindowCaptionButton> _buttons;

		public ToolWindowCaption(ToolWindow parent)
		{
			Parent = parent;
		}

		public ToolWindow Parent { get; set; }

		/// <summary>
		///	Handle image must have 2 bitmap, first bitmap
		/// represent normal state, second represent
		/// Parent have Focus.
		/// </summary>
		public Bitmap[] HandleImage { get; set; }

		public void AddButton(ToolWindowCaptionButton button)
		{
			_buttons.Add(button);
		}

		public void HitTest(MouseEventArgs e)
		{
			if (!_boundses[0].Contains(e.Location))
				return;

			for (var i = 0; i < _buttons.Count; i++)
			{
				if (_buttons[i].Bounds.Contains(e.Location))
				{
					if (e.Button == MouseButtons.Left)
					{
						_buttons[i].Checked = !_buttons[i].Checked;
						return;
					}

					_buttons[i].Highlight = !_buttons[i].Highlight;
				}
			}
		}

		private void InvalidateBounds()
		{
		}
	}
}
