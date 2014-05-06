using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace BinEdit.Controls
{
	public class ToolWindowDesigner : ScrollableControlDesigner
	{
		private bool _parentSelected = false;

		protected Pen BorderPen
		{
			get
			{
				return new Pen((double)Control.BackColor.GetBrightness() < 0.5 ? ControlPaint.Light(Control.BackColor) : ControlPaint.Dark(Control.BackColor))
				{
					DashStyle = DashStyle.Dash
				};
			}
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Design.ParentControlDesigner"/>, and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources. </param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				var selectionService = (ISelectionService)GetService(typeof(ISelectionService));
				if (selectionService != null)
					selectionService.SelectionChanged -= OnSelectionChanged;
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Indicates whether a mouse click at the specified point should be handled by the control.
		/// </summary>
		/// <returns>
		/// true if a click at the specified point is to be handled by the control; otherwise, false.
		/// </returns>
		/// <param name="point">A <see cref="T:System.Drawing.Point"/> indicating the position at which the mouse was clicked, in screen coordinates. </param>
		protected override bool GetHitTest(Point point)
		{
			var pc = (ToolWindow)Control;

			if (!_parentSelected) return false;

			var hitTest = Control.PointToClient(point);
			return !pc.DisplayRectangle.Contains(hitTest);
		}

		/// <summary>
		/// Called when the control that the designer is managing has painted its surface so the designer can paint any additional adornments on top of the control.
		/// </summary>
		/// <param name="pe">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that provides data for the event. </param>
		//protected override void OnPaintAdornments(PaintEventArgs pe)
		//{
		//	DrawBorder(pe.Graphics);
		//	base.OnPaintAdornments(pe);
		//}

		protected virtual void DrawBorder(Graphics g)
		{
			var c = (ToolWindow)Component;
			if (c == null || !c.Visible)
				return;

			var borderPen = BorderPen;
			var clientRect = Control.DisplayRectangle;
			++clientRect.X;
			++clientRect.Y;
			clientRect.Width -= 5;
			clientRect.Height -= 5;
			g.DrawRectangle(borderPen, clientRect);
			borderPen.Dispose();
		}

		/// <summary>
		/// Initializes the designer with the specified component.
		/// </summary>
		/// <param name="component">The <see cref="T:System.ComponentModel.IComponent"/> to associate with the designer. </param>
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);

			var selectionService = (ISelectionService)GetService(typeof(ISelectionService));
			if (selectionService != null)
				selectionService.SelectionChanged += OnSelectionChanged;
		}

		private void OnSelectionChanged(object sender, System.EventArgs e)
		{
			_parentSelected = false;//this is for HitTest purposes

			var svc = (ISelectionService)GetService(typeof(ISelectionService));
			if (svc == null) return;

			var comp = (ToolWindow)Component;
			var selected = svc.GetSelectedComponents();
			foreach (var sel in selected)
			{
				if (sel == comp)
					_parentSelected = true;
			}
		}
	}
}
