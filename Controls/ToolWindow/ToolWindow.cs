using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BinEdit.Controls
{
	[ComVisible(true)]
	[DefaultEvent("Paint")]
	[DefaultProperty("Text")]
	[Docking(DockingBehavior.Ask)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ToolboxBitmap(typeof(ToolWindow), "Resources.ToolWindow.bmp")]
	public partial class ToolWindow : Control
	{
		public ToolWindow()
		{
			InitializeComponent();
		}
	}
}
