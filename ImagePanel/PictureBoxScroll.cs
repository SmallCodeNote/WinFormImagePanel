﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;

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

        Bitmap bitmap_source;
        Bitmap bitmap_show;

        List<System.Drawing.Point> points;


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
        Point imageSourceFocusPoint;

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
            if (bitmap_source != null) bitmap_source.Dispose();
            if (bitmap_show != null) bitmap_show.Dispose();

            bitmap_source = (Bitmap)img.Clone();
            bitmap_show = (Bitmap)img.Clone();

            updatePictureBox(pictureBox1, bitmap_show);
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

            points = new List<Point>();

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
            this._pictureFramePanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this._pictureFramePanel_MouseMove);

            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = _pictureFramePanel.Name + "_pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(_parentPanel.Width - scrollBarWidth, _parentPanel.Height - scrollBarWidth);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            this.pictureBox1.MouseEnter += new System.EventHandler(this._pictureFramePanel_MouseEnter);
            this.pictureBox1.MouseLeave += new System.EventHandler(this._pictureFramePanel_MouseLeave);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this._pictureFramePanel_MouseDown);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this._pictureFramePanel_MouseUp);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this._pictureFramePanel_MouseMove);
            this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(_pictureFramePanel_MouseWheel);


            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Location = new System.Drawing.Point(_parentPanel.Width - scrollBarWidth, 0);
            this.vScrollBar1.Name = _parentPanel.Name + "_vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(scrollBarWidth, _parentPanel.Height - scrollBarWidth);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();

            this._pictureFramePanel.ResumeLayout(false);
            this._parentPanel.ResumeLayout(false);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="X">pictureBox coordinate</param>
        /// <param name="Y">pictureBox coordinate</param>
        private void setImageSourceFocusPoint(Point pictureBoxPoint)
        {
            imageSourceFocusPoint.X = (int)(pictureBoxPoint.X / factor);
            imageSourceFocusPoint.Y = (int)(pictureBoxPoint.Y / factor);

            Debug.WriteLine("im " + imageSourceFocusPoint.ToString());
            getMouseReport();
        }

        private void setImageSourceFocusPointFromViewCenter()
        {
            if (this.pictureBox1.Image != null)
            {
                int Factor = (int)(factor * 100);
                imageSourceFocusPoint.X = ((-this.pictureBox1.Left + this._pictureFramePanel.Width / 2) * 100) / Factor;
                imageSourceFocusPoint.Y = ((-this.pictureBox1.Top + this._pictureFramePanel.Height / 2) * 100) / Factor;
            }

            getMouseReport();

        }

        private void setImageSourceFocusPointFromImageCenter()
        {
            if (this.pictureBox1.Image != null)
            {
                imageSourceFocusPoint.X = this.pictureBox1.Image.Width / 2;
                imageSourceFocusPoint.Y = this.pictureBox1.Image.Height / 2;
            }
            getMouseReport();
        }

        private void setPanelFocusPointFromImagePoint(Point pictureBoxPoint)
        {

            if (this.pictureBox1.Image != null)
            {
                panelFocusPoint.X = pictureBoxPoint.X + this.pictureBox1.Location.X;
                panelFocusPoint.Y = pictureBoxPoint.Y + this.pictureBox1.Location.Y;
            }

            Debug.WriteLine("pp " + panelFocusPoint.ToString());


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
            string report = imageSourceFocusPoint.ToString() + " " +
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
                this.hScrollBar1.Maximum = this.pictureBox1.Width;
                this.hScrollBar1.LargeChange = this._pictureFramePanel.Width;
            }

            if (this.pictureBox1.Image.Height > this._pictureFramePanel.Height)
            {
                this.vScrollBar1.Enabled = true;
                this.vScrollBar1.Maximum = this.pictureBox1.Height;
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
            
            if (MouseOnImage && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                pictureBox1.Top = (int)((-imageSourceFocusPoint.Y) * factor) + _parentPanel.Height / 2;
                pictureBox1.Left = (int)((-imageSourceFocusPoint.X) * factor) + _parentPanel.Width / 2;

            }

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
            setImageSourceFocusPointFromViewCenter();

            getMouseReport();
        }

        private void _pictureFramePanel_MouseMove(object sender, MouseEventArgs e)
        {
            MouseLocation_Now = e.Location;
            setPanelFocusPointFromImagePoint(e.Location);
            setImageSourceFocusPoint(e.Location);

            if (MouseOnImage)
            {
                if (MouseMiddleDown)
                {
                    Point pb = this.pictureBox1.Location;
                    int newTop = -(pb.Y + (MouseLocation_Now.Y - MouseLocation_MouseDown.Y));
                    int newLeft = -(pb.X + (MouseLocation_Now.X - MouseLocation_MouseDown.X));


                    if (newTop > vScrollBar1.Maximum - vScrollBar1.LargeChange) { newTop = vScrollBar1.Maximum - vScrollBar1.LargeChange; }
                    if (newTop < vScrollBar1.Minimum) { newTop = vScrollBar1.Minimum; }

                    vScrollBar1.Value = newTop;
                    vScrollBar1_Scroll(null, null);


                    if (newLeft > hScrollBar1.Maximum - hScrollBar1.LargeChange) { newLeft = hScrollBar1.Maximum - hScrollBar1.LargeChange; }
                    if (newLeft < hScrollBar1.Minimum) { newLeft = hScrollBar1.Minimum; }

                    hScrollBar1.Value = newLeft;
                    hScrollBar1_Scroll(null, null);

                }

                getMouseReport();
            }
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
            setImageSourceFocusPointFromViewCenter();

            getMouseReport();
        }

        private void _pictureFramePanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (MouseOnImage && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                double factorStep = 1200.0;
                if (factor > 2) { factorStep = 300; };

                double nextFactor = factor + e.Delta / factorStep;
                if (nextFactor < 0.1) { nextFactor = 0.1; }
                if (nextFactor > 10) { nextFactor = 10.0; }

                double deltaFactor = nextFactor - factor;
                factor = nextFactor;

                Debug.WriteLine("factor = " + factor.ToString());

                pictureBox1.Width = (int)(bitmap_source.Width * factor);
                pictureBox1.Height = (int)(bitmap_source.Height * factor);

                pictureBox1.Top = (int)((-imageSourceFocusPoint.Y) * factor) + panelFocusPoint.Y;
                pictureBox1.Left = (int)((-imageSourceFocusPoint.X) * factor) + panelFocusPoint.X;

                updateScrollBarParam();

            }
            else if (MouseOnImage && !MouseRightDown)
            {
                int deltaStep = vScrollBar1.LargeChange / 2;
                if (e.Delta < 0) { deltaStep *= -1; }

                int nextValue = vScrollBar1.Value - deltaStep;

                if (nextValue > vScrollBar1.Maximum - vScrollBar1.LargeChange) { nextValue = vScrollBar1.Maximum - vScrollBar1.LargeChange; }
                if (nextValue < vScrollBar1.Minimum) { nextValue = vScrollBar1.Minimum; }

                vScrollBar1.Value = nextValue;
                vScrollBar1_Scroll(null, null);

            }
            else if (MouseOnImage && MouseRightDown)
            {
                int deltaStep = hScrollBar1.LargeChange / 2;
                if (e.Delta < 0) { deltaStep *= -1; }

                int nextValue = hScrollBar1.Value - deltaStep;

                if (nextValue > hScrollBar1.Maximum - hScrollBar1.LargeChange) { nextValue = hScrollBar1.Maximum - hScrollBar1.LargeChange; }
                if (nextValue < hScrollBar1.Minimum) { nextValue = hScrollBar1.Minimum; }

                hScrollBar1.Value = nextValue;
                hScrollBar1_Scroll(null, null);

            }

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
