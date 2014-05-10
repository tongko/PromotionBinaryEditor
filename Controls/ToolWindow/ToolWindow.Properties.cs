using System.ComponentModel;
using System.Drawing;

namespace BinEdit.Controls
{
	//	Properties
	partial class ToolWindow
	{
		#region Fields

		private Color _focusColor;
		private int _flags;

		#endregion


		#region Appearance

		[DefaultValue(typeof(Color), "238, 238, 242")]
		[Description("Color to set at caption when control got focus.")]
		[Category("Appearance")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Color FocusColor { get; set; }

		#endregion

		#region Window Style

		[Category("Window Style")]
		public bool ContextButton { get; set; }

		[Category("Window Style")]
		public bool PinButton { get; set; }

		[Category("Window Style")]
		public bool CloseButton { get; set; }

		#endregion
	}
}
