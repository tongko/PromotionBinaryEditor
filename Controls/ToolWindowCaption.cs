using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BinEdit.Controls
{
	public class ToolWindowCaption
	{
		private const int Margin = 5;
		private const int CaptionHeight = 21;

		private static readonly Font CaptionFont = new Font("Segoe UI", 9f, FontStyle.Regular);
		private static readonly Brush BrushBackground = new SolidBrush(Color.FromArgb(238, 238, 242));
		private static readonly Brush BrushBackgroundFocus = new SolidBrush(Color.FromArgb(0, 122, 204));
		private static readonly Brush BrushForeground = new SolidBrush(Color.Black);
		private static readonly Brush BrushForegroundForcus = new SolidBrush(Color.White);

		private Rectangle _capBounds;
		private Rectangle _bounds;
		private Rectangle _textBounds;
		private Rectangle _handleBounds;
		private readonly List<ToolWindowCaptionButton> _buttons;

		private ToolWindowCaptionButton _prevButton;

		public ToolWindowCaption(ToolWindow parent)
		{
			Parent = parent;
			_buttons = new List<ToolWindowCaptionButton>();
			_bounds = new Rectangle(0, 0, 0, 0);
			_textBounds = new Rectangle(0, 0, 0, 0);
			_handleBounds = new Rectangle(0, 0, 0, 0);
		}

		public ToolWindow Parent { get; set; }

		/// <summary>
		///	Handle image must have 2 bitmap, first bitmap
		/// represent normal state, second represent
		/// Parent have Focus.
		/// </summary>
		public Image[] HandleImages { get; set; }

		public Rectangle Bounds { get { return _bounds; } }

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
			Parent.Invalidate(button.Bounds);
		}

		public virtual void OnMouseUp(MouseEventArgs e)
		{
			if (!_bounds.Contains(e.Location)) return;

			var button = _buttons.FirstOrDefault(b => b.Bounds.Contains(e.Location));
			if (button == null) return;

			button.Checked = button.Highlight = false;

			Parent.Invalidate(button.Bounds);
		}

		public virtual void OnMouseMove(MouseEventArgs e)
		{
			if (!_bounds.Contains(e.Location)) return;

			var button = _buttons.FirstOrDefault(b => b.Bounds.Contains(e.Location));
			if (button == null)
			{
				button = _buttons.FirstOrDefault(b => b.Checked || b.Highlight);
				if (button == null) return;

				button.Checked = button.Highlight = false;
				//_prevButton = null;
			}
			else
			{
				//if (button == _prevButton) return;

				button.Highlight = true;
				//_prevButton = button;
			}

			Parent.Invalidate(button.Bounds);
		}

		public virtual void OnTextChanged(EventArgs e)
		{
			using (var g = Parent.CreateGraphics())
				CalculateBounds(g);

			Parent.Invalidate(_capBounds);
		}

		public virtual void OnResize(EventArgs e)
		{
			CalculateBounds();

			_capBounds = new Rectangle(0, 0, Parent.Width, CaptionHeight);
			Parent.Invalidate(_capBounds);
		}

		public virtual void OnLostFocus()
		{
			Parent.Invalidate(_capBounds);
		}

		public virtual void OnPaint(PaintEventArgs e, Graphics g)
		{
			if (!e.ClipRectangle.IntersectsWith(_bounds)) return;

			//var g = e.Graphics;
			var focus = Parent.Focus();

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
	}
}
