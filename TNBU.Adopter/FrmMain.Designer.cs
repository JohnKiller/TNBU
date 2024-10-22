namespace TNBU.Adopter {
	partial class FrmMain {
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			LwDevices = new ListView();
			ChMac = new ColumnHeader();
			ChIp = new ColumnHeader();
			ChModel = new ColumnHeader();
			ChVersion = new ColumnHeader();
			SuspendLayout();
			// 
			// LwDevices
			// 
			LwDevices.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			LwDevices.Columns.AddRange(new ColumnHeader[] { ChMac, ChIp, ChModel, ChVersion });
			LwDevices.FullRowSelect = true;
			LwDevices.GridLines = true;
			LwDevices.Location = new Point(12, 12);
			LwDevices.MultiSelect = false;
			LwDevices.Name = "LwDevices";
			LwDevices.Size = new Size(617, 255);
			LwDevices.TabIndex = 0;
			LwDevices.UseCompatibleStateImageBehavior = false;
			LwDevices.View = View.Details;
			LwDevices.DoubleClick += LwDevices_DoubleClick;
			// 
			// ChMac
			// 
			ChMac.Text = "MAC";
			ChMac.Width = 120;
			// 
			// ChIp
			// 
			ChIp.Text = "IP";
			ChIp.Width = 120;
			// 
			// ChModel
			// 
			ChModel.Text = "Model";
			ChModel.Width = 200;
			// 
			// ChVersion
			// 
			ChVersion.Text = "Version";
			ChVersion.Width = 100;
			// 
			// FrmMain
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(641, 279);
			Controls.Add(LwDevices);
			Name = "FrmMain";
			Text = "Adopter";
			ResumeLayout(false);
		}

		#endregion

		private ListView LwDevices;
		private ColumnHeader ChMac;
		private ColumnHeader ChIp;
		private ColumnHeader ChModel;
		private ColumnHeader ChVersion;
	}
}
