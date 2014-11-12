namespace TUI {
    partial class CodeWindow {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.turtleTrailBox = new System.Windows.Forms.PictureBox();
            this.Source = new System.Windows.Forms.TextBox();
            this.Go = new System.Windows.Forms.Button();
            this.ClearButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.turtleTrailBox)).BeginInit();
            this.SuspendLayout();
            // 
            // turtleTrailBox
            // 
            this.turtleTrailBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.turtleTrailBox.Location = new System.Drawing.Point(10, 12);
            this.turtleTrailBox.Name = "turtleTrailBox";
            this.turtleTrailBox.Size = new System.Drawing.Size(1311, 927);
            this.turtleTrailBox.TabIndex = 2;
            this.turtleTrailBox.TabStop = false;
            this.turtleTrailBox.Paint += new System.Windows.Forms.PaintEventHandler(this.turtleTrailBox_Paint);
            // 
            // Source
            // 
            this.Source.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Source.Location = new System.Drawing.Point(10, 945);
            this.Source.Multiline = true;
            this.Source.Name = "Source";
            this.Source.Size = new System.Drawing.Size(1187, 240);
            this.Source.TabIndex = 0;
            this.Source.Text = "Ctrl-{1..3} to load examples";
            // 
            // Go
            // 
            this.Go.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Go.Location = new System.Drawing.Point(1203, 1076);
            this.Go.Name = "Go";
            this.Go.Size = new System.Drawing.Size(118, 109);
            this.Go.TabIndex = 1;
            this.Go.Text = "&Go";
            this.Go.UseVisualStyleBackColor = true;
            this.Go.Click += new System.EventHandler(this.Go_Click);
            // 
            // ClearButton
            // 
            this.ClearButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClearButton.Location = new System.Drawing.Point(1203, 945);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(118, 125);
            this.ClearButton.TabIndex = 4;
            this.ClearButton.Text = "&Clear";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // CodeWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1332, 1197);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.turtleTrailBox);
            this.Controls.Add(this.Go);
            this.Controls.Add(this.Source);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CodeWindow";
            this.Text = "Turtle UI";
            ((System.ComponentModel.ISupportInitialize)(this.turtleTrailBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox turtleTrailBox;
        private System.Windows.Forms.TextBox Source;
        private System.Windows.Forms.Button Go;
        private System.Windows.Forms.Button ClearButton;


    }
}

