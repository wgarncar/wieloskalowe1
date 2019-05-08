using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace projekt2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            DrawArea = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = DrawArea;
        }

        Bitmap DrawArea;
        Graphics g;
        Pen pen;
        Brush brush;

        int countH = 30;
        int countW = 30;
        int cellSize = 10;

        bool[,] lifeTab;
        bool[,] lifeTabNew;

        bool stop = false;


        private void Form1_Load(object sender, EventArgs e)
        {
            g = Graphics.FromImage(DrawArea);
            pen = new Pen(Brushes.Black);
            brush = new SolidBrush(Color.Black);

            button1.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;

            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 1000;

            textBox1.Text = "30";
            textBox2.Text = "30";
            textBox3.Text = "1000";


        }

        //start
        private void button1_Click(object sender, EventArgs e)
        {

            draw();
            stop = false;

            button3.Enabled = true;
            button5.Enabled = false;

            timer1.Start();

        }

        private void calculate()
        {
            
            for(int i = 0; i < countH; i++)
            {
                int up = i - 1;
                if (up < 0) up = countH - 1;
                int down = i + 1;
                if (down > countH - 1) down = 0;

                for (int j = 0; j < countW; j++)
                {
                    int sum = 0;
                    int left = j - 1;
                    if (left < 0) left = countW - 1;
                    int right = j + 1;
                    if (right > countW-1) right = 0;

                    sum += lifeTab[i, left] ? 1 : 0;
                    sum += lifeTab[i, right] ? 1 : 0;
                    sum += lifeTab[up, left] ? 1 : 0;
                    sum += lifeTab[up, j] ? 1 : 0;
                    sum += lifeTab[up, right] ? 1 : 0;
                    sum += lifeTab[down, left] ? 1 : 0;
                    sum += lifeTab[down, j] ? 1 : 0;
                    sum += lifeTab[down, right] ? 1 : 0;

                    if (lifeTab[i, j] == false && sum == 3) lifeTabNew[i, j] = true;
                    else if (lifeTab[i, j] == true && sum > 3) lifeTabNew[i, j] = false;
                    else if (lifeTab[i, j] == true && sum <= 3 && sum >= 2) lifeTabNew[i, j] = true;
                    else if (lifeTab[i, j] == true && sum < 2) lifeTabNew[i, j] = false;
                    else lifeTabNew[i, j] = false;

                }
            }

            Array.Copy(lifeTabNew, 0, lifeTab, 0, countH * countW);
        }

        private void draw()
        {
            g.Clear(Color.White);

            for (int y = 0; y <= countW; ++y)
            {
                g.DrawLine(pen, 0, y * cellSize, countH * cellSize, y * cellSize);
            }

            for (int x = 0; x <= countH; ++x)
            {
                g.DrawLine(pen, x * cellSize, 0, x * cellSize, countW * cellSize);
            }

            for (int i = 0; i < countH; i++)
            {
                for(int j = 0; j < countW; j++)
                {
                    if(lifeTab[i, j])
                    {
                        g.FillRectangle(brush, i* cellSize, j* cellSize, cellSize, cellSize);
                        g.Flush();
                    }
                }
            }
            pictureBox1.Image = DrawArea;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            calculate();
            draw();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (stop == true)
            {
                if (e is MouseEventArgs)
                {
                    var me = e as MouseEventArgs;
                    Console.WriteLine("x: " + me.X + " y: " + me.Y);

                    lifeTab[me.X / cellSize, me.Y / cellSize] = !lifeTab[me.X / cellSize, me.Y / cellSize];
                    draw();
                }
            }
        }

        //set size
        private void button2_Click(object sender, EventArgs e)
        {
            if (!Int32.TryParse(textBox1.Text, out countH) || countH < 5)
            {
                countH = 30;
            }
            if (!Int32.TryParse(textBox2.Text, out countW) || countW < 5)
            {
                countW = 30;
            }

            lifeTab = new bool[countH, countW];
            lifeTabNew = new bool[countH, countW];

            for (int i = 0; i < countH; i++)
            {
                for (int j = 0; j < countW; j++)
                {
                    lifeTab[i, j] = false;
                    lifeTabNew[i, j] = false;
                }
            }

            if (countH >= countW) cellSize = pictureBox1.Height / countH;
            else cellSize = pictureBox1.Width / countW;

            for (int y = 0; y <= countW; ++y)
            {
                g.DrawLine(pen, 0, y * cellSize, countH * cellSize, y * cellSize);
            }

            for (int x = 0; x <= countH; ++x)
            {
                g.DrawLine(pen, x * cellSize, 0, x * cellSize, countW * cellSize);
            }

            pictureBox1.Image = DrawArea;

            button2.Enabled = false;
            button1.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;

            stop = true;

        }

        //set state
        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.GetSelected(0) == true)
            {
                lifeTab[countH / 2 - 1, countW / 2] = true;
                lifeTab[countH / 2 + 2, countW / 2] = true;
                lifeTab[countH / 2, countW / 2 - 1] = true;
                lifeTab[countH / 2, countW / 2 + 1] = true;
                lifeTab[countH / 2 + 1, countW / 2 - 1] = true;
                lifeTab[countH / 2 + 1, countW / 2 + 1] = true;
            }
            else if (listBox1.GetSelected(1) == true)
            {
                lifeTab[countH / 2, countW / 2] = true;
                lifeTab[countH / 2 - 1, countW / 2] = true;
                lifeTab[countH / 2, countW / 2 - 1] = true;
                lifeTab[countH / 2 + 1, countW / 2 - 1] = true;
                lifeTab[countH / 2 + 1, countW / 2 + 1] = true;
            }
            else if (listBox1.GetSelected(2) == true)
            {
                lifeTab[countH / 2, countW / 2] = true;
                lifeTab[countH / 2, countW / 2 + 1] = true;
                lifeTab[countH / 2, countW / 2 - 1] = true;
            }
            else if (listBox1.GetSelected(3) == true)
            {
                Random random = new Random();
                int num = random.Next(countH * countW);

                for (int i = 0; i < num; i++)
                {
                    lifeTab[random.Next(countH), random.Next(countW)] = true;
                }

            }

            draw();
        }

        //stop
        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            stop = true;

            button5.Enabled = true;
            button3.Enabled = false;
        }

        //reset
        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Stop();

            g.Clear(Color.White);
            pictureBox1.Image = null;

            button1.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;

            button2.Enabled = true;
        }

        //set time
        private void button6_Click(object sender, EventArgs e)
        {
            int tmp = 0;
            if (!Int32.TryParse(textBox3.Text, out tmp))
            {
                tmp = 1000;
            }

            timer1.Interval = tmp;

        }
    }
}
