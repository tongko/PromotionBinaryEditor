﻿namespace BinEdit.Controls
{
	partial class ToolWindowOld
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblEvent = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblEvent
			// 
			this.lblEvent.AutoSize = true;
			this.lblEvent.Location = new System.Drawing.Point(50, 199);
			this.lblEvent.Name = "lblEvent";
			this.lblEvent.Size = new System.Drawing.Size(38, 15);
			this.lblEvent.TabIndex = 0;
			this.lblEvent.Text = "label1";
			// 
			// ToolWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.lblEvent);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "ToolWindow";
			this.Size = new System.Drawing.Size(300, 600);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblEvent;
	}
}