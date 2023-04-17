using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;




namespace Tomograph
{

    public partial class Form1 : Form
    {

        Bin bin = new Bin();
        View view = new View();
        bool loaded = false; // чтобы не запускать отрисовку, пока не загружены данные
        int currentLayer = 0; // номер слоя визуализации

        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);

        bool needReload = false;

        void displayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps={0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.readBIN(str);
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                trackBar1.Maximum = Bin.Z - 1;
                glControl1.Invalidate();
            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            
            if(radioButton1.Checked)
            {
                if (loaded)
                {
                    if (checkBox1.Checked) view.DrawQuadStrip(currentLayer);
                    else view.DrawQuads(currentLayer);
                    glControl1.SwapBuffers();
                }
            }
            else if (radioButton2.Checked)
            {
                if (loaded)
                {
                    if (needReload)
                    {
                        view.generateTextureImage(currentLayer);
                        view.Load2DTexture();
                        needReload = false;
                    }
                    view.DrawQuads(currentLayer);
                    glControl1.SwapBuffers();   //загружает наш буфер в буфер экрана.

                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            if(radioButton2.Checked) needReload = true;
        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label1.Text = "Minimal value (" + trackBar2.Value.ToString() + ") :";
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            label2.Text = "TF Width (" + trackBar3.Value.ToString() + ") :";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            view = new View(trackBar2.Value, trackBar2.Value + trackBar3.Value);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            view = new View();
        }
    }
}
