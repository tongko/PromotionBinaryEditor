using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;

namespace BinEdit.Controls.Design
{
	public class CaptionGlyph : Glyph
	{
		private readonly ToolWindowOld _control;
		private readonly BehaviorService _behaviorSvc;

		public CaptionGlyph(BehaviorService behaviorSvc, Control control)
			: base(new PreventDropBehavior())
		{
			_control = control as ToolWindowOld;
			_behaviorSvc = behaviorSvc;
			if (control == null)
				throw new ArgumentException("control");
		}

		/// <summary>
		/// Gets the bounds of the <see cref="T:System.Windows.Forms.Design.Behavior.Glyph"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Drawing.Rectangle"/> representing the bounds of the <see cref="T:System.Windows.Forms.Design.Behavior.Glyph"/>.
		/// </returns>
		public override Rectangle Bounds
		{
			get
			{
				var rcCaption = _control.GetCaptionBounds();
				var rcGlyph = new Rectangle(0, 0, rcCaption.Width + rcCaption.X, rcCaption.Height + rcCaption.Y);
				var pt = _behaviorSvc.ControlToAdornerWindow(_control);
				rcCaption.Offset(pt);

				return rcCaption;
			}
		}

		public override Cursor GetHitTest(Point p)
		{
			if (Bounds.Contains(p))
				return Cursors.Arrow;

			return null;
		}

		public override void Paint(PaintEventArgs pe)
		{
			var b = new SolidBrush(Color.FromArgb(5, 200, 200, 200));
			pe.Graphics.FillRectangle(b, Bounds);
		}

		class PreventDropBehavior : Behavior
		{
			/// <summary>
			/// Permits custom drag-and-drop behavior.
			/// </summary>
			/// <param name="g">A <see cref="T:System.Windows.Forms.Design.Behavior.Glyph"/> object on which to invoke drag-and-drop behavior.</param><param name="e">A <see cref="T:System.Windows.Forms.DragEventArgs"/> that contains the event data. </param>
			public override void OnDragDrop(Glyph g, DragEventArgs e)
			{
				e.Effect = DragDropEffects.None;
			}

			/// <summary>
			/// Permits custom drag-enter behavior.
			/// </summary>
			/// <param name="g">A <see cref="T:System.Windows.Forms.Design.Behavior.Glyph"/> on which to invoke drag-enter behavior.</param><param name="e">A <see cref="T:System.Windows.Forms.DragEventArgs"/> that contains the event data. </param>
			public override void OnDragEnter(Glyph g, DragEventArgs e)
			{
				e.Effect = DragDropEffects.None;
			}

			/// <summary>
			/// Permits custom drag-over behavior.
			/// </summary>
			/// <param name="g">A <see cref="T:System.Windows.Forms.Design.Behavior.Glyph"/> on which to invoke drag-over behavior.</param><param name="e">A <see cref="T:System.Windows.Forms.DragEventArgs"/> that contains the event data. </param>
			public override void OnDragOver(Glyph g, DragEventArgs e)
			{
				e.Effect = DragDropEffects.None;
			}
		}
	}
}
