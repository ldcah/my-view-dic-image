namespace MyViewROI
{
    partial class MyHalconWindow
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.hWindowControl1 = new HalconDotNet.HWindowControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.加载图片ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.图片另存为ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开文件位置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveAndZoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NoActionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4ROI = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblImageName = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // hWindowControl1
            // 
            this.hWindowControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.hWindowControl1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.hWindowControl1.ContextMenuStrip = this.contextMenuStrip1;
            this.hWindowControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindowControl1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl1.Location = new System.Drawing.Point(0, 0);
            this.hWindowControl1.Name = "hWindowControl1";
            this.hWindowControl1.Size = new System.Drawing.Size(680, 484);
            this.hWindowControl1.TabIndex = 3;
            this.hWindowControl1.WindowSize = new System.Drawing.Size(680, 484);
            this.hWindowControl1.SizeChanged += new System.EventHandler(this.hWindowControl1_SizeChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.加载图片ToolStripMenuItem,
            this.图片另存为ToolStripMenuItem,
            this.打开文件位置ToolStripMenuItem,
            this.MoveAndZoomToolStripMenuItem,
            this.MoveToolStripMenuItem,
            this.NoActionToolStripMenuItem,
            this.ResetToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 180);
            // 
            // 加载图片ToolStripMenuItem
            // 
            this.加载图片ToolStripMenuItem.Name = "加载图片ToolStripMenuItem";
            this.加载图片ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.加载图片ToolStripMenuItem.Text = "图像-加载";
            this.加载图片ToolStripMenuItem.Click += new System.EventHandler(this.加载图片ToolStripMenuItem_Click);
            // 
            // 图片另存为ToolStripMenuItem
            // 
            this.图片另存为ToolStripMenuItem.Name = "图片另存为ToolStripMenuItem";
            this.图片另存为ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.图片另存为ToolStripMenuItem.Text = "图像-另存为...";
            this.图片另存为ToolStripMenuItem.Click += new System.EventHandler(this.图片另存为ToolStripMenuItem_Click);
            // 
            // 打开文件位置ToolStripMenuItem
            // 
            this.打开文件位置ToolStripMenuItem.Name = "打开文件位置ToolStripMenuItem";
            this.打开文件位置ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.打开文件位置ToolStripMenuItem.Text = "图像-所在位置";
            this.打开文件位置ToolStripMenuItem.Click += new System.EventHandler(this.打开文件位置ToolStripMenuItem_Click);
            // 
            // MoveAndZoomToolStripMenuItem
            // 
            this.MoveAndZoomToolStripMenuItem.Name = "MoveAndZoomToolStripMenuItem";
            this.MoveAndZoomToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.MoveAndZoomToolStripMenuItem.Text = "缩放+移动";
            this.MoveAndZoomToolStripMenuItem.Visible = false;
            this.MoveAndZoomToolStripMenuItem.Click += new System.EventHandler(this.ZoomToolStripMenuItem_Click);
            // 
            // MoveToolStripMenuItem
            // 
            this.MoveToolStripMenuItem.Name = "MoveToolStripMenuItem";
            this.MoveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.MoveToolStripMenuItem.Text = "仅移动";
            this.MoveToolStripMenuItem.Visible = false;
            this.MoveToolStripMenuItem.Click += new System.EventHandler(this.MoveToolStripMenuItem_Click);
            // 
            // NoActionToolStripMenuItem
            // 
            this.NoActionToolStripMenuItem.Name = "NoActionToolStripMenuItem";
            this.NoActionToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.NoActionToolStripMenuItem.Text = "无动作";
            this.NoActionToolStripMenuItem.Visible = false;
            this.NoActionToolStripMenuItem.Click += new System.EventHandler(this.NoActionToolStripMenuItem_Click);
            // 
            // ResetToolStripMenuItem
            // 
            this.ResetToolStripMenuItem.Name = "ResetToolStripMenuItem";
            this.ResetToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.ResetToolStripMenuItem.Text = "复位窗口";
            this.ResetToolStripMenuItem.Click += new System.EventHandler(this.ResetToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.statusStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel4ROI,
            this.lblImageName});
            this.statusStrip1.Location = new System.Drawing.Point(0, 459);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(680, 25);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            this.statusStrip1.Visible = false;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F);
            this.toolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(171, 20);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel4ROI
            // 
            this.toolStripStatusLabel4ROI.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.toolStripStatusLabel4ROI.Name = "toolStripStatusLabel4ROI";
            this.toolStripStatusLabel4ROI.Size = new System.Drawing.Size(30, 20);
            this.toolStripStatusLabel4ROI.Text = "ROI";
            // 
            // lblImageName
            // 
            this.lblImageName.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F);
            this.lblImageName.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblImageName.Name = "lblImageName";
            this.lblImageName.Size = new System.Drawing.Size(464, 20);
            this.lblImageName.Spring = true;
            this.lblImageName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MyHalconWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.hWindowControl1);
            this.Name = "MyHalconWindow";
            this.Size = new System.Drawing.Size(680, 484);
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public HalconDotNet.HWindowControl hWindowControl1;
        private System.Windows.Forms.ToolStripMenuItem MoveAndZoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MoveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NoActionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ResetToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4ROI;
        private System.Windows.Forms.ToolStripStatusLabel lblImageName;
        private System.Windows.Forms.ToolStripMenuItem 加载图片ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 图片另存为ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开文件位置ToolStripMenuItem;
    }
}
