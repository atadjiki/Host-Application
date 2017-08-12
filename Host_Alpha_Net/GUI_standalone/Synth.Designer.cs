namespace GUI_standalone
{
    partial class Synth
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
            this.octaveList = new System.Windows.Forms.ComboBox();
            this.c1Button = new System.Windows.Forms.Button();
            this.d1Button = new System.Windows.Forms.Button();
            this.e1Button = new System.Windows.Forms.Button();
            this.f1Button = new System.Windows.Forms.Button();
            this.g1Button = new System.Windows.Forms.Button();
            this.a1Button = new System.Windows.Forms.Button();
            this.b1Button = new System.Windows.Forms.Button();
            this.cSharpButton = new System.Windows.Forms.Button();
            this.button16 = new System.Windows.Forms.Button();
            this.button17 = new System.Windows.Forms.Button();
            this.button18 = new System.Windows.Forms.Button();
            this.button19 = new System.Windows.Forms.Button();
            this.bindingsPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.bindingsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // octaveList
            // 
            this.octaveList.FormattingEnabled = true;
            this.octaveList.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8"});
            this.octaveList.Location = new System.Drawing.Point(12, 347);
            this.octaveList.Name = "octaveList";
            this.octaveList.Size = new System.Drawing.Size(62, 21);
            this.octaveList.TabIndex = 1;
            this.octaveList.Text = "Octave";
            // 
            // c1Button
            // 
            this.c1Button.Location = new System.Drawing.Point(3, 3);
            this.c1Button.Name = "c1Button";
            this.c1Button.Size = new System.Drawing.Size(52, 311);
            this.c1Button.TabIndex = 0;
            this.c1Button.UseVisualStyleBackColor = true;
            this.c1Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.c1Button_MouseDown);
            this.c1Button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.c1Button_MouseUp);
            // 
            // d1Button
            // 
            this.d1Button.Location = new System.Drawing.Point(54, 3);
            this.d1Button.Name = "d1Button";
            this.d1Button.Size = new System.Drawing.Size(54, 311);
            this.d1Button.TabIndex = 2;
            this.d1Button.UseVisualStyleBackColor = true;
            this.d1Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.d1Button_MouseDown);
            this.d1Button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.d1Button_MouseUp);
            // 
            // e1Button
            // 
            this.e1Button.Location = new System.Drawing.Point(106, 3);
            this.e1Button.Name = "e1Button";
            this.e1Button.Size = new System.Drawing.Size(54, 311);
            this.e1Button.TabIndex = 3;
            this.e1Button.UseVisualStyleBackColor = true;
            this.e1Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.e1Button_MouseDown);
            this.e1Button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.e1Button_MouseUp);
            // 
            // f1Button
            // 
            this.f1Button.Location = new System.Drawing.Point(157, 3);
            this.f1Button.Name = "f1Button";
            this.f1Button.Size = new System.Drawing.Size(57, 311);
            this.f1Button.TabIndex = 4;
            this.f1Button.UseVisualStyleBackColor = true;
            this.f1Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.f1Button_MouseDown);
            this.f1Button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.f1Button_MouseUp);
            // 
            // g1Button
            // 
            this.g1Button.Location = new System.Drawing.Point(211, 3);
            this.g1Button.Name = "g1Button";
            this.g1Button.Size = new System.Drawing.Size(54, 311);
            this.g1Button.TabIndex = 5;
            this.g1Button.UseVisualStyleBackColor = true;
            this.g1Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.g1Button_MouseDown);
            this.g1Button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.g1Button_MouseUp);
            // 
            // a1Button
            // 
            this.a1Button.Location = new System.Drawing.Point(262, 3);
            this.a1Button.Name = "a1Button";
            this.a1Button.Size = new System.Drawing.Size(57, 311);
            this.a1Button.TabIndex = 6;
            this.a1Button.UseVisualStyleBackColor = true;
            this.a1Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.a1Button_MouseDown);
            this.a1Button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.a1Button_MouseUp);
            // 
            // b1Button
            // 
            this.b1Button.Location = new System.Drawing.Point(316, 3);
            this.b1Button.Name = "b1Button";
            this.b1Button.Size = new System.Drawing.Size(54, 311);
            this.b1Button.TabIndex = 7;
            this.b1Button.UseVisualStyleBackColor = true;
            this.b1Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.b1Button_MouseDown);
            this.b1Button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.b1Button_MouseUp);
            // 
            // cSharpButton
            // 
            this.cSharpButton.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.cSharpButton.Location = new System.Drawing.Point(32, 3);
            this.cSharpButton.Name = "cSharpButton";
            this.cSharpButton.Size = new System.Drawing.Size(34, 260);
            this.cSharpButton.TabIndex = 16;
            this.cSharpButton.Text = "A";
            this.cSharpButton.UseVisualStyleBackColor = false;
            // 
            // button16
            // 
            this.button16.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button16.Location = new System.Drawing.Point(92, 3);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(34, 260);
            this.button16.TabIndex = 17;
            this.button16.Text = "A";
            this.button16.UseVisualStyleBackColor = false;
            // 
            // button17
            // 
            this.button17.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button17.Location = new System.Drawing.Point(180, 3);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(34, 260);
            this.button17.TabIndex = 18;
            this.button17.Text = "A";
            this.button17.UseVisualStyleBackColor = false;
            // 
            // button18
            // 
            this.button18.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button18.Location = new System.Drawing.Point(243, 3);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(34, 260);
            this.button18.TabIndex = 19;
            this.button18.Text = "A";
            this.button18.UseVisualStyleBackColor = false;
            // 
            // button19
            // 
            this.button19.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button19.Location = new System.Drawing.Point(302, 3);
            this.button19.Name = "button19";
            this.button19.Size = new System.Drawing.Size(34, 260);
            this.button19.TabIndex = 20;
            this.button19.Text = "A";
            this.button19.UseVisualStyleBackColor = false;
            // 
            // bindingsPanel
            // 
            this.bindingsPanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bindingsPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.bindingsPanel.Controls.Add(this.button19);
            this.bindingsPanel.Controls.Add(this.button18);
            this.bindingsPanel.Controls.Add(this.button17);
            this.bindingsPanel.Controls.Add(this.button16);
            this.bindingsPanel.Controls.Add(this.cSharpButton);
            this.bindingsPanel.Controls.Add(this.b1Button);
            this.bindingsPanel.Controls.Add(this.a1Button);
            this.bindingsPanel.Controls.Add(this.g1Button);
            this.bindingsPanel.Controls.Add(this.f1Button);
            this.bindingsPanel.Controls.Add(this.e1Button);
            this.bindingsPanel.Controls.Add(this.d1Button);
            this.bindingsPanel.Controls.Add(this.c1Button);
            this.bindingsPanel.Location = new System.Drawing.Point(-1, 1);
            this.bindingsPanel.Name = "bindingsPanel";
            this.bindingsPanel.Size = new System.Drawing.Size(897, 314);
            this.bindingsPanel.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 331);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "First Half ";
            // 
            // Synth
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.ClientSize = new System.Drawing.Size(367, 516);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.octaveList);
            this.Controls.Add(this.bindingsPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Synth";
            this.Text = "Synth";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Synth_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Synth_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Synth_KeyUp);
            this.bindingsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox octaveList;
        private System.Windows.Forms.Button c1Button;
        private System.Windows.Forms.Button d1Button;
        private System.Windows.Forms.Button e1Button;
        private System.Windows.Forms.Button f1Button;
        private System.Windows.Forms.Button g1Button;
        private System.Windows.Forms.Button a1Button;
        private System.Windows.Forms.Button b1Button;
        private System.Windows.Forms.Button cSharpButton;
        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.Button button17;
        private System.Windows.Forms.Button button18;
        private System.Windows.Forms.Button button19;
        private System.Windows.Forms.Panel bindingsPanel;
        private System.Windows.Forms.Label label1;

    }
}