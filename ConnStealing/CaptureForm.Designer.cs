namespace ConnStealing
{
    partial class CaptureForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CaptureForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb1StartStop = new System.Windows.Forms.ToolStripButton();
            this.stb2Clear = new System.Windows.Forms.ToolStripButton();
            this.tsb3Send = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.packetInfoTextbox = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.captureStatisticsToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.gbrSniffType = new System.Windows.Forms.GroupBox();
            this.rbOnlyRecvData = new System.Windows.Forms.RadioButton();
            this.rbEveryPacket = new System.Windows.Forms.RadioButton();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.gbrSniffType.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb1StartStop,
            this.stb2Clear,
            this.tsb3Send});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(915, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsb1StartStop
            // 
            this.tsb1StartStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb1StartStop.Image = global::ConnStealing.Properties.Resources.stop_icon_disabled;
            this.tsb1StartStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb1StartStop.Name = "tsb1StartStop";
            this.tsb1StartStop.Size = new System.Drawing.Size(23, 22);
            this.tsb1StartStop.Click += new System.EventHandler(this.tsb1StartStop_Click);
            // 
            // stb2Clear
            // 
            this.stb2Clear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.stb2Clear.Image = ((System.Drawing.Image)(resources.GetObject("stb2Clear.Image")));
            this.stb2Clear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stb2Clear.Name = "stb2Clear";
            this.stb2Clear.Size = new System.Drawing.Size(36, 22);
            this.stb2Clear.Text = "clear";
            this.stb2Clear.Click += new System.EventHandler(this.stb2Clear_Click);
            // 
            // tsb3Send
            // 
            this.tsb3Send.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsb3Send.Image = ((System.Drawing.Image)(resources.GetObject("tsb3Send.Image")));
            this.tsb3Send.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb3Send.Name = "tsb3Send";
            this.tsb3Send.Size = new System.Drawing.Size(75, 22);
            this.tsb3Send.Text = "Send Packet";
            this.tsb3Send.Click += new System.EventHandler(this.tsb3Send_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 28);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView);
            this.splitContainer1.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel1_Paint);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.packetInfoTextbox);
            this.splitContainer1.Size = new System.Drawing.Size(891, 467);
            this.splitContainer1.SplitterDistance = 224;
            this.splitContainer1.TabIndex = 3;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.Size = new System.Drawing.Size(891, 224);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            // 
            // packetInfoTextbox
            // 
            this.packetInfoTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.packetInfoTextbox.Location = new System.Drawing.Point(0, 0);
            this.packetInfoTextbox.Name = "packetInfoTextbox";
            this.packetInfoTextbox.Size = new System.Drawing.Size(891, 239);
            this.packetInfoTextbox.TabIndex = 1;
            this.packetInfoTextbox.Text = "";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.captureStatisticsToolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 498);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(915, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // captureStatisticsToolStripStatusLabel
            // 
            this.captureStatisticsToolStripStatusLabel.Name = "captureStatisticsToolStripStatusLabel";
            this.captureStatisticsToolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // gbrSniffType
            // 
            this.gbrSniffType.Controls.Add(this.rbOnlyRecvData);
            this.gbrSniffType.Controls.Add(this.rbEveryPacket);
            this.gbrSniffType.Location = new System.Drawing.Point(658, 2);
            this.gbrSniffType.Name = "gbrSniffType";
            this.gbrSniffType.Size = new System.Drawing.Size(245, 23);
            this.gbrSniffType.TabIndex = 5;
            this.gbrSniffType.TabStop = false;
            // 
            // rbOnlyRecvData
            // 
            this.rbOnlyRecvData.AutoSize = true;
            this.rbOnlyRecvData.Location = new System.Drawing.Point(98, 3);
            this.rbOnlyRecvData.Name = "rbOnlyRecvData";
            this.rbOnlyRecvData.Size = new System.Drawing.Size(143, 17);
            this.rbOnlyRecvData.TabIndex = 1;
            this.rbOnlyRecvData.Text = "Only Received with Data";
            this.rbOnlyRecvData.UseVisualStyleBackColor = true;
            // 
            // rbEveryPacket
            // 
            this.rbEveryPacket.AutoSize = true;
            this.rbEveryPacket.Checked = true;
            this.rbEveryPacket.Location = new System.Drawing.Point(3, 3);
            this.rbEveryPacket.Name = "rbEveryPacket";
            this.rbEveryPacket.Size = new System.Drawing.Size(89, 17);
            this.rbEveryPacket.TabIndex = 0;
            this.rbEveryPacket.TabStop = true;
            this.rbEveryPacket.Text = "Every Packet";
            this.rbEveryPacket.UseVisualStyleBackColor = true;
            this.rbEveryPacket.CheckedChanged += new System.EventHandler(this.rbEveryPacket_CheckedChanged);
            // 
            // CaptureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 520);
            this.Controls.Add(this.gbrSniffType);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "CaptureForm";
            this.Text = "ConnStealing";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CaptureForm_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.gbrSniffType.ResumeLayout(false);
            this.gbrSniffType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb1StartStop;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.RichTextBox packetInfoTextbox;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel captureStatisticsToolStripStatusLabel;
        private System.Windows.Forms.ToolStripButton stb2Clear;
        private System.Windows.Forms.GroupBox gbrSniffType;
        private System.Windows.Forms.RadioButton rbOnlyRecvData;
        private System.Windows.Forms.RadioButton rbEveryPacket;
        private System.Windows.Forms.ToolStripButton tsb3Send;
    }
}