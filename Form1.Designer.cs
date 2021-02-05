namespace ScoringEngineTeamGenerator
{
	partial class TeamGen
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
			this.btnTemplate = new System.Windows.Forms.Button();
			this.tc_TeamTabs = new System.Windows.Forms.TabControl();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.teamAmount = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.teamAmount)).BeginInit();
			this.SuspendLayout();
			// 
			// btnTemplate
			// 
			this.btnTemplate.Location = new System.Drawing.Point(13, 13);
			this.btnTemplate.Name = "btnTemplate";
			this.btnTemplate.Size = new System.Drawing.Size(118, 23);
			this.btnTemplate.TabIndex = 0;
			this.btnTemplate.Text = "Open Template";
			this.btnTemplate.UseVisualStyleBackColor = true;
			this.btnTemplate.Click += new System.EventHandler(this.btnTemplate_Click);
			// 
			// tc_TeamTabs
			// 
			this.tc_TeamTabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tc_TeamTabs.Location = new System.Drawing.Point(0, 0);
			this.tc_TeamTabs.Name = "tc_TeamTabs";
			this.tc_TeamTabs.SelectedIndex = 0;
			this.tc_TeamTabs.Size = new System.Drawing.Size(800, 450);
			this.tc_TeamTabs.TabIndex = 1;
			this.tc_TeamTabs.Visible = false;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// teamAmount
			// 
			this.teamAmount.Location = new System.Drawing.Point(137, 13);
			this.teamAmount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.teamAmount.Name = "teamAmount";
			this.teamAmount.Size = new System.Drawing.Size(41, 20);
			this.teamAmount.TabIndex = 2;
			this.teamAmount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.teamAmount.ValueChanged += new System.EventHandler(this.teamAmount_ValueChanged);
			// 
			// TeamGen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.teamAmount);
			this.Controls.Add(this.btnTemplate);
			this.Controls.Add(this.tc_TeamTabs);
			this.Name = "TeamGen";
			this.Text = "SE: Team Generator";
			((System.ComponentModel.ISupportInitialize)(this.teamAmount)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnTemplate;
		private System.Windows.Forms.TabControl tc_TeamTabs;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.NumericUpDown teamAmount;
	}
}

