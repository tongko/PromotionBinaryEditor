using BinEdit.Controls.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BinEdit.Controls
{
	public partial class ToolWindow : UserControl
	{
		private const int CaptionHeight = 21;
		private const int LeftMargin = 5;
		private const int RightMargin = LeftMargin * 2;
		private static readonly Font CaptionFont = new Font("Segoe UI", 8, FontStyle.Regular, GraphicsUnit.Pixel);
		private static readonly Pen BorderPen = new Pen(Color.FromArgb(204, 206, 219));
		private static readonly Brush BrushBackground = new SolidBrush(Color.FromArgb(238, 238, 242));
		private static readonly Brush BrushBackgroundFocus = new SolidBrush(Color.FromArgb(0, 122, 204));
		private static readonly Brush BrushForeground = new SolidBrush(Color.Black);
		private static readonly Brush BrushForegroundForcus = new SolidBrush(Color.White);
		private const int WmNcPaint = 0x85;

		private const int HandleImg = 0;
		private const int HandleFocus = 1;
		private const int Context = 2;
		private const int ContextHover = 3;
		private const int ContextFocus = 4;
		private const int ContextFocusHover = 5;
		private const int Pin = 6;
		private const int PinHover = 7;
		private const int PinFocus = 8;
		private const int PinFocusHover = 9;
		private const int Close = 10;
		private const int CloseHover = 11;
		private const int CloseFocus = 12;
		private const int CloseFocusHover = 13;


		private static readonly List<Bitmap> CaptionImage =
			new List<Bitmap>(
				new[]
				{
					Resources.ToolCaptionHandle, Resources.ToolCaptionHandleFocus,
					Resources.ToolCaptionContext, Resources.ToolCaptionContextHover, Resources.ToolCaptionContextFocus,
					Resources.ToolCaptionContextFocusHover,
					Resources.ToolCaptionPin, Resources.ToolCaptionPinHover, Resources.ToolCaptionPinFocus,
					Resources.ToolCaptionPinFocusHover,
					Resources.ToolCaptionClose, Resources.ToolCaptionCloseHover, Resources.ToolCaptionCloseFocus,
					Resources.ToolCaptionCloseFocusHover
				});

		private readonly Rectangle[] _rcCaptions = new Rectangle[]
		{
			Rectangle.Empty,
			Rectangle.Empty,
			Rectangle.Empty,
			Rectangle.Empty,
			Rectangle.Empty,
			Rectangle.Empty
		};

		#region Constructor

		public ToolWindow()
		{
			InitCaption();
			InitializeComponent();
		}

		#endregion


		#region Override

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (_rcCaptions[0].Contains(e.Location))
			{
				if (_rcCaptions[5].Contains(e.Location))

			}

			base.OnMouseMove(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var g = e.Graphics;
			if (e.ClipRectangle.IntersectsWith(_rcCaptions[0]))
				PaintCaption(g);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			using (var g = CreateGraphics())
			{
				CalculateCaptionBound(g);
				Invalidate(_rcCaptions[0]);	//	Force redraw of caption
			}
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);

			using (var g = CreateGraphics())
			{
				CalculateCaptionBound(g);
				Invalidate(_rcCaptions[0]);
			}
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WmNcPaint)
			{
				PaintNcBorder();
				return;
			}

			base.WndProc(ref m);
		}

		#endregion


		#region Methods

		private void InitCaption()
		{

		}

		private void CalculateCaptionBound(Graphics graphics)
		{
			_rcCaptions[0].X = 0;
			_rcCaptions[0].Y = 0;
			_rcCaptions[0].Width = ClientRectangle.Width;
			_rcCaptions[0].Height = CaptionHeight;

			var textSize = graphics.MeasureString(Text, CaptionFont);
			_rcCaptions[1].X = 0;
			_rcCaptions[1].Y = 0;
			_rcCaptions[1].Width = (int)Math.Ceiling(textSize.Width) + LeftMargin + RightMargin;
			_rcCaptions[1].Height = CaptionHeight;

			const int rightEndMargin = 4;
			var w = CaptionImage[CloseFocusHover].Width;
			var h = CaptionImage[CloseFocusHover].Height;
			var vCenter = (CaptionHeight - h) / 2;
			_rcCaptions[5].X = _rcCaptions[0].Right - w - rightEndMargin;
			_rcCaptions[5].Y = vCenter;
			_rcCaptions[5].Width = w;
			_rcCaptions[5].Height = h;

			w = CaptionImage[PinFocusHover].Width;
			h = CaptionImage[PinFocusHover].Height;
			_rcCaptions[4].X = _rcCaptions[5].X - 1 - w;
			_rcCaptions[4].Y = vCenter;
			_rcCaptions[4].Width = w;
			_rcCaptions[4].Height = h;

			w = CaptionImage[ContextFocusHover].Width;
			h = CaptionImage[ContextFocusHover].Height;
			_rcCaptions[3].X = _rcCaptions[4].X - 1 - w;
			_rcCaptions[3].Y = vCenter;
			_rcCaptions[3].Width = w;
			_rcCaptions[3].Height = h;

			h = CaptionImage[HandleImg].Height;
			vCenter = (CaptionHeight - h) / 2;
			_rcCaptions[2].X = _rcCaptions[1].Right + 1;
			_rcCaptions[2].Y = vCenter;
			_rcCaptions[2].Width = _rcCaptions[3].X - rightEndMargin - _rcCaptions[2].X;
			_rcCaptions[2].Height = h;
		}

		private void PaintCaption(Graphics g)
		{
			//	Draw background
			var bgBrush = Focus() ? BrushBackgroundFocus : BrushBackground;
			g.FillRectangle(bgBrush, _rcCaptions[0]);

			//	Draw Text
			var sf = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};
			var fgBrush = Focus() ? BrushForegroundForcus : BrushForeground;
			g.DrawString(Text, CaptionFont, fgBrush, _rcCaptions[1], sf);

			//	Draw Handle
			var tb = new TextureBrush(CaptionImage[Focus() ? HandleFocus : HandleImg]);
			g.FillRectangle(tb, _rcCaptions[2]);
		}

		private void PaintNcBorder()
		{
			if (BorderStyle == BorderStyle.None) return;

			//	Obtain windows DC
			var hDc = GetDCEx(
				Handle,
				IntPtr.Zero,
				DeviceContextValues.Window | DeviceContextValues.Cache | DeviceContextValues.UseStyle);
			if (hDc == IntPtr.Zero)
				throw new Win32Exception(Marshal.GetLastWin32Error());
			using (var g = Graphics.FromHdc(hDc))
			{
				var rcBorder = new Rectangle(0, 0, Width - 1, Height - 1);
				g.DrawRectangle(BorderPen, rcBorder);
			}

			if (hDc != IntPtr.Zero)
				ReleaseDC(Handle, hDc);
		}

		#endregion


		#region Unsafe Code

		[DllImport("user32.dll")]
		static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, DeviceContextValues flags);

		[DllImport("user32.dll")]
		static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

		#endregion
	}
}
