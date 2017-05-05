namespace GUI_Alpha
{
    partial class ControlPanel
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
            this.components = new System.ComponentModel.Container();
            this.masterVolume = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.masterPan = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.devicePan = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.deviceVolume = new System.Windows.Forms.TrackBar();
            this.deviceList = new System.Windows.Forms.ComboBox();
            this.slotBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.masterVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.masterPan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.devicePan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.slotBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // masterVolume
            // 
            this.masterVolume.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.masterVolume.Cursor = System.Windows.Forms.Cursors.Hand;
            this.masterVolume.Location = new System.Drawing.Point(4, 58);
            this.masterVolume.Maximum = 100;
            this.masterVolume.Name = "masterVolume";
            this.masterVolume.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.masterVolume.Size = new System.Drawing.Size(45, 104);
            this.masterVolume.TabIndex = 0;
            this.masterVolume.Value = 100;
            this.masterVolume.ValueChanged += new System.EventHandler(this.masterVolume_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Volume";
            // 
            // masterPan
            // 
            this.masterPan.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.masterPan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.masterPan.Location = new System.Drawing.Point(55, 58);
            this.masterPan.Maximum = 50;
            this.masterPan.Minimum = -50;
            this.masterPan.Name = "masterPan";
            this.masterPan.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.masterPan.Size = new System.Drawing.Size(45, 104);
            this.masterPan.TabIndex = 2;
            this.masterPan.ValueChanged += new System.EventHandler(this.masterPan_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(61, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Pan";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Master Controls";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(219, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Pan";
            // 
            // devicePan
            // 
            this.devicePan.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.devicePan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.devicePan.Location = new System.Drawing.Point(213, 58);
            this.devicePan.Maximum = 50;
            this.devicePan.Minimum = -50;
            this.devicePan.Name = "devicePan";
            this.devicePan.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.devicePan.Size = new System.Drawing.Size(45, 104);
            this.devicePan.TabIndex = 8;
            this.devicePan.ValueChanged += new System.EventHandler(this.devicePan_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(162, 42);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Volume";
            // 
            // deviceVolume
            // 
            this.deviceVolume.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.deviceVolume.Cursor = System.Windows.Forms.Cursors.Hand;
            this.deviceVolume.Location = new System.Drawing.Point(162, 58);
            this.deviceVolume.Maximum = 100;
            this.deviceVolume.Name = "deviceVolume";
            this.deviceVolume.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.deviceVolume.Size = new System.Drawing.Size(45, 104);
            this.deviceVolume.TabIndex = 6;
            this.deviceVolume.Value = 100;
            this.deviceVolume.ValueChanged += new System.EventHandler(this.deviceVolume_ValueChanged);
            // 
            // deviceList
            // 
            this.deviceList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.deviceList.FormattingEnabled = true;
            this.deviceList.Location = new System.Drawing.Point(162, 9);
            this.deviceList.MaxDropDownItems = 12;
            this.deviceList.Name = "deviceList";
            this.deviceList.Size = new System.Drawing.Size(98, 21);
            this.deviceList.Sorted = true;
            this.deviceList.TabIndex = 10;
            this.deviceList.SelectedIndexChanged += new System.EventHandler(this.deviceList_SelectedIndexChanged);
            this.deviceList.Click += new System.EventHandler(this.deviceList_Click);
            // 
            // slotBindingSource
            // 
            this.slotBindingSource.DataSource = typeof(Host_Alpha.Slot);
            // 
            // ControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(284, 173);
            this.Controls.Add(this.deviceList);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.devicePan);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.deviceVolume);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.masterPan);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.masterVolume);
            this.Name = "ControlPanel";
            this.Text = "Audio Mobile Host";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlPanel_FormClosing);
            this.Load += new System.EventHandler(this.ControlPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.masterVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.masterPan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.devicePan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.slotBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar masterVolume;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar masterPan;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar devicePan;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TrackBar deviceVolume;
        private System.Windows.Forms.ComboBox deviceList;
        private System.Windows.Forms.BindingSource slotBindingSource;
    }
}

