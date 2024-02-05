using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        private ContextMenu contextMenu;


        Bitmap bitmap_source;
        Bitmap bitmap_show;

        List<System.Drawing.Point> pointList;

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

            pointList = new List<Point>();

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

            // 
            // ContextMenu
            //
            this.contextMenu = new ContextMenu();

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

            if (MouseOnImage && MouseLeftDown && (Control.ModifierKeys & Keys.Alt) == Keys.Alt)
            {
                pictureBox1.Top = (int)((-imageSourceFocusPoint.Y) * factor) + _parentPanel.Height / 2;
                pictureBox1.Left = (int)((-imageSourceFocusPoint.X) * factor) + _parentPanel.Width / 2;

            }

            if (MouseOnImage && MouseLeftDown && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (!listPointHaveNear(pointList, imageSourceFocusPoint, drawPointDiameter / 2))
                {
                    pointList.Add(imageSourceFocusPoint);
                    Refresh();
                }
                else
                {
                    pointListElement_MoveTarget = getNearPointFromlist(pointList, imageSourceFocusPoint, drawPointDiameter / 2);
                }
            }

            if (MouseOnImage && MouseRightDown && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (listPointHaveNear(pointList, imageSourceFocusPoint, drawPointDiameter / 2))
                {
                    int pointListElement_RemoveTarget = getNearPointFromlist(pointList, imageSourceFocusPoint, drawPointDiameter / 2);

                    pointList.RemoveAt(pointListElement_RemoveTarget);
                    Refresh();

                }
            }
            else if (MouseOnImage && MouseRightDown)
            {

                if (listPointHaveNear(pointList, imageSourceFocusPoint, drawPointDiameter / 2))
                {
                    MouseRightDown = false;
                    this.contextMenu.MenuItems.Clear();
                    this.contextMenu.MenuItems.Add(new MenuItem("SendListSetToClipboard", new EventHandler(this.MenuItem_SendListSetToClipboard_Click)));
                    this.contextMenu.MenuItems.Add(new MenuItem("ClearPointList", new EventHandler(this.MenuItem_ClearPointList_Click)));
                    this.contextMenu.Show(pictureBox1, new Point(e.X, e.Y));

                }
                else
                {
                    MouseRightDown = false;
                    this.contextMenu.MenuItems.Clear();
                    this.contextMenu.MenuItems.Add(new MenuItem("GetListFromClipboard", new EventHandler(this.MenuItem_GetListFromClipboard_Click)));
                    this.contextMenu.Show(pictureBox1, new Point(e.X, e.Y));

                }

            };

            getMouseReport();
        }

        private void MenuItem_SendListSetToClipboard_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var point in pointList)
            {
                sb.Append(point.ToString());
            }

            Clipboard.SetText(sb.ToString()+"\r\n");

        }

        private void MenuItem_GetListFromClipboard_Click(object sender, EventArgs e)
        {

            string PointsString =  Clipboard.GetText();
            getListFromString(PointsString);

        }

        private void MenuItem_ClearPointList_Click(object sender, EventArgs e)
        {
            pointList.Clear();
            Refresh();
        }


        int pointListElement_MoveTarget = int.MaxValue;


        bool listPointHaveNear(List<Point> points, Point targetPoint, double distance)
        {
            var result = points.Where(x => Math.Sqrt(Math.Pow(x.X - targetPoint.X, 2) + Math.Pow(x.Y - targetPoint.Y, 2)) <= distance).ToList();
            if (result.Count > 0) return true;
            return false;
        }

        int getNearPointFromlist(List<Point> points, Point targetPoint, double distance)
        {
            var result = points.Where(x => Math.Sqrt(Math.Pow(x.X - targetPoint.X, 2) + Math.Pow(x.Y - targetPoint.Y, 2)) <= distance).ToList();

            for (int listIndex = 0; listIndex < points.Count; listIndex++)
            {
                var p = points[listIndex];
                if (p.X == result[0].X && p.Y == result[0].Y) return listIndex;

            }

            return int.MaxValue;

        }


        public float drawPointDiameter = 16;

        void drawShowBitmap()
        {

            Graphics g = Graphics.FromImage(bitmap_show);

            g.DrawImage(bitmap_source, 0, 0);

            for (int pointIndex = 0; pointIndex < pointList.Count; pointIndex++)
            {
                g.DrawEllipse(Pens.Red, pointList[pointIndex].X - drawPointDiameter / 2, pointList[pointIndex].Y - drawPointDiameter / 2, drawPointDiameter, drawPointDiameter);
            }

            for (int pointIndex = 0; pointIndex < pointList.Count - 1; pointIndex++)
            {
                g.DrawLine(Pens.Red, pointList[pointIndex], pointList[pointIndex + 1]);
            }

            if (pointList.Count > 2)
            {
                g.DrawLine(Pens.Red, pointList[0], pointList[pointList.Count - 1]);

            }
            g.Dispose();

        }

        private void _pictureFramePanel_MouseUp(object sender, MouseEventArgs e)
        {
            MouseLocation_MouseDown = e.Location;
            if (e.Button == MouseButtons.Left) MouseLeftDown = false;
            if (e.Button == MouseButtons.Middle) MouseMiddleDown = false;
            if (e.Button == MouseButtons.Right) MouseRightDown = false;

            getMouseReport();
            pointListElement_MoveTarget = int.MaxValue;
        }

        private void _pictureFramePanel_MouseEnter(object sender, EventArgs e)
        {
            MouseOnImage = true;
            getMouseReport();
        }

        private void _pictureFramePanel_MouseLeave(object sender, EventArgs e)
        {
            MouseOnImage = false;
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
                if (MouseLeftDown && pointListElement_MoveTarget != int.MaxValue)
                {

                    int listIndex = pointListElement_MoveTarget;

                    if (pointList[listIndex] != imageSourceFocusPoint)
                    {
                        pointList[listIndex] = new Point(imageSourceFocusPoint.X, imageSourceFocusPoint.Y);
                    }

                    Refresh();

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

            pointListElement_MoveTarget = int.MaxValue;

        }

        private void _pictureFramePanel_Panel_MouseEnter(object sender, EventArgs e)
        {
            MouseOnImage = true;
            getMouseReport();
        }

        private void _pictureFramePanel_Panel_MouseLeave(object sender, EventArgs e)
        {
            MouseOnImage = false;
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

        public void Refresh()
        {
            drawShowBitmap();
            pictureBox1.Refresh();
        }

        public void getListFromString(string PointsString)
        {
            MatchCollection matches = Regex.Matches(PointsString, @"\{X=(\d+),Y=(\d+)\}");
            pointList.Clear();


            foreach (Match match in matches)
            {
                int x = int.Parse(match.Groups[1].Value);
                int y = int.Parse(match.Groups[2].Value);
                pointList.Add(new Point(x, y));
            }

            Refresh();

        }

    }
}
