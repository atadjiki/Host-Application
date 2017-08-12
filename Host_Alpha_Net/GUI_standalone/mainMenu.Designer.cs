namespace GUI_standalone
{
    partial class mainMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainMenu));
            this.synthLaunchButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // synthLaunchButton
            // 
            this.synthLaunchButton.Location = new System.Drawing.Point(106, 100);
            this.synthLaunchButton.Name = "synthLaunchButton";
            this.synthLaunchButton.Size = new System.Drawing.Size(61, 51);
            this.synthLaunchButton.TabIndex = 0;
            this.synthLaunchButton.Text = "Launch Synth";
            this.synthLaunchButton.UseVisualStyleBackColor = true;
            this.synthLaunchButton.Click += new System.EventHandler(this.synthLaunchButton_Click);
            // 
            // mainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.synthLaunchButton);
            this.Name = "mainMenu";
            this.Text = "Standalone Host";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button synthLaunchButton;
    }
}