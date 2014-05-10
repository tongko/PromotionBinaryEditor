using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BinEdit.Controls
{
	public class ToolWindowCaption
	{
		#region Fields

		private const int Margin = 5;
		private const int CaptionHeight = 21;

		private static readonly Font CaptionFont = new Font("Segoe UI", 9f, FontStyle.Regular);
		private static readonly Brush BrushBackground = new SolidBrush(Color.FromArgb(238, 238, 242));
		private static readonly Brush BrushBackgroundFocus = new SolidBrush(Color.FromArgb(0, 122, 204));
		private static readonly Brush BrushForeground = new SolidBrush(Color.Black);
		private static readonly Brush BrushForegroundForcus = new SolidBrush(Color.White);

		//		private ToolTip _toolTip;
		private Rectangle _bounds;
		private Rectangle _textBounds;
		private Rectangle _handleBounds;
		private readonly List<ToolWindowCaptionButton> _buttons;

		#endregion


		#region Constructor

		public ToolWindowCaption(ToolWindowOld parent)
		{
			Parent = parent;
			_buttons = new List<ToolWindowCaptionButton>();
			_bounds = new Rectangle(0, 0, 0, 0);
			_textBounds = new Rectangle(0, 0, 0, 0);
			_handleBounds = new Rectangle(0, 0, 0, 0);
			//			_toolTip = new ToolTip { AutoPopDelay = 5000, InitialDelay = 1000, ReshowDelay = 500, ShowAlways = true };
		}

		#endregion


		#region Properties

		public ToolWindowOld Parent { get; set; }

		/// <summary>
		///	Handle image must have 2 bitmap, first bitmap
		/// represent normal state, second represent
		/// Parent have Focus.
		/// </summary>
		public Image[] HandleImages { get; set; }

		public Rectangle Bounds { get { return _bounds; } }

		#endregion


		#region Methods

		public void AddButton(ToolWindowCaptionButton button)
		{
			_buttons.Add(button);
		}

		public virtual void OnMouseDown(MouseEventArgs e)
		{
			if (!_bounds.Contains(e.Location)) return;

			var button = _buttons.FirstOrDefault(b => b.Bounds.Contains(e.Location));
			if (button == null) return;

			button.Checked = true;
		}

		public virtual void OnMouseUp(MouseEventArgs e)
		{
			if (!_bounds.Contains(e.Location)) return;

			var button = _buttons.FirstOrDefault(b => b.Bounds.Contains(e.Location));
			if (button == null || !button.Checked) return;

			button.Checked = false;

			Parent.Invalidate(button.Bounds);
		}

		public virtual void OnMouseMove(MouseEventArgs e)
		{
			if (!_bounds.Contains(e.Location)) return;

			var button = _buttons.FirstOrDefault(b => b.Bounds.Contains(e.Location));
			if (button == null)
			{
				button = _buttons.FirstOrDefault(b => b.Highlight);
				if (button == null) return;

				button.Highlight = false;
			}
			else
			{
				button.Highlight = true;
				var buttons = _buttons.Where(b => b.Highlight);
				foreach (var b in buttons.Where(b => b != button))
					b.Highlight = false;
			}
		}

		public virtual void OnTextChanged(EventArgs e)
		{
			using (var g = Parent.CreateGraphics())
				CalculateBounds(g);

			Parent.Invalidate(_bounds);
		}

		public virtual void OnResize(EventArgs e)
		{
			CalculateBounds();

			CalculateBounds();
			Parent.Invalidate(_bounds);
		}

		public virtual void OnFocusChanged()
		{
			Parent.Invalidate(_bounds);
		}

		public virtual void OnPaint(PaintEventArgs e, Graphics g)
		{
			if (!e.ClipRectangle.IntersectsWith(_bounds)) return;

			var focus = Parent.IsFocused;

			//	Draw background
			var bgBrush = focus ? BrushBackgroundFocus : BrushBackground;
			g.FillRectangle(bgBrush, _bounds);

			//	Draw Text
			var sf = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};
			var fgBrush = focus ? BrushForegroundForcus : BrushForeground;
			g.DrawString(Parent.Text, CaptionFont, fgBrush, _textBounds, sf);

			//	Draw Handle
			var rc = new Rectangle(_handleBounds.Location, HandleImages[0].Size);
			var img = HandleImages[focus ? 1 : 0];
			while (rc.Right <= _handleBounds.Right)
			{
				g.DrawImage(img, rc);
				rc.X += rc.Width;
			}
			g.DrawImage(img, new Rectangle(rc.X, rc.Y, 1, rc.Height));

			//	Draw buttons
			for (var i = 0; i < _buttons.Count; i++)
			{
				g.DrawImage(_buttons[i].GetImage(), _buttons[i].Bounds);
			}
		}

		private void CalculateBounds(Graphics graphics = null)
		{
			var rc = Parent.ClientRectangle;
			_bounds.X = rc.X;
			_bounds.Y = rc.Y;
			_bounds.Width = rc.Width;
			_bounds.Height = CaptionHeight;

			if (graphics != null)
			{
				var textSize = graphics.MeasureString(Parent.Text, CaptionFont);
				_textBounds.X = rc.X;
				_textBounds.Y = rc.Y;
				_textBounds.Width = (int)Math.Ceiling(textSize.Width) + Margin * 2;
				_textBounds.Height = CaptionHeight;
			}

			const int rightEndMargin = 4;
			int h;
			int vCenter;
			for (var i = _buttons.Count - 1; i >= 0; i--)
			{
				var bmp = _buttons[i].GetImage();
				var w = bmp.Width;
				h = bmp.Height;
				vCenter = (CaptionHeight - h) / 2 + rc.Y;

				var x = i == _buttons.Count - 1 ? _bounds.Right - w - rightEndMargin : _buttons[i + 1].Bounds.X - 1 - w;
				var y = vCenter;
				_buttons[i].SetBounds(x, y, w, h);
			}

			h = HandleImages[0].Height;
			vCenter = (CaptionHeight - h) / 2 + rc.Y;
			_handleBounds.X = _textBounds.Right + Margin;
			_handleBounds.Y = vCenter;
			_handleBounds.Width = _buttons[0].Bounds.X - rightEndMargin - _handleBounds.X;
			_handleBounds.Height = h;
		}

		#endregion
	}
}
