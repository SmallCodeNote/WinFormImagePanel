using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImagePanel
{
    class PictureBoxScroll
    {
        Panel _parentPanel;
        Panel _pictureFramePanel;
        int scrollBarWidth = 25;
        private PictureBox pictureBox1;
        private VScrollBar vScrollBar1;
        private HScrollBar hScrollBar1;


        Point MouseLocation_MouseDown;
        Point MouseLocation_Now;
        bool MouseLeftDown = false;
        bool MouseRightDown = false;
        bool MouseMiddleDown = false;
        bool MouseOnImage = false;

        public double factor = 1.00;

        /// <summary>
        /// image expanding focus point on image coordinate
        /// </summary>
        Point imageFocusPoint;

        /// <summary>
        /// image expanding focus point on panel coordinate
        /// </summary>
        Point panelFocusPoint;

        public PictureBoxScroll(Panel parentPanel)
        {
            _parentPanel = parentPanel;
            InitializeComponent();

        }

        
        private void updatePictureBox(PictureBox pictureBox, Bitmap bitmap)
        {
            if (pictureBox.Image != null) { pictureBox.Image.Dispose(); }
            pictureBox.Image = bitmap;
            pictureBox.Width = bitmap.Width;
            pictureBox.Height = bitmap.Height;

        }

        public void setPicture(Bitmap img)
        {
            updatePictureBox(pictureBox1, img);
            updateScrollBarParam();

        }

        private void InitializeComponent()
        {
            this._pictureFramePanel = new Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this._pictureFramePanel.SuspendLayout();
            this._parentPanel.SuspendLayout();


            // 
            // _pictureFramePanel_Panel
            // 
            this._pictureFramePanel.Controls.Add(this.pictureBox1);
            this._pictureFramePanel.Location = new System.Drawing.Point(0, 0);
            this._pictureFramePanel.Name = "_pictureFramePanel_Panel";
            this._pictureFramePanel.Size = new System.Drawing.Size(_parentPanel.Width - scrollBarWidth, _parentPanel.Height - scrollBarWidth);
            this._pictureFramePanel.TabIndex = 0;
            this._pictureFramePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this._pictureFramePanel_Panel_MouseDown);
            this._pictureFramePanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this._pictureFramePanel_Panel_MouseUp);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = _pictureFramePanel.Name + "_pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(_parentPanel.Width - scrollBarWidth, _parentPanel.Height - scrollBarWidth);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;

            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this._pictureFramePanel_MouseDown);
            this.pictureBox1.MouseEnter += new System.EventHandler(this._pictureFramePanel_MouseEnter);
            this.pictureBox1.MouseLeave += new System.EventHandler(this._pictureFramePanel_MouseLeave);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this._pictureFramePanel_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this._pictureFramePanel_MouseUp);

            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Location = new System.Drawing.Point(_parentPanel.Width- scrollBarWidth, 0);
            this.vScrollBar1.Name = _parentPanel.Name + "_vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(scrollBarWidth, _parentPanel.Height- scrollBarWidth);
            this.vScrollBar1.TabIndex = 1;
            this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Location = new System.Drawing.Point(0, _parentPanel.Height - scrollBarWidth);
            this.hScrollBar1.Name = _parentPanel.Name + "_hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(_parentPanel.Width - scrollBarWidth, scrollBarWidth);
            this.hScrollBar1.TabIndex = 2;
            this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
            // 
            // Form1
            //
            this._pictureFramePanel.Controls.Add(this.pictureBox1);

            this._parentPanel.Controls.Add(this._pictureFramePanel);
            this._parentPanel.Controls.Add(this.hScrollBar1);
            this._parentPanel.Controls.Add(this.vScrollBar1);
            //this._pictureFramePanel.Name = "Form1";
            //this._pictureFramePanel.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();

            this._pictureFramePanel.ResumeLayout(false);
            this._parentPanel.ResumeLayout(false);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="X">pictureBox coordinate</param>
        /// <param name="Y">pictureBox coordinate</param>
        private void setImageFocusPoint(int X, int Y)
        {
            int Factor = (int)(factor*100);

            imageFocusPoint.X = (X * 100) / Factor;
            imageFocusPoint.Y = (Y * 100) / Factor;

            getMouseReport();
        }

        private void setImageFocusPointFromViewCenter()
        {
            if (this.pictureBox1.Image != null)
            {
                int Factor = (int)(factor * 100);
                imageFocusPoint.X = ((-this.pictureBox1.Left + this._pictureFramePanel.Width / 2) * 100) / Factor;
                imageFocusPoint.Y = ((-this.pictureBox1.Top + this._pictureFramePanel.Height / 2) * 100) / Factor;
            }

            getMouseReport();

        }

        private void setImageFocusPointFromImageCenter()
        {
            if (this.pictureBox1.Image != null)
            {
                imageFocusPoint.X = this.pictureBox1.Image.Width / 2;
                imageFocusPoint.Y = this.pictureBox1.Image.Height / 2;
            }
            getMouseReport();
        }

        private void setPanelFocusPointFromImagePoint(int X, int Y)
        {
            if (this.pictureBox1.Image != null)
            {
                panelFocusPoint.X = X + this.pictureBox1.Location.X;
                panelFocusPoint.Y = Y + this.pictureBox1.Location.Y;
            }

            getMouseReport();
        }


        private void setPanelFocusPointOnCenter()
        {
            panelFocusPoint.X = this._pictureFramePanel.Width / 2;
            panelFocusPoint.Y = this._pictureFramePanel.Height / 2;
            getMouseReport();
        }
        private string getMouseReport()
        {
            string report = imageFocusPoint.ToString() + " " +
                //MouseLocation_Now.ToString() + " " +
                panelFocusPoint.ToString() + " " +
                (MouseLeftDown ? 1 : 0).ToString() + " " +
                (MouseRightDown ? 1 : 0).ToString() + " " +
                (MouseMiddleDown ? 1 : 0).ToString() + " " +
                (MouseOnImage ? 1 : 0).ToString()
                ;

            //this.label_MouseInfo.Text = report;
            return report;

        }

        private void updateScrollBarParam()
        {
            if (this.pictureBox1.Image.Width > this._pictureFramePanel.Width)
            {
                this.hScrollBar1.Enabled = true;
                this.hScrollBar1.Maximum = this.pictureBox1.Image.Width;
                this.hScrollBar1.LargeChange = this._pictureFramePanel.Width;
            }

            if (this.pictureBox1.Image.Height > this._pictureFramePanel.Height)
            {
                this.vScrollBar1.Enabled = true;
                this.vScrollBar1.Maximum = this.pictureBox1.Image.Height;
                this.vScrollBar1.LargeChange = this._pictureFramePanel.Height;
            }

        }

        private bool updateScrollBarPosition()
        {
            try
            {
                this.hScrollBar1.Value = -pictureBox1.Location.X;
                this.vScrollBar1.Value = -pictureBox1.Location.Y;
            }
            catch { return false; }

            return true;
        }

        private void _pictureFramePanel_MouseDown(object sender, MouseEventArgs e)
        {
            MouseLocation_MouseDown = e.Location;
            if (e.Button == MouseButtons.Left) MouseLeftDown = true;
            if (e.Button == MouseButtons.Middle) MouseMiddleDown = true;
            if (e.Button == MouseButtons.Right) MouseRightDown = true;

            getMouseReport();
        }

        private void _pictureFramePanel_MouseUp(object sender, MouseEventArgs e)
        {
            MouseLocation_MouseDown = e.Location;
            if (e.Button == MouseButtons.Left) MouseLeftDown = false;
            if (e.Button == MouseButtons.Middle) MouseMiddleDown = false;
            if (e.Button == MouseButtons.Right) MouseRightDown = false;

            getMouseReport();

        }

        private void _pictureFramePanel_MouseEnter(object sender, EventArgs e)
        {
            MouseOnImage = true;
            getMouseReport();
        }

        private void _pictureFramePanel_MouseLeave(object sender, EventArgs e)
        {
            MouseOnImage = false;
            setPanelFocusPointOnCenter();
            setImageFocusPointFromViewCenter();

            getMouseReport();
        }

        private void _pictureFramePanel_MouseMove(object sender, MouseEventArgs e)
        {
            MouseLocation_Now = e.Location;

            if (MouseOnImage) setPanelFocusPointFromImagePoint(e.X, e.Y);
            if (MouseOnImage) setImageFocusPoint(e.X, e.Y);

            if (MouseMiddleDown)
            {
                Point pb = this.pictureBox1.Location;
                int newLeft = pb.X + MouseLocation_Now.X - MouseLocation_MouseDown.X;
                int newTop = pb.Y + MouseLocation_Now.Y - MouseLocation_MouseDown.Y;

                //if (-newTop >= vScrollBar1.Minimum && -newTop <= vScrollBar1.Maximum - this._pictureFramePanel.Height)
                {
                    this.pictureBox1.Top = newTop;
                }
                //if (-newLeft >= hScrollBar1.Minimum && -newLeft <= hScrollBar1.Maximum - this._pictureFramePanel.Width)
                {
                    this.pictureBox1.Left = newLeft;
                }

                updateScrollBarPosition();

            }

            getMouseReport();

        }


        private void _pictureFramePanel_Panel_MouseDown(object sender, MouseEventArgs e)
        {
            MouseLocation_MouseDown = e.Location;
            if (e.Button == MouseButtons.Left) MouseLeftDown = true;
            if (e.Button == MouseButtons.Middle) MouseMiddleDown = true;
            if (e.Button == MouseButtons.Right) MouseRightDown = true;

            getMouseReport();
        }

        private void _pictureFramePanel_Panel_MouseUp(object sender, MouseEventArgs e)
        {
            MouseLocation_MouseDown = e.Location;
            if (e.Button == MouseButtons.Left) MouseLeftDown = false;
            if (e.Button == MouseButtons.Middle) MouseMiddleDown = false;
            if (e.Button == MouseButtons.Right) MouseRightDown = false;

            getMouseReport();

        }

        private void _pictureFramePanel_Panel_MouseEnter(object sender, EventArgs e)
        {
            MouseOnImage = true;
            getMouseReport();
        }

        private void _pictureFramePanel_Panel_MouseLeave(object sender, EventArgs e)
        {
            MouseOnImage = false;
            setPanelFocusPointOnCenter();
            setImageFocusPointFromViewCenter();

            getMouseReport();
        }

        private void _pictureFramePanel_Panel_MouseMove(object sender, MouseEventArgs e)
        {
            MouseLocation_Now = e.Location;

            if (MouseOnImage) setPanelFocusPointFromImagePoint(e.X, e.Y);
            if (MouseOnImage) setImageFocusPoint(e.X, e.Y);

            if (MouseMiddleDown)
            {
                Point pb = this.pictureBox1.Location;
                int newLeft = pb.X + MouseLocation_Now.X - MouseLocation_MouseDown.X;
                int newTop = pb.Y + MouseLocation_Now.Y - MouseLocation_MouseDown.Y;

                //if (-newTop >= vScrollBar1.Minimum && -newTop <= vScrollBar1.Maximum - this._pictureFramePanel_Panel.Height)
                {
                    this.pictureBox1.Top = newTop;
                }
                //if (-newLeft >= hScrollBar1.Minimum && -newLeft <= hScrollBar1.Maximum - this._pictureFramePanel_Panel.Width)
                {
                    this.pictureBox1.Left = newLeft;
                }

                updateScrollBarPosition();

            }

            getMouseReport();

        }


        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            this.pictureBox1.Top = -this.vScrollBar1.Value;
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            this.pictureBox1.Left = -this.hScrollBar1.Value;
        }


    }
}
