using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImagePanel
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        PictureBoxScroll pictureBoxScroll;

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBoxScroll = new PictureBoxScroll(panel1);
        }

        private void button_LoadImage_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK) return;

            pictureBoxScroll.setPicture(new Bitmap(ofd.FileName));

        }

        private void button_CreateTestImage_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG|*.png";
            sfd.FileName = "testImage001.png";

            if (sfd.ShowDialog() != DialogResult.OK) return;


            Bitmap testImage = new Bitmap(2048, 2048);

            Graphics g = Graphics.FromImage(testImage);
            g.FillRectangle(Brushes.White, 0, 0, testImage.Width, testImage.Height);

            Font corFont = new Font("MS UI Gothic", 8);
            StringFormat corFormat = new StringFormat();
            corFormat.FormatFlags = StringFormatFlags.DirectionVertical;

            for (int px = 0; px < testImage.Width; px += 100)
            {
                g.DrawLine(Pens.Black, px, 0, px, testImage.Height);

                for (int py = 0; py < testImage.Height; py += 100)
                {
                    g.DrawString(px.ToString(), corFont, Brushes.Black, px, py , corFormat);
                }

            }


            for (int py = 0; py < testImage.Height; py += 100)
            {
                g.DrawLine(Pens.Black, 0, py, testImage.Width, py);
                for (int px = 0; px < testImage.Width; px += 100)
                {
                    g.DrawString(py.ToString(), corFont, Brushes.Black, px + 16, py);
                }

            }

            testImage.Save(sfd.FileName);
            testImage.Dispose();

        }
    }
}
