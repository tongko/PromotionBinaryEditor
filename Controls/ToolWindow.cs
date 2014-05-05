using BinEdit.Controls.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BinEdit.Controls
{
	public partial class ToolWindow : UserControl
	{
		private static readonly Pen BorderPen = new Pen(Color.FromArgb(204, 206, 219));
		private const int WmNcPaint = 0x85;
		private const int WmNcHitTest = 0x0084;
		private const int HtError = -2;
		private const int HtTransparent = -1;
		private const int HtNowhere = 0;
		private const int HtClient = 1;
		private const int HtCaption = 2;
		private const int HtSysmenu = 3;
		private const int HtGrowBox = 4;
		private const int HtMenu = 5;
		private const int HtHScroll = 6;
		private const int HtVScroll = 7;
		private const int HtLeft = 10;
		private const int HtRight = 11;
		private const int HtTop = 12;
		private const int HtTopLeft = 13;
		private const int HtTopTight = 14;
		private const int HtBottom = 15;
		private const int HtBottomLeft = 16;
		private const int HtBottomRight = 17;
		private const int HtBorder = 18;
		private const int HtObject = 19;
		private const int HtClose = 20;
		private const int HtHelp = 21;

		private ToolWindowCaption _caption;

		#region Constructor

		public ToolWindow()
		{
			InitCaption();
			Text = "ToolWindow1";
			Dock = DockStyle.Left;
			InitializeComponent();
		}

		#endregion


		#region Override

		/// <returns>
		/// The text associated with this control.
		/// </returns>
		[Browsable(true), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
		public sealed override string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		/// <summary>
		/// Gets or sets which control borders are docked to its parent control and determines how a control is resized with its parent.
		/// </summary>
		/// <returns>
		/// One of the <see cref="T:System.Windows.Forms.DockStyle"/> values. The default is <see cref="F:System.Windows.Forms.DockStyle.None"/>.
		/// </returns>
		/// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the <see cref="T:System.Windows.Forms.DockStyle"/> values. </exception>
		[Browsable(true), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
		public sealed override DockStyle Dock
		{
			get { return base.Dock; }
			set { base.Dock = value; }
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				_caption.OnMouseDown(e);

			base.OnMouseDown(e);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data. </param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				_caption.OnMouseUp(e);

			base.OnMouseUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			_caption.OnMouseMove(e);

			base.OnMouseMove(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			//	Paint Caption
			_caption.OnPaint(e);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			_caption.OnResize(e);
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);

			_caption.OnTextChanged(e);
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WmNcPaint)
			{
				PaintNcBorder();
				return;
			}

			if (m.Msg == WmNcHitTest)
			{
				NcHitTest(ref m);
				return;
			}

			base.WndProc(ref m);
		}

		#endregion


		#region Methods

		public static int HIWORD(int n)
		{
			return n >> 16 & ushort.MaxValue;
		}

		public static int HIWORD(IntPtr n)
		{
			return HIWORD((int)(long)n);
		}

		private static int LOWORD(int n)
		{
			return n & ushort.MaxValue;
		}

		private static int LOWORD(IntPtr n)
		{
			return LOWORD((int)(long)n);
		}

		private static void NcHitTest(ref Message m)
		{
			var x = LOWORD(m.LParam);
			var y = HIWORD(m.LParam);
		}

		private void InitCaption()
		{
			_caption = new ToolWindowCaption(this)
			{
				HandleImages = new Image[] { Resources.ToolCaptionHandle, Resources.ToolCaptionHandleFocus }
			};

			var b = new ToolWindowCaptionButton(this, CaptionButtonType.CheckButton)
			{
				ImageList = new Image[]
				{
					Resources.ToolCaptionContext, Resources.ToolCaptionContextHover, Resources.ToolCaptionContextFocus,
					Resources.ToolCaptionContextFocusHover, Resources.ToolCaptionContextChecked
				}
			};
			_caption.AddButton(b);
			b = new ToolWindowCaptionButton(this, CaptionButtonType.Button)
			{
				ImageList = new Image[]
				{
					Resources.ToolCaptionPin, Resources.ToolCaptionPinHover, Resources.ToolCaptionPinFocus, Resources.ToolCaptionPinFocusHover, Resources.ToolCaptionPinChecked
				}
			};
			_caption.AddButton(b);
			b = new ToolWindowCaptionButton(this, CaptionButtonType.Button)
			{
				ImageList = new Image[]
				{
					Resources.ToolCaptionClose, Resources.ToolCaptionCloseHover, Resources.ToolCaptionCloseFocus, Resources.ToolCaptionCloseFocusHover,
					Resources.ToolCaptionCloseChecked
				}
			};
			_caption.AddButton(b);
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
