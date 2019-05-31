using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wiel3
{
    public partial class Form1 : Form
    {
        private Graphics g;
        private Pen pen;
        private SolidBrush brush;
        private SolidBrush brush2;

        private Random random = new Random();

        private int[,] tab;
        private int[,] newtab;
        private int nodesPerWidth;
        private int nodesPerHeight;



        int amount;
        int col;
        int row;
        int radius;

        Boolean period = true;

        private List<Grain> grains;
        
        public int PIXEL_SIZE;
        public readonly int DEFAULT_SIZE = 9;
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            PIXEL_SIZE = DEFAULT_SIZE;
            g = pictureBox1.CreateGraphics();
            pen = new Pen(Color.Black);            
            brush = new SolidBrush(Color.Black);
            brush2 = new SolidBrush(Color.White);

            grains = new List<Grain>();

            

            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 1000;

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;

            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            

            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;

            button7.Enabled = false;

            timer1.Start();
        }

        class Grain
        {
            private int index;
            private SolidBrush brushh;
            private int x;
            private int y;

            public Grain(int index, int[] color, int x, int y)
            {
                this.index = index;
                this.brushh = new SolidBrush(Color.FromArgb(color[0], color[1], color[2]));
                this.x = x;
                this.y = y;
            }

            public SolidBrush getBrush()
            {
                return this.brushh;
            }

            public int getIndex()
            {
                return index;
            }

            public int getX()
            {
                return x;
            }

            public int getY()
            {
                return y;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            calculate();

            draw();
        }

        private void calculate()
        {
            Array.Copy(tab, 0, newtab, 0, nodesPerWidth * nodesPerHeight);

            int chosen = listBox1.SelectedIndex;

            if (chosen == 0)
            {
                vonNeumann();
            }
            else if(chosen == 1)
            {
                moore();
            }
            else if(chosen >= 2 && chosen <=6)
            {
                pentagonal();
            }
            else if (chosen >= 7 && chosen <= 9)
            {
                hexagonal();
            }

            Array.Copy(newtab, 0, tab, 0, nodesPerWidth * nodesPerHeight);
        }

        private void vonNeumann()
        {
            if (checkBox1.Checked) { period = true; } else { period = false; }
            for (int i = 0; i < nodesPerWidth; i++)
            {
                for (int j = 0; j < nodesPerHeight; j++)
                {
                    if (tab[i, j] == 0)
                    {
                        int[] neighbour = new int[grains.Count];
                        for (int k = 0; k < grains.Count; k++)
                        {
                            neighbour[k] = 0;
                        }
                        int left = i - 1;
                        int right = i + 1;
                        int up = j - 1;
                        int down = j + 1;

                        if (period)
                        {
                            if (left < 0) left = nodesPerWidth - 1;
                            if (right >= nodesPerWidth) right = 0;
                            if (up < 0) up = nodesPerHeight - 1;
                            if (down >= nodesPerHeight) down = 0;
                        }
                        else
                        {
                            if (left < 0) left = 0;
                            if (right >= nodesPerWidth) right = nodesPerWidth - 1;
                            if (up < 0) up = 0;
                            if (down >= nodesPerHeight) down = nodesPerHeight - 1;
                        }

                        if (tab[left, j] != 0) neighbour[tab[left, j] - 1]++;
                        if (tab[right, j] != 0) neighbour[tab[right, j] - 1]++;
                        if (tab[i, up] != 0) neighbour[tab[i, up] - 1]++;
                        if (tab[i, down] != 0) neighbour[tab[i, down] - 1]++;

                        int max = 0;
                        int maxid = 0;
                        for (int z = 0; z < grains.Count; ++z)
                            if (neighbour[z] > max)
                            {
                                max = neighbour[z];
                                maxid = z;
                            }

                        if (max != 0)
                        {
                            newtab[i, j] = grains[maxid].getIndex();
                        }

                    }
                }
            }
        }

        private void moore()
        {
            if (checkBox1.Checked) { period = true; } else { period = false; }
            for (int i = 0; i < nodesPerWidth; i++)
            {
                for (int j = 0; j < nodesPerHeight; j++)
                {
                    if (tab[i, j] == 0)
                    {
                        int[] neighbour = new int[grains.Count];
                        for (int k = 0; k < grains.Count; k++)
                        {
                            neighbour[k] = 0;
                        }
                        int left = i - 1;
                        int right = i + 1;
                        int up = j - 1;
                        int down = j + 1;

                        if (period)
                        {
                            if (left < 0) left = nodesPerWidth - 1;
                            if (right >= nodesPerWidth) right = 0;
                            if (up < 0) up = nodesPerHeight - 1;
                            if (down >= nodesPerHeight) down = 0;
                        }
                        else
                        {
                            if (left < 0) left = 0;
                            if (right >= nodesPerWidth) right = nodesPerWidth - 1;
                            if (up < 0) up = 0;
                            if (down >= nodesPerHeight) down = nodesPerHeight - 1;
                        }

                        if (tab[left, j] != 0) neighbour[tab[left, j] - 1]++;
                        if (tab[right, j] != 0) neighbour[tab[right, j] - 1]++;
                        if (tab[i, up] != 0) neighbour[tab[i, up] - 1]++;
                        if (tab[i, down] != 0) neighbour[tab[i, down] - 1]++;

                        if (tab[left, down] != 0) neighbour[tab[left, down] - 1]++;
                        if (tab[right, down] != 0) neighbour[tab[right, down] - 1]++;
                        if (tab[right, up] != 0) neighbour[tab[right, up] - 1]++;
                        if (tab[left, up] != 0) neighbour[tab[left, up] - 1]++;

                        int max = 0;
                        int maxid = 0;
                        for (int z = 0; z < grains.Count; ++z)
                            if (neighbour[z] > max)
                            {
                                max = neighbour[z];
                                maxid = z;
                            }

                        if (max != 0)
                        {
                            newtab[i, j] = grains[maxid].getIndex();
                        }

                    }
                }
            }
        }

        public void pentagonal()
        {
            
            if (checkBox1.Checked) { period = true; } else { period = false; }
            for (int i = 0; i < nodesPerWidth; i++)
            {
                for (int j = 0; j < nodesPerHeight; j++)
                {
                    if (tab[i, j] == 0)
                    {

                        int[] neighbour = new int[grains.Count];
                        for (int k = 0; k < grains.Count; k++)
                        {
                            neighbour[k] = 0;
                        }
                        int left = i - 1;
                        int right = i + 1;
                        int up = j - 1;
                        int down = j + 1;

                        if (period)
                        {
                            if (left < 0) left = nodesPerWidth - 1;
                            if (right >= nodesPerWidth) right = 0;
                            if (up < 0) up = nodesPerHeight - 1;
                            if (down >= nodesPerHeight) down = 0;
                        }
                        else
                        {
                            if (left < 0) left = 0;
                            if (right >= nodesPerWidth) right = nodesPerWidth - 1;
                            if (up < 0) up = 0;
                            if (down >= nodesPerHeight) down = nodesPerHeight - 1;
                        }
                        int chosen = listBox1.SelectedIndex;

                        if (chosen == 2)
                        {
                            chosen = random.Next(3, 7);
                        }
                        if (chosen == 3)
                        {
                            if (tab[left, j] != 0) neighbour[tab[left, j] - 1]++;
                            if (tab[right, j] != 0) neighbour[tab[right, j] - 1]++;
                            if (tab[i, up] != 0) neighbour[tab[i, up] - 1]++;

                            if (tab[left, up] != 0) neighbour[tab[left, up] - 1]++;
                            if (tab[right, up] != 0) neighbour[tab[right, up] - 1]++;
                        }
                        if (chosen == 4)
                        {
                            if (tab[left, j] != 0) neighbour[tab[left, j] - 1]++;
                            if (tab[right, j] != 0) neighbour[tab[right, j] - 1]++;
                            if (tab[i, down] != 0) neighbour[tab[i, down] - 1]++;

                            if (tab[left, down] != 0) neighbour[tab[left, down] - 1]++;
                            if (tab[right, down] != 0) neighbour[tab[right, down] - 1]++;
                        }
                        if (chosen == 5)
                        {
                            if (tab[left, j] != 0) neighbour[tab[left, j] - 1]++;
                            if (tab[i, up] != 0) neighbour[tab[i, up] - 1]++;
                            if (tab[i, down] != 0) neighbour[tab[i, down] - 1]++;

                            if (tab[left, down] != 0) neighbour[tab[left, down] - 1]++;
                            if (tab[left, up] != 0) neighbour[tab[left, up] - 1]++;
                        }
                        if (chosen == 6)
                        {
                            if (tab[right, j] != 0) neighbour[tab[right, j] - 1]++;
                            if (tab[i, up] != 0) neighbour[tab[i, up] - 1]++;
                            if (tab[i, down] != 0) neighbour[tab[i, down] - 1]++;

                            if (tab[right, down] != 0) neighbour[tab[right, down] - 1]++;
                            if (tab[right, up] != 0) neighbour[tab[right, up] - 1]++;
                        }



                        int max = 0;
                        int maxid = 0;
                        for (int z = 0; z < grains.Count; ++z)
                            if (neighbour[z] > max)
                            {
                                max = neighbour[z];
                                maxid = z;
                            }

                        if (max != 0)
                        {
                            newtab[i, j] = grains[maxid].getIndex();
                        }

                    }
                }
            }
        }

        public void hexagonal()
        {

            if (checkBox1.Checked) { period = true; } else { period = false; }
            for (int i = 0; i < nodesPerWidth; i++)
            {
                for (int j = 0; j < nodesPerHeight; j++)
                {
                    if (tab[i, j] == 0)
                    {

                        int[] neighbour = new int[grains.Count];
                        for (int k = 0; k < grains.Count; k++)
                        {
                            neighbour[k] = 0;
                        }
                        int left = i - 1;
                        int right = i + 1;
                        int up = j - 1;
                        int down = j + 1;

                        if (period)
                        {
                            if (left < 0) left = nodesPerWidth - 1;
                            if (right >= nodesPerWidth) right = 0;
                            if (up < 0) up = nodesPerHeight - 1;
                            if (down >= nodesPerHeight) down = 0;
                        }
                        else
                        {
                            if (left < 0) left = 0;
                            if (right >= nodesPerWidth) right = nodesPerWidth - 1;
                            if (up < 0) up = 0;
                            if (down >= nodesPerHeight) down = nodesPerHeight - 1;
                        }
                        int chosen = listBox1.SelectedIndex;

                        if (chosen == 7)
                        {
                            chosen = random.Next(8, 10);
                        }
                        if (chosen == 8)
                        {
                            if (tab[left, j] != 0) neighbour[tab[left, j] - 1]++;
                            if (tab[right, j] != 0) neighbour[tab[right, j] - 1]++;
                            if (tab[i, up] != 0) neighbour[tab[i, up] - 1]++;
                            if (tab[i, down] != 0) neighbour[tab[i, down] - 1]++;

                            if (tab[left, down] != 0) neighbour[tab[left, down] - 1]++;
                            if (tab[right, up] != 0) neighbour[tab[right, up] - 1]++;
                        }
                        if (chosen == 9)
                        {
                            if (tab[left, j] != 0) neighbour[tab[left, j] - 1]++;
                            if (tab[right, j] != 0) neighbour[tab[right, j] - 1]++;
                            if (tab[i, up] != 0) neighbour[tab[i, up] - 1]++;
                            if (tab[i, down] != 0) neighbour[tab[i, down] - 1]++;

                            if (tab[left, up] != 0) neighbour[tab[left, up] - 1]++;
                            if (tab[right, down] != 0) neighbour[tab[right, down] - 1]++;
                        }
                        

                        int max = 0;
                        int maxid = 0;
                        for (int z = 0; z < grains.Count; ++z)
                            if (neighbour[z] > max)
                            {
                                max = neighbour[z];
                                maxid = z;
                            }

                        if (max != 0)
                        {
                            newtab[i, j] = grains[maxid].getIndex();
                        }

                    }
                }
            }
        }
        private void draw()
        {
            g.Clear(Color.White);

            for (int i = 0; i < nodesPerWidth; i++)
            {
                for (int j = 0; j < nodesPerHeight; j++)
                {
                    int val = tab[i, j];
                    if (val > 0)
                    {
                        g.FillRectangle(grains[val - 1].getBrush(), (i * PIXEL_SIZE), (j * PIXEL_SIZE), PIXEL_SIZE, PIXEL_SIZE);
                        g.Flush();
                    }
                }
            }

            //System.Threading.Thread.Sleep(5000);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!Int32.TryParse(textBox1.Text, out amount))
            {
                amount = 1;
            }
            int breakPoint = 0;
            for (int i = 0; i < amount; i++)
            {
                breakPoint = 0;
                int x, y;
                do
                {
                    x = random.Next(0, nodesPerWidth);
                    y = random.Next(0, nodesPerHeight);
                    Console.WriteLine(x + " " + y);
                    breakPoint++;
                    if (breakPoint > 2000)
                        break;
                } while (tab[x, y] != 0);

                if (breakPoint < 2000)
                    createNewGrain(x, y);
            }

            draw();
        }

        private void createNewGrain(int x, int y)
        {
            if (tab[x, y] == 0)
            {
                int r = random.Next(10, 245);
                int g = random.Next(10, 245);
                int b = random.Next(10, 245);
                int[] color = { r, g, b };

                Grain grain = new Grain(grains.Count + 1, color, x, y);

                grains.Add(grain);
                tab[x, y] = grain.getIndex();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!Int32.TryParse(textBox2.Text, out col) || col < 1 || col > nodesPerWidth)
            {
                col = 1;
            }
            if (!Int32.TryParse(textBox3.Text, out row) || row < 1 || row > nodesPerHeight)
            {
                row = 1;
            }

            int maxInWidth = nodesPerWidth % 2 != 0 ? nodesPerWidth-- : nodesPerWidth;
            int mexInHeight = nodesPerHeight % 2 != 0 ? nodesPerHeight-- : nodesPerHeight;

            int colDelay = (maxInWidth) / (col);
            int rowDelay = mexInHeight / (row);

            for (int i = 0; i < col; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    createNewGrain(i * colDelay + colDelay / 2, j * rowDelay + rowDelay / 2);
                }
            }
            draw();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < nodesPerWidth; i++)
            {
                for (int j = 0; j < nodesPerHeight; j++)
                {
                    tab[i, j] = 0;
                    newtab[i, j] = 0;
                }
            }

            draw();
            button7.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;

            timer1.Stop();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (e is MouseEventArgs)
            {
                var me = e as MouseEventArgs;
                //Console.WriteLine("x: " + me.X + " y: " + me.Y);
                createNewGrain(me.X / PIXEL_SIZE, me.Y / PIXEL_SIZE);

                draw();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!Int32.TryParse(textBox1.Text, out amount))
            {
                amount = 1;
            }
            if (!Int32.TryParse(textBox4.Text, out radius))
            {
                radius = 2;
            }
            if(checkBox1.Checked) { period = true; } else { period = false; }
            int breakPoint = 0;
            int udane = 0;
            for (int i = 0; i < amount; i++)
            {
                breakPoint = 0;
                int x, y;
                bool go = true;
                do
                {
                    x = random.Next(0, nodesPerWidth);
                    y = random.Next(0, nodesPerHeight);
                    if(tab[x,y] == 0)
                    {
                        bool empty = true;
                        for(int k = -radius; k <= radius; k++)
                        {

                            for(int z = -radius; z <= radius; z++)
                            {
                                if (period)
                                {
                                    int one = x + k;
                                    if (one < 0) { one = nodesPerWidth + x + k; }
                                    if (one >= nodesPerWidth) { one = one - nodesPerWidth; }
                                    int two = y + z;
                                    if (two < 0) { two = nodesPerHeight + y + z; }
                                    if (two >= nodesPerHeight) { two = two - nodesPerHeight; }

                                    if (tab[one, two] != 0)
                                    {
                                        empty = false;
                                    }
                                }
                                else
                                {
                                    if ((x + k) >= 0 && (x + k) < nodesPerWidth && (y + z) >= 0 && (y + z) < nodesPerHeight && tab[x + k, y + z] != 0)
                                    {
                                        empty = false;
                                    }
                                }
                            }
                        }
                        if (empty)
                        {
                            go = false;
                            udane++;
                        }
                    }
                    breakPoint++;
                    if (breakPoint > 2000)
                        break;
                } while (go);

                if (breakPoint < 2000)
                    createNewGrain(x, y);
            }

            draw();
            if(udane < amount)
            {
                System.Windows.Forms.MessageBox.Show("Udało się tylko: " + udane + " razy.");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (!Int32.TryParse(textBox5.Text, out nodesPerWidth) || nodesPerWidth < 5)
            {
                nodesPerWidth = pictureBox1.Width / PIXEL_SIZE;
            }
            if (!Int32.TryParse(textBox6.Text, out nodesPerHeight) || nodesPerHeight < 5)
            {
                nodesPerHeight = pictureBox1.Height / PIXEL_SIZE;
            }

            if(nodesPerWidth > nodesPerHeight)
            {
                PIXEL_SIZE = pictureBox1.Width / nodesPerWidth;
            } else
            {
                PIXEL_SIZE = pictureBox1.Height / nodesPerHeight;
            }

            
            tab = new int[nodesPerWidth, nodesPerHeight];
            newtab = new int[nodesPerWidth, nodesPerHeight];

            for (int i = 0; i < nodesPerWidth; i++)
            {
                for (int j = 0; j < nodesPerHeight; j++)
                {
                    tab[i, j] = 0;
                    newtab[i, j] = 0;
                }
            }

            button7.Enabled = false;

            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;

            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
        }
    }
}
