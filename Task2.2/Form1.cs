using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task2._2
{
	public partial class Form1 : Form
	{

		int[] countRed, countGreen, countBlue;
        int width = 1;
		public Form1()
		{
			InitializeComponent();
			pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
        }

		byte[] rgbValuesRed, rgbValuesGreen, rgbValuesBlue;
		private void Form1_Load(object sender, EventArgs e)
		{

		}

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            label1.Text = openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
		{
			pictureBox1.Image = Bitmap.FromFile(label1.Text);
            pictureBox3.Image = Bitmap.FromFile(label1.Text);
            pictureBox5.Image = Bitmap.FromFile(label1.Text);
			Graphics g1 = Graphics.FromImage(pictureBox1.Image);
            Graphics g2 = Graphics.FromImage(pictureBox3.Image);
            Graphics g3 = Graphics.FromImage(pictureBox5.Image);
            Bitmap bmp1 = pictureBox1.Image as Bitmap;
            Bitmap bmp2 = pictureBox3.Image as Bitmap;
            Bitmap bmp3 = pictureBox5.Image as Bitmap;

            width = System.Drawing.Image.FromFile(label1.Text).Width / 2;

            // Lock the bitmap's bits.  
            Rectangle rect1 = new Rectangle(0, 0, bmp1.Width, bmp1.Height);
			System.Drawing.Imaging.BitmapData bmpData1 =
				bmp1.LockBits(rect1, System.Drawing.Imaging.ImageLockMode.ReadWrite,
				bmp1.PixelFormat);
            Rectangle rect2 = new Rectangle(0, 0, bmp2.Width, bmp2.Height);
            System.Drawing.Imaging.BitmapData bmpData2 =
                bmp2.LockBits(rect2, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp2.PixelFormat);
            Rectangle rect3 = new Rectangle(0, 0, bmp3.Width, bmp3.Height);
            System.Drawing.Imaging.BitmapData bmpData3 =
                bmp3.LockBits(rect3, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp3.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr1 = bmpData1.Scan0;
            IntPtr ptr2 = bmpData2.Scan0;
            IntPtr ptr3 = bmpData3.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes1 = Math.Abs(bmpData1.Stride) * bmp1.Height;
			rgbValuesRed = new byte[bytes1];
            int bytes2 = Math.Abs(bmpData2.Stride) * bmp2.Height;
            rgbValuesGreen = new byte[bytes2];
            int bytes3 = Math.Abs(bmpData3.Stride) * bmp3.Height;
            rgbValuesBlue = new byte[bytes3];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr1, rgbValuesRed, 0, bytes1);
            System.Runtime.InteropServices.Marshal.Copy(ptr2, rgbValuesGreen, 0, bytes2);
            System.Runtime.InteropServices.Marshal.Copy(ptr3, rgbValuesBlue, 0, bytes3);

            // Set every third value to 255. A 24bpp bitmap will look red.  
            for (int counter = 0; counter < rgbValuesRed.Length; counter += 3)
			{
				rgbValuesRed[counter+1] = 0;
				rgbValuesRed[counter] = 0;

                rgbValuesGreen[counter + 2] = 0;
                rgbValuesGreen[counter] = 0;

                rgbValuesBlue[counter + 1] = 0;
                rgbValuesBlue[counter + 2] = 0;

                // 0 - blue, 1 - green, 2 - red
            }
			// Copy the RGB values back to the bitmap
			System.Runtime.InteropServices.Marshal.Copy(rgbValuesRed, 0, ptr1, bytes1);
            System.Runtime.InteropServices.Marshal.Copy(rgbValuesGreen, 0, ptr2, bytes2);
            System.Runtime.InteropServices.Marshal.Copy(rgbValuesBlue, 0, ptr3, bytes3);

            // Unlock the bits.
            bmp1.UnlockBits(bmpData1);
            bmp2.UnlockBits(bmpData2);
            bmp3.UnlockBits(bmpData3);

            int i = 0;
			/*for (int counter = 0; counter < rgbValuesRed.Length; counter += 3)
			{
				bmp1.SetPixel(i % bmp1.Width, i / bmp1.Width,
					Color.FromArgb(rgbValuesRed[counter+2], rgbValuesRed[counter + 1], rgbValuesRed[counter]));
                bmp2.SetPixel(i % bmp2.Width, i / bmp2.Width,
                    Color.FromArgb(rgbValuesGreen[counter + 2], rgbValuesGreen[counter + 1], rgbValuesGreen[counter]));
                bmp3.SetPixel(i % bmp3.Width, i / bmp3.Width,
                    Color.FromArgb(rgbValuesBlue[counter + 2], rgbValuesBlue[counter + 1], rgbValuesBlue[counter]));
                i++;

			}*/

			countRed = new int[256];
            countGreen = new int[256];
            countBlue = new int[256];
            countColors();


			pictureBox1.Refresh();
            this.pictureBox2.Visible = true;
            pictureBox2.Refresh();

            pictureBox3.Refresh();
            this.pictureBox4.Visible = true;
            pictureBox4.Refresh();

            pictureBox5.Refresh();
            this.pictureBox6.Visible = true;
            pictureBox6.Refresh();
        }

        private void countColors()
		{
			for (int counter = 0; counter < rgbValuesRed.Length; counter += 3)
            {
                countBlue[rgbValuesBlue[counter]]++;
                countGreen[rgbValuesGreen[counter + 1]]++;
                countRed[rgbValuesRed[counter + 2]]++;
            }
		}

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            System.Drawing.Graphics g;
            g = e.Graphics;
            int h = this.pictureBox2.Height;
            Pen myPen = new Pen(System.Drawing.Color.Red, 1);
            for (int i = 0; i < 256; ++i)
                g.DrawLine(myPen, i, h, i, h - (countRed[i]) / width);
        }

        private void pictureBox4_Paint(object sender, PaintEventArgs e)
        {
            System.Drawing.Graphics g;
            g = e.Graphics;
            int h = this.pictureBox4.Height;
            Pen myPen = new Pen(System.Drawing.Color.Green, 1);
            for (int i = 0; i < 256; ++i)
                g.DrawLine(myPen, i, h, i, h - (countGreen[i]) / width);
        }

        private void pictureBox6_Paint(object sender, PaintEventArgs e)
        {
            System.Drawing.Graphics g;
            g = e.Graphics;
            int h = this.pictureBox6.Height;
            Pen myPen = new Pen(System.Drawing.Color.Blue, 1);
            for (int i = 0; i < 256; ++i)
                g.DrawLine(myPen, i, h, i, h - (countBlue[i]) / width);
        }
    }
}
