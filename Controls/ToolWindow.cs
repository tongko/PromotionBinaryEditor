using BinEdit.Controls.Properties;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

namespace BinEdit.Controls
{
	[Designer(typeof(ToolWindowDesigner), typeof(IDesigner))]
	public partial class ToolWindow : UserControl
	{
		#region Fields

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
		private Bitmap _backBuffer;

		#endregion


		#region Constructor

		public ToolWindow()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint
				| ControlStyles.UserPaint
				| ControlStyles.DoubleBuffer, true);
			InitCaption();
			Dock = DockStyle.Left;
			Text = "ToolWindow";
			InitializeComponent();
		}

		#endregion


		#region Properties

		new private BorderStyle BorderStyle { get; set; }

		new public Rectangle ClientRectangle
		{
			get { return new Rectangle(4, 4, Width - 8, Height - 8); }
		}

		public bool IsFocused { get; private set; }

		#endregion


		#region Override

		/// <returns>
		/// The text associated with this control.
		/// </returns>
		[
		Category("Appearance"),
		Browsable(true),
		DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
		DefaultValue("ToolWindow")
		]
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

		protected override void OnControlAdded(ControlEventArgs e)
		{
			if (_caption.Bounds.Contains(e.Control.Location))
				e.Control.Location = new Point(e.Control.Location.X, _caption.Bounds.Height);

			e.Control.GotFocus += ControlOnGotFocus;
			e.Control.LostFocus += ControlOnLostFocus;
			base.OnControlAdded(e);
		}

		private void ControlOnLostFocus(object sender, EventArgs e)
		{
			IsFocused = false;
			_caption.OnFocusChanged();
		}

		private void ControlOnGotFocus(object sender, EventArgs eventArgs)
		{
			IsFocused = true;
			_caption.OnFocusChanged();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.LostFocus"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);

			_caption.OnFocusChanged();
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);

			_caption.OnFocusChanged();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			_caption.OnMouseMove(e);

			base.OnMouseMove(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (_backBuffer == null)
				_backBuffer = new Bitmap(Width, Height);

			var g = Graphics.FromImage(_backBuffer);

			//	draw border
			PaintNcBorder(g);

			//	Paint Caption
			_caption.OnPaint(e, g);

			g.Dispose();

			e.Graphics.DrawImageUnscaled(_backBuffer, 0, 0);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.SizeChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
		protected override void OnSizeChanged(EventArgs e)
		{
			if (_backBuffer != null)
			{
				_backBuffer.Dispose();
				_backBuffer = null;
			}

			_caption.OnResize(e);
			base.OnSizeChanged(e);
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);

			_caption.OnTextChanged(e);
		}

		protected override void WndProc(ref Message m)
		{
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

		private void NcHitTest(ref Message m)
		{
			var x = LOWORD(m.LParam);
			var y = HIWORD(m.LParam);

			var pt = PointToClient(new Point(x, y));

			//if (_caption.Bounds.Contains(pt))
			m.Result = new IntPtr(HtClient);
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

		private void PaintNcBorder(Graphics g)
		{
			g.Clear(BackColor);	//	Background

			const int margin = 3;
			var rc = ClientRectangle;
			var rcBorder = new Rectangle(rc.X - 1, rc.Y - 1, rc.Width + 1, rc.Height + 1);
			g.DrawRectangle(BorderPen, rcBorder);
		}

		#endregion
	}
}
