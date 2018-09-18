using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test2
{
    public partial class Form1 : Form
    {
        private Graphics g1, g2, g3, g4, g5;
        byte[] rgbValues1, rgbValues2, rgbValues3;
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string pict = openFileDialog1.FileName;
            pictureBox1.Image = Bitmap.FromFile(pict);
            pictureBox2.Image = Bitmap.FromFile(pict);
            pictureBox3.Image = Bitmap.FromFile(pict);
            g1 = Graphics.FromImage(pictureBox1.Image);
            g2 = Graphics.FromImage(pictureBox2.Image);
            g3 = Graphics.FromImage(pictureBox3.Image);
            Bitmap bmp1 = pictureBox1.Image as Bitmap;
            Bitmap bmp2 = pictureBox2.Image as Bitmap;
            Bitmap bmp3 = pictureBox3.Image as Bitmap;
            int [] img1 = new int[256];
            int [] img2 = new int[256];

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
            int bytes2 = Math.Abs(bmpData2.Stride) * bmp2.Height;
            int bytes3 = Math.Abs(bmpData3.Stride) * bmp3.Height;
            rgbValues1 = new byte[bytes1];
            rgbValues2 = new byte[bytes2];
            rgbValues3 = new byte[bytes3];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr1, rgbValues1, 0, bytes1);
            System.Runtime.InteropServices.Marshal.Copy(ptr2, rgbValues2, 0, bytes2);
            System.Runtime.InteropServices.Marshal.Copy(ptr3, rgbValues3, 0, bytes3);

            // Set every third value to 255. A 24bpp bitmap will look red.  
            //for (int counter = 0; counter < rgbValues1.Length; counter += 3)
            for (int counter = 0; counter < bytes1; counter += 3)
            {
                byte y1 = (byte)((rgbValues1[counter] + rgbValues1[counter + 1] + rgbValues1[counter + 2]) / 3);
                rgbValues1[counter] = y1;
                rgbValues1[counter + 1] = y1;
                rgbValues1[counter + 2] = y1;
                img1[y1]++;

                byte y2 = (byte)(0.299 * rgbValues2[counter + 2] + 0.587 * rgbValues2[counter + 1] + 0.114 * rgbValues2[counter]);
                rgbValues2[counter] = y2;
                rgbValues2[counter + 1] = y2;
                rgbValues2[counter + 2] = y2;
                img2[y2]++;

                rgbValues3[counter] = (byte)Math.Abs(y1 - y2);
                rgbValues3[counter + 1] = (byte)Math.Abs(y1 - y2);
                rgbValues3[counter + 2] = (byte)Math.Abs(y1 - y2);
                
            }
            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues1, 0, ptr1, bytes1);
            System.Runtime.InteropServices.Marshal.Copy(rgbValues2, 0, ptr2, bytes2);
            System.Runtime.InteropServices.Marshal.Copy(rgbValues3, 0, ptr3, bytes3);

            // Unlock the bits.
            bmp1.UnlockBits(bmpData1);
            bmp2.UnlockBits(bmpData2);
            bmp3.UnlockBits(bmpData3);

            int i = 0;
            for (int counter = 0; counter < rgbValues1.Length; counter += 3)
            {
                bmp1.SetPixel(i % bmp1.Width, i / bmp1.Width,
                    Color.FromArgb(rgbValues1[counter + 2], rgbValues1[counter + 1], rgbValues1[counter]));

                bmp2.SetPixel(i % bmp2.Width, i / bmp2.Width,
                    Color.FromArgb(rgbValues2[counter + 2], rgbValues2[counter + 1], rgbValues2[counter]));

                bmp3.SetPixel(i % bmp3.Width, i / bmp3.Width,
                    Color.FromArgb(rgbValues3[counter + 2], rgbValues3[counter + 1], rgbValues3[counter]));
                i++;

            }
            pictureBox1.Refresh();
            pictureBox2.Refresh();
            pictureBox3.Refresh();
            const int sd = 10;
            double norm1 = (double)pictureBox4.Height / img1.Max(), norm2 =  (double)pictureBox5.Height / img2.Max();
            Pen pen = new Pen(Color.CadetBlue);

            for (int j = sd; j < 256 + sd; ++j)
            {
                g4.DrawLine(pen, j, pictureBox4.Height - (int)(img1[j - sd] * norm1), j, pictureBox4.Height);
                g5.DrawLine(pen, j, pictureBox4.Height - (int)(img2[j - sd] * norm2), j, pictureBox4.Height);
            }

        }
        public Form1()
        {
            InitializeComponent();
            g1 = pictureBox1.CreateGraphics();
            g2 = pictureBox2.CreateGraphics();
            g3 = pictureBox3.CreateGraphics();
            g4 = pictureBox4.CreateGraphics();
            g5 = pictureBox5.CreateGraphics();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        

       }
}
