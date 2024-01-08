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
    }
}
