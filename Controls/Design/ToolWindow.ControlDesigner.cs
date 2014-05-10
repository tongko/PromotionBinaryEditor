using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace BinEdit.Controls.Design
{
	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	public class ToolWindowDesigner : ScrollableControlDesigner
	{
		private bool _parentSelected;
		//private IDesignerHost _host = null;
		//private IToolboxService _toolbox = null;
		//private ToolboxItem _mouseDragItem = null;
		private Adorner _adorner;

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

		protected ToolWindowOld DesigningControl
		{
			get { return (ToolWindowOld)Control; }
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
				if (_adorner != null)
				{
					var bs = BehaviorService;
					if (bs != null)
						bs.Adorners.Remove(_adorner);
				}
			}

			base.Dispose(disposing);
		}

		public override IList SnapLines
		{
			get
			{
				var margin = DesigningControl.Margin;
				var arrayList = new ArrayList(4);
				var width = DesigningControl.Width;
				var height = DesigningControl.Height;
				var capBounds = DesigningControl.GetCaptionBounds();

				arrayList.Add(new SnapLine(SnapLineType.Top, capBounds.Bottom, SnapLinePriority.Always));
				arrayList.Add(new SnapLine(SnapLineType.Bottom, height - margin.Bottom - 1, SnapLinePriority.Always));
				arrayList.Add(new SnapLine(SnapLineType.Left, margin.Left + 1, SnapLinePriority.Always));
				arrayList.Add(new SnapLine(SnapLineType.Right, width - margin.Right - 1, SnapLinePriority.Always));
				//arrayList.Add(new SnapLine(SnapLineType.Horizontal, -margin.Top, "Margin.Top", SnapLinePriority.Always));
				arrayList.Add(new SnapLine(SnapLineType.Horizontal, margin.Bottom + height, "Margin.Bottom", SnapLinePriority.Always));
				arrayList.Add(new SnapLine(SnapLineType.Vertical, -margin.Left, "Margin.Left", SnapLinePriority.Always));
				arrayList.Add(new SnapLine(SnapLineType.Vertical, margin.Right + width, "Margin.Right", SnapLinePriority.Always));

				return arrayList;

			}
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
			var pc = (ToolWindowOld)Control;

			if (!_parentSelected) return false;

			var hitTest = Control.PointToClient(point);
			return !pc.DisplayRectangle.Contains(hitTest);
		}

		///// <summary>
		///// Called in order to clean up a drag-and-drop operation.
		///// </summary>
		///// <param name="de">A <see cref="T:System.Windows.Forms.DragEventArgs"/> that provides data for the event.</param>
		//protected override void OnDragComplete(DragEventArgs de)
		//{
		//	base.OnDragComplete(de);

		//	_mouseDragItem = null;
		//}

		//protected override void OnDragDrop(DragEventArgs de)
		//{
		//	if (_host == null)
		//		_host = (IDesignerHost)GetService(typeof(IDesignerHost));
		//	if (_host != null)
		//	{
		//		if (_toolbox == null)
		//			_toolbox = (IToolboxService)GetService(typeof(IToolboxService));
		//		if (_toolbox != null)
		//		{
		//			_mouseDragItem = _toolbox.DeserializeToolboxItem(de.Data, _host);
		//		}
		//	}

		//	base.OnDragDrop(de);
		//}

		///// <summary>
		///// Called when a drag-and-drop operation enters the control designer view.
		///// </summary>
		///// <param name="de">A <see cref="T:System.Windows.Forms.DragEventArgs"/> that provides data for the event. </param>
		//protected override void OnDragEnter(DragEventArgs de)
		//{
		//	base.OnDragEnter(de);

		//	if (de.Effect == DragDropEffects.None || DesigningControl == null) return;

		//	var rc = DesigningControl.GetCaptionBounds();
		//	var pt = DesigningControl.PointToClient(new Point(de.X, de.Y));
		//	if (rc.Contains(pt))
		//		de.Effect = DragDropEffects.None;
		//}

		///// <summary>
		///// Called when a drag-and-drop object is dragged over the control designer view.
		///// </summary>
		///// <param name="de">A <see cref="T:System.Windows.Forms.DragEventArgs"/> that provides data for the event. </param>
		//protected override void OnDragOver(DragEventArgs de)
		//{
		//	base.OnDragOver(de);

		//	if (de.Effect == DragDropEffects.None || DesigningControl == null) return;

		//	var rc = DesigningControl.GetCaptionBounds();
		//	var pt = DesigningControl.PointToClient(new Point(de.X, de.Y));
		//	if (rc.Contains(pt))
		//		de.Effect = DragDropEffects.None;
		//}

		/// <summary>
		/// Initializes the designer with the specified component.
		/// </summary>
		/// <param name="component">The <see cref="T:System.ComponentModel.IComponent"/> to associate with the designer. </param>
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);

			_adorner = new Adorner();
			var bs = BehaviorService;
			if (bs != null)
				bs.Adorners.Add(_adorner);
			_adorner.Glyphs.Add(new CaptionGlyph(bs, Control));

			var selectionService = (ISelectionService)GetService(typeof(ISelectionService));
			if (selectionService != null)
				selectionService.SelectionChanged += OnSelectionChanged;
			//_host = (IDesignerHost)GetService(typeof(IDesignerHost));
			//_toolbox = (IToolboxService)GetService(typeof(IToolboxService));
		}

		private void OnSelectionChanged(object sender, System.EventArgs e)
		{
			_parentSelected = false;//this is for HitTest purposes

			var svc = (ISelectionService)GetService(typeof(ISelectionService));
			if (svc == null) return;

			var comp = (ToolWindowOld)Component;
			var selected = svc.GetSelectedComponents();
			foreach (var sel in selected)
			{
				if (sel == comp)
				{
					_parentSelected = true;
					DesigningControl.SetFocus(true);
				}
				else
				{
					_parentSelected = false;
					DesigningControl.SetFocus(false);
				}

			}
		}

		protected override void PreFilterProperties(IDictionary properties)
		{
			base.PostFilterProperties(properties);

			properties.Remove("BorderStyle");
		}
	}
}
