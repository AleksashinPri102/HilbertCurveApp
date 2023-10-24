using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HilbertCurveApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(1000, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private float LastX, LastY;
        private Bitmap HilbertImage;

        private void btnGo_Click(object sender, EventArgs e)
        {
            int depth = int.Parse(txtDepth.Text);
            if (depth > 8)
            {
                if (MessageBox.Show("A large depth may take a long time to draw (and will be mostly black anyway). Do you want to continue?",
                    "Continue?", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }

            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            float total_length, start_x, start_y, start_length;

            // Определяем, насколько большой мы можем сделать кривую.
            if (picCanvas.ClientSize.Height < picCanvas.ClientSize.Width)
            {
                total_length = (float)(0.9 * picCanvas.ClientSize.Height);
            }
            else
            {
                total_length = (float)(0.9 * picCanvas.ClientSize.Width);
            }

            start_x = (picCanvas.ClientSize.Width - total_length) / 2;
            start_y = (picCanvas.ClientSize.Height - total_length) / 2;

            // Вычисляем длину стороны для данного уровня глубины.
            start_length = (float)(total_length / (Math.Pow(2, depth) - 1));

            HilbertImage = new Bitmap(picCanvas.ClientSize.Width, picCanvas.ClientSize.Height);
            picCanvas.Image = HilbertImage;

            using (Graphics gr = Graphics.FromImage(HilbertImage))
            {
                // Рисуем кривую.
                gr.Clear(picCanvas.BackColor);
                LastX = (int)start_x;
                LastY = (int)start_y;
                Hilbert(gr, depth, start_length, 0);
            }

            // Отображаем результат.
            picCanvas.Refresh();
            this.Cursor = Cursors.Default;
        }
        private void btnNextStep_Click(object sender, EventArgs e)
        {
            int currentDepth = int.Parse(txtDepth.Text);
            currentDepth++;
            txtDepth.Text = currentDepth.ToString();

            btnGo_Click(sender, e);
        }
        // Рисуем кривую Гильберта.
        private void Hilbert(Graphics gr, int depth, float dx, float dy)
        {
            if (depth > 1) Hilbert(gr, depth - 1, dy, dx);
            DrawRelative(gr, dx, dy);
            if (depth > 1) Hilbert(gr, depth - 1, dx, dy);
            DrawRelative(gr, dy, dx);
            if (depth > 1) Hilbert(gr, depth - 1, dx, dy);
            DrawRelative(gr, -dx, -dy);
            if (depth > 1) Hilbert(gr, depth - 1, -dy, -dx);
        }

        // Рисуем линию (LastX, LastY)-(LastX + dx, LastY + dy) и
        // обновляем LastX = LastX + dx, LastY = LastY + dy.
        private void DrawRelative(Graphics gr, float dx, float dy)
        {
            gr.DrawLine(Pens.Black, LastX, LastY, LastX + dx, LastY + dy);
            LastX += dx;
            LastY += dy;
        }
    }
}