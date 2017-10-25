namespace CSharp3DEngine
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
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wireFrameModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.orthogonalModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noBackfaceCullingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.TimeForGameLabel = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1234, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newGameToolStripMenuItem,
            this.toolStripMenuItem2,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newGameToolStripMenuItem
            // 
            this.newGameToolStripMenuItem.Name = "newGameToolStripMenuItem";
            this.newGameToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.newGameToolStripMenuItem.Text = "New Game";
            this.newGameToolStripMenuItem.Click += new System.EventHandler(this.newGameToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(129, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wireFrameModeToolStripMenuItem,
            this.orthogonalModeToolStripMenuItem,
            this.noBackfaceCullingToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // wireFrameModeToolStripMenuItem
            // 
            this.wireFrameModeToolStripMenuItem.Name = "wireFrameModeToolStripMenuItem";
            this.wireFrameModeToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.wireFrameModeToolStripMenuItem.Text = "Wire-Frame Mode";
            this.wireFrameModeToolStripMenuItem.Click += new System.EventHandler(this.wireFrameModeToolStripMenuItem_Click);
            // 
            // orthogonalModeToolStripMenuItem
            // 
            this.orthogonalModeToolStripMenuItem.Name = "orthogonalModeToolStripMenuItem";
            this.orthogonalModeToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.orthogonalModeToolStripMenuItem.Text = "Orthogonal Mode";
            this.orthogonalModeToolStripMenuItem.Click += new System.EventHandler(this.orthogonalModeToolStripMenuItem_Click);
            // 
            // noBackfaceCullingToolStripMenuItem
            // 
            this.noBackfaceCullingToolStripMenuItem.Name = "noBackfaceCullingToolStripMenuItem";
            this.noBackfaceCullingToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.noBackfaceCullingToolStripMenuItem.Text = "No Backface Culling";
            this.noBackfaceCullingToolStripMenuItem.Click += new System.EventHandler(this.noBackfaceCullingToolStripMenuItem_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 20;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label1.Location = new System.Drawing.Point(839, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Time for the game";
            // 
            // TimeForGameLabel
            // 
            this.TimeForGameLabel.BackColor = System.Drawing.Color.White;
            this.TimeForGameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.TimeForGameLabel.Location = new System.Drawing.Point(839, 124);
            this.TimeForGameLabel.Name = "TimeForGameLabel";
            this.TimeForGameLabel.Size = new System.Drawing.Size(154, 20);
            this.TimeForGameLabel.TabIndex = 2;
            this.TimeForGameLabel.Text = "60 sec";
            this.TimeForGameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 722);
            this.Controls.Add(this.TimeForGameLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "3D View";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
      private System.Windows.Forms.Timer timer1;
    private System.Windows.Forms.ToolStripMenuItem wireFrameModeToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem orthogonalModeToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem noBackfaceCullingToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem newGameToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label TimeForGameLabel;

  }
}

