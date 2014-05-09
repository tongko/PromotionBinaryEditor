namespace TestUserControl
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.toolWindow1 = new BinEdit.Controls.ToolWindow();
			this.SuspendLayout();
			// 
			// toolWindow1
			// 
			this.toolWindow1.BackColor = System.Drawing.Color.White;
			this.toolWindow1.Dock = System.Windows.Forms.DockStyle.Left;
			this.toolWindow1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.toolWindow1.Location = new System.Drawing.Point(0, 0);
			this.toolWindow1.Name = "toolWindow1";
			this.toolWindow1.Size = new System.Drawing.Size(301, 657);
			this.toolWindow1.TabIndex = 2;
			this.toolWindow1.Text = "Properties";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(808, 657);
			this.Controls.Add(this.toolWindow1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private BinEdit.Controls.ToolWindow toolWindow1;

	}
}

