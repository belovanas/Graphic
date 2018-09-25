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
        byte[] rgbValues1, rgbValues2, rgbValues3;
        double[] hsvValues2, hsvValues3;
        double hue_change, saturation_change, value_change = 0;

        //Применение введенных пользователем изменений
        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox3.Image = (Bitmap)pictureBox2.Image.Clone();

            hue_change = Convert.ToDouble(textBox1.Text);
            saturation_change = Convert.ToDouble(textBox2.Text);
            value_change = Convert.ToDouble(textBox3.Text);

            Bitmap bmp3 = pictureBox3.Image as Bitmap;
            Rectangle rect3 = new Rectangle(0, 0, bmp3.Width, bmp3.Height);
            System.Drawing.Imaging.BitmapData bmpData3 =
                bmp3.LockBits(rect3, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp3.PixelFormat);
            IntPtr ptr3 = bmpData3.Scan0;
            int bytes3 = Math.Abs(bmpData3.Stride) * bmp3.Height;
            System.Runtime.InteropServices.Marshal.Copy(ptr3, rgbValues3, 0, bytes3);
            ColorToHSV(rgbValues3, hsvValues3, bytes3);
            bmp3.UnlockBits(bmpData3);

            int i = 0;
            for (int counter = 0; counter < rgbValues1.Length; counter += 3)
            {
                bmp3.SetPixel(i % bmp3.Width, i / bmp3.Width,
                    ColorFromHSV(hsvValues3[counter + 2], hsvValues3[counter + 1], hsvValues3[counter]));
                i++;

            }

            pictureBox3.Refresh();
            Bitmap bmpSave = (Bitmap)pictureBox3.Image;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "test3img";
            sfd.Filter = "Image files (*.bmp)|*.bmp|All files (*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)

                bmpSave.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                
        }



        bool is_equal(double x, double y)
        {
            return Math.Abs(x - y) < 0.0001;
        }

        public void ColorToHSV(byte[] rgbValues, double[] hsvValues, int bytes)
        {
            for (int counter = 0; counter < bytes; counter += 3)
            {
               /* Console.WriteLine(rgbValues[counter + 2]);
                Console.WriteLine(rgbValues[counter + 1]);
                Console.WriteLine(rgbValues[counter]);*/
                int red = rgbValues[counter + 2];
                int green = rgbValues[counter + 1];
                int blue = rgbValues[counter];
                double max = Math.Max(red, Math.Max(green, blue));
                double min = Math.Min(red, Math.Min(green, blue));
                //  Console.WriteLine();

                /*double hue = 0;
                if (is_equal(max, min))
                    hue = 0;
                else if (is_equal(max, red))
                {
                    if (green < blue)
                        hue = 60d * (green - blue) / (max - min) + 360;
                    else
                        hue = 60d * (green - blue) / (max - min);
                }
                else if (is_equal(max, green))
                {
                    hue = 60d * (blue - red) / (max - min) + 120;
                }
                else if (is_equal(max, blue))
                {
                    hue = 60d * (red - green) / (max - min) + 240;
                }

                */
                Color color = Color.FromArgb(red, green, blue);
                double hue = color.GetHue();
                double saturation = (max == 0) ? 0 : 1d - (1d * min / max);
                double value = max / 255d;

                hue = (hue + hue_change);
                saturation = (saturation + saturation_change);
                if (saturation > 1)
                    saturation = 1;
                if (saturation < 0)
                    saturation = 0;
                value = (value + value_change);
                if (value > 1)
                    value = 1;
                if (value < 0)
                    value = 0;

                hsvValues[counter + 2] = hue;
                hsvValues[counter + 1] = saturation;
                hsvValues[counter] = value;

               /* Console.WriteLine(hsvValues[counter + 2]);
                Console.WriteLine(hsvValues[counter + 1]);
                Console.WriteLine(hsvValues[counter]);
                Console.WriteLine();
                Console.WriteLine();*/
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int vmin = Convert.ToInt32(value * (1 - saturation));
            int vdec = Convert.ToInt32(value * (1 - f * saturation));
            int vinc = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, vinc, vmin);
            else if (hi == 1)
                return Color.FromArgb(255, vdec, v, vmin);
            else if (hi == 2)
                return Color.FromArgb(255, vmin, v, vinc);
            else if (hi == 3)
                return Color.FromArgb(255, vmin, vdec, v);
            else if (hi == 4)
                return Color.FromArgb(255, vinc, vmin, v);
            else
                return Color.FromArgb(255, v, vmin, vdec);
        }

        public void textbox_SetText()
        {
            if (textBox1.Text.Length == 0)
            {
                this.textBox1.Text = "0";
                this.textBox2.Text = "0";
                this.textBox3.Text = "0";
            }
        }



        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string pict = openFileDialog1.FileName;
            pictureBox1.Image = Bitmap.FromFile(pict);
            pictureBox2.Image = Bitmap.FromFile(pict);
            pictureBox3.Image = Bitmap.FromFile(pict);
            Bitmap bmp1 = pictureBox1.Image as Bitmap;
            Bitmap bmp2 = pictureBox2.Image as Bitmap;
            Bitmap bmp3 = pictureBox3.Image as Bitmap;

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
            hsvValues2 = new double[bytes2];
            hsvValues3 = new double[bytes3];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr1, rgbValues1, 0, bytes1);
            System.Runtime.InteropServices.Marshal.Copy(ptr2, rgbValues2, 0, bytes2);
            System.Runtime.InteropServices.Marshal.Copy(ptr3, rgbValues3, 0, bytes3);

            ColorToHSV(rgbValues2, hsvValues2, bytes1);
            ColorToHSV(rgbValues3, hsvValues3, bytes1);


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
                    Color.FromArgb((byte)(rgbValues1[counter + 2]), (byte)(rgbValues1[counter + 1]), (byte)(rgbValues1[counter])));

                bmp2.SetPixel(i % bmp2.Width, i / bmp2.Width,
                    ColorFromHSV(hsvValues2[counter + 2], hsvValues2[counter + 1], hsvValues2[counter]));


                bmp3.SetPixel(i % bmp3.Width, i / bmp3.Width,
                    ColorFromHSV((hsvValues3[counter + 2]), (hsvValues3[counter + 1]), (hsvValues3[counter])));
                i++;

            }
            pictureBox1.Refresh();
            pictureBox2.Refresh();
            pictureBox3.Refresh();

        }


        public Form1()
        {
            InitializeComponent();

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;

            textbox_SetText();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }



    }
}

