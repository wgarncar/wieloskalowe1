using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace wiel3
{
    public partial class Form1 : Form
    {
        private Graphics g;
        private Pen pen;
        private SolidBrush brush;
        private SolidBrush brush2;
        private SolidBrush brushx = new SolidBrush(Color.FromArgb(255, 0, 0));
        private SolidBrush brushEnergy1 = new SolidBrush(Color.FromArgb(0, 238, 0));
        private SolidBrush brushEnergy0= new SolidBrush(Color.FromArgb(0, 186, 207));
        private SolidBrush brushRed= new SolidBrush(Color.FromArgb(255, 0, 0));

        private Random random = new Random();

        Form2 secondForm = new Form2();

        private int[,] tab;
        private int[,] newtab;
        private Gravity[,] gravityList;
        private bool[,] gravityList2;
        private bool[,] gravityList3;
        private int nodesPerWidth;
        private int nodesPerHeight;


        bool full = false;
        bool isEnergy = false;
        bool showEnergy = false;
        int jgb = 1;
        double kt= 0.1;

        int amount;
        int col;
        int row;
        int radius;

        double A = 8.6711E13;
        double B = 9.41;
        double t = 0;
        double ro = 0;
        double roprev = 0;

        bool dislocate = false;
        double disMax = 4.21584E+12;
        double disMaxPerCell;
        int disCount = 0;
        double maxt = 0.2;
        public int maxiter = 201;
        double sumDis;
        int red = 255;

        bool stop = false;

        string path = "C:/Users/wgarncar/source/repos/wiel3/wiel3/Dane.txt";
        StreamWriter sw = new StreamWriter("C:/Users/wgarncar/source/repos/wiel3/wiel3/Dane.txt");

        

        Boolean period = true;

        private List<Grain> grains;
        
        public int PIXEL_SIZE = 1;
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
            timer1.Interval = 500;

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;

            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;

            checkBox2.Enabled = false;
            checkBox3.Enabled = false;
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
            if (!stop)
            {
                isEnergy = (checkBox2.Checked) ? true : false;
                showEnergy = (checkBox3.Checked) ? true : false;


                if (dislocate)
                {
                    t = (double)disCount / (double)1000;
                    disCount++;
                    Console.WriteLine(disCount);

                }
                if (disCount > 1)
                {
                    for (int i = 0; i < nodesPerWidth; i++)
                    {
                        for (int j = 0; j < nodesPerHeight; j++)
                        {
                            gravityList2[i, j] = gravityList[i, j].getState();
                        }
                    }
                }
                if (disCount > 2)
                {
                    for (int i = 0; i < nodesPerWidth; i++)
                    {
                        for (int j = 0; j < nodesPerHeight; j++)
                        {
                            gravityList3[i, j] = gravityList2[i, j];
                        }
                    }
                }

                if (!Double.TryParse(textBox7.Text, out kt) || kt > 5 || kt < 0.1)
                {
                    kt = 0.1;
                }

                calculate();

                if (full) checkBox2.Enabled = true;
                if (full) checkBox3.Enabled = true;

                draw();

                if (disCount == maxiter)
                {
                    stop = true;
                    sw.WriteLine("***");
                    sw.Close();
                    dislocate = false;
                    System.Windows.Forms.MessageBox.Show("Program skończył liczyć rekrystalizacje.");
                    
                }
            }
        }

        private void stopTimer()
        {
            timer1.Enabled = false;
        }

        private void calculate()
        {
            Array.Copy(tab, 0, newtab, 0, nodesPerWidth * nodesPerHeight);

            if (dislocate)
            {
                sw.Write(t + " ");
                sumDis = 0;
                double dis = dislication(t);
                double tmpdis = dis;
                ro = dis - roprev;
                sw.Write(dis + " " + ro + " ");
                ro = rozrzuc30(ro);
                rozrzuc_rowno(ro, 0.01);
                sw.Write(sumDis + " ");


                for (int i = 0; i < nodesPerWidth; i++)
                {
                    for (int j = 0; j < nodesPerHeight; j++)
                    {
                        

                        if (gravityList[i, j].getState() == false)
                        {
                            int chosen = listBox1.SelectedIndex;
                            List<Location> neighbour;

                            if (chosen == 0)
                            {
                                neighbour = vonNeumannDis(i, j);
                            }
                            else if (chosen >= 2 && chosen <= 6)
                            {
                                neighbour = pentagonalDis(i, j);
                            }
                            else if (chosen >= 7 && chosen <= 9)
                            {
                                neighbour = hexagonalDis(i, j);
                            }
                            else
                            {
                                neighbour = mooreDis(i, j);
                            }
                            if (gravityList[i, j].getDislocation() >= disMaxPerCell)
                            {
                                int inne = 0;
                                for(int k = 0; k < neighbour.Count; k++)
                                {
                                    if (tab[i, j] != tab[neighbour[k].getX(), neighbour[k].getY()]) inne++;
                                }
                                if (inne > 0)
                                {
                                    gravityList[i, j].setState(true);
                                    gravityList[i, j].setB(random.Next(0, 255));
                                    gravityList[i, j].setDislocation(0);
                                }
                            }

                            for (int k = 0; k < neighbour.Count; k++) {
                                if(gravityList2[neighbour[k].getX(), neighbour[k].getY()] == true && gravityList3[neighbour[k].getX(), neighbour[k].getY()] == false)
                                {
                                    bool isBiggest = true;
                                    for(int z = 0; z < neighbour.Count; z++)
                                    {
                                        if (z != k)
                                        {
                                            if(gravityList[neighbour[z].getX(), neighbour[z].getY()].getDislocation() > gravityList[i, j].getDislocation())
                                            {
                                                isBiggest = false;
                                                break;
                                            }
                                        }
                                    }
                                    if (isBiggest)
                                    {
                                        gravityList[i, j].setState(true);
                                        gravityList[i, j].setB(random.Next(0, 255));
                                        gravityList[i, j].setDislocation(0);
                                    }
                                }
                            }
                        }

                    }
                }
                zapis_do_pliku();
                roprev = dis;

            }
            else
            {

                int chosen = listBox1.SelectedIndex;

                if (chosen == 0)
                {
                    vonNeumann();
                }
                else if (chosen == 1)
                {
                    moore();
                }
                else if (chosen >= 2 && chosen <= 6)
                {
                    pentagonal();
                }
                else if (chosen >= 7 && chosen <= 9)
                {
                    hexagonal();
                }
                else if (chosen == 10)
                {
                    radiusGrowth();
                }

                Array.Copy(newtab, 0, tab, 0, nodesPerWidth * nodesPerHeight);

                if (full == false)
                {
                    full = true;

                    for (int i = 0; i < nodesPerWidth; i++)
                    {
                        for (int j = 0; j < nodesPerHeight; j++)
                        {
                            if (tab[i, j] == 0)
                            {
                                full = false;
                                break;
                            }
                        }
                    }
                }

            }

        }

        private void vonNeumann()
        {
            if (checkBox1.Checked) { period = true; } else { period = false; }
            for (int i = 0; i < nodesPerWidth; i++)
            {
                for (int j = 0; j < nodesPerHeight; j++)
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


                    if (isEnergy)
                    {
                        int sum = 0;
                        int diferent = 0;
                        for (int z = 0; z < grains.Count; ++z)
                        {
                            if (tab[i, j] != grains[z].getIndex())
                            {
                                diferent++;
                                int tmp = neighbour[z];
                                while (tmp > 0)
                                {
                                    sum++;
                                    tmp--;
                                }
                            }
                        }
                        int en = jgb * sum;
                        if (diferent > 0)
                        {
                            int next = 0;
                            bool a = true;
                            while (a)
                            {
                                next = random.Next(0, grains.Count);
                                if (neighbour[next] != 0) a = false;
                            }

                            int sum2 = 0;

                            for (int z = 0; z < grains.Count; ++z)
                            {
                                if (grains[next].getIndex() != grains[z].getIndex())
                                {

                                    int tmp = neighbour[z];
                                    while (tmp > 0)
                                    {
                                        sum2++;
                                        tmp--;
                                    }
                                }
                            }
                            int en2 = jgb * sum2;

                            int deltaEn = en2 - en;
                            if (deltaEn <= 0)
                            {
                                gravityList[i, j].setEnergy(en2);
                                newtab[i, j] = grains[next].getIndex();
                            }
                            else
                            {
                                double pos = Math.Exp(-deltaEn / kt);
                                double rand = random.NextDouble();

                                if (rand <= pos)
                                {
                                    gravityList[i, j].setEnergy(en2);
                                    newtab[i, j] = grains[next].getIndex();
                                }
                            }
                        }
                    }
                    else if (tab[i, j] == 0)
                    {

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

                    if (isEnergy)
                    {
                        int sum = 0;
                        int diferent = 0;
                        for (int z = 0; z < grains.Count; ++z)
                        {
                            if (tab[i, j] != grains[z].getIndex())
                            {
                                diferent++;
                                int tmp = neighbour[z];
                                while (tmp > 0)
                                {
                                    sum++;
                                    tmp--;
                                }
                            }
                        }
                        int en = jgb * sum;
                        if (diferent > 0)
                        {
                            int next = 0;
                            bool a = true;
                            while (a)
                            {
                                next = random.Next(0, grains.Count);
                                if (neighbour[next] != 0) a = false;
                            }

                            int sum2 = 0;

                            for (int z = 0; z < grains.Count; ++z)
                            {
                                if (grains[next].getIndex() != grains[z].getIndex())
                                {

                                    int tmp = neighbour[z];
                                    while (tmp > 0)
                                    {
                                        sum2++;
                                        tmp--;
                                    }
                                }
                            }
                            int en2 = jgb * sum2;

                            int deltaEn = en2 - en;
                            if (deltaEn <= 0)
                            {
                                gravityList[i, j].setEnergy(en2);
                                newtab[i, j] = grains[next].getIndex();
                            }
                            else
                            {
                                double pos = Math.Exp(-deltaEn / kt);
                                double rand = random.NextDouble();

                                if (rand <= pos)
                                {
                                    gravityList[i, j].setEnergy(en2);
                                    newtab[i, j] = grains[next].getIndex();
                                }
                            }
                        }
                    }
                    else if (tab[i, j] == 0)
                    {

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

                    if (isEnergy)
                    {
                        int sum = 0;
                        int diferent = 0;
                        for (int z = 0; z < grains.Count; ++z)
                        {
                            if (tab[i, j] != grains[z].getIndex())
                            {
                                diferent++;
                                int tmp = neighbour[z];
                                while (tmp > 0)
                                {
                                    sum++;
                                    tmp--;
                                }
                            }
                        }
                        int en = jgb * sum;
                        if (diferent > 0)
                        {
                            int next = 0;
                            bool a = true;
                            while (a)
                            {
                                next = random.Next(0, grains.Count);
                                if (neighbour[next] != 0) a = false;
                            }

                            int sum2 = 0;

                            for (int z = 0; z < grains.Count; ++z)
                            {
                                if (grains[next].getIndex() != grains[z].getIndex())
                                {

                                    int tmp = neighbour[z];
                                    while (tmp > 0)
                                    {
                                        sum2++;
                                        tmp--;
                                    }
                                }
                            }
                            int en2 = jgb * sum2;

                            int deltaEn = en2 - en;
                            if (deltaEn <= 0)
                            {
                                gravityList[i, j].setEnergy(en2);
                                newtab[i, j] = grains[next].getIndex();
                            }
                            else
                            {
                                double pos = Math.Exp(-deltaEn / kt);
                                double rand = random.NextDouble();

                                if (rand <= pos)
                                {
                                    gravityList[i, j].setEnergy(en2);
                                    newtab[i, j] = grains[next].getIndex();
                                }
                            }
                        }
                    }
                    else if (tab[i, j] == 0)
                    {

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

                    if (isEnergy)
                    {
                        int sum = 0;
                        int diferent = 0;
                        for (int z = 0; z < grains.Count; ++z)
                        {
                            if (tab[i, j] != grains[z].getIndex())
                            {
                                diferent++;
                                int tmp = neighbour[z];
                                while (tmp > 0)
                                {
                                    sum++;
                                    tmp--;
                                }
                            }
                        }
                        int en = jgb * sum;
                        if (diferent > 0)
                        {
                            int next = 0;
                            bool a = true;
                            while (a)
                            {
                                next = random.Next(0, grains.Count);
                                if (neighbour[next] != 0) a = false;
                            }

                            int sum2 = 0;

                            for (int z = 0; z < grains.Count; ++z)
                            {
                                if (grains[next].getIndex() != grains[z].getIndex())
                                {

                                    int tmp = neighbour[z];
                                    while (tmp > 0)
                                    {
                                        sum2++;
                                        tmp--;
                                    }
                                }
                            }
                            int en2 = jgb * sum2;

                            int deltaEn = en2 - en;
                            if (deltaEn <= 0)
                            {
                                gravityList[i, j].setEnergy(en2);
                                newtab[i, j] = grains[next].getIndex();
                            }
                            else
                            {
                                double pos = Math.Exp(-deltaEn / kt);
                                double rand = random.NextDouble();

                                if (rand <= pos)
                                {
                                    gravityList[i, j].setEnergy(en2);
                                    newtab[i, j] = grains[next].getIndex();
                                }
                            }
                        }
                    }
                    else if (tab[i, j] == 0)
                    {

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

        private void radiusGrowth()
        {
            if (!Int32.TryParse(textBox4.Text, out radius))
            {
                radius = 2;
            }
            if (checkBox1.Checked) { period = true; } else { period = false; }

            for (int i = 0; i < nodesPerWidth; i++)
            {
                for (int j = 0; j < nodesPerHeight; j++)
                {
                        int[] neighbour = new int[grains.Count];

                        for (int k = -radius; k <= radius; k++)
                        {

                            for (int z = -radius; z <= radius; z++)
                            {
                                int one = i + k; ;
                                int two = j + z; ;

                                int tmp = 0;
                                int tmp2 = 0;

                                if (period)
                                {

                                    if (one < 0) {
                                        one = nodesPerWidth + i + k;
                                        tmp = -1;
                                    }
                                    if (
                                        one >= nodesPerWidth) { one = one - nodesPerWidth;
                                        tmp = 1;
                                    }
                                    
                                    if (two < 0) {
                                        two = nodesPerHeight + j + z;
                                        tmp2 = -1;
                                    }
                                    if (
                                        two >= nodesPerHeight) { two = two - nodesPerHeight;
                                        tmp2 = 1;
                                    }

                                }

                                if (one >= 0 && one < nodesPerWidth && two >= 0 && two < nodesPerHeight)
                                {

                                    if (tab[one, two] != 0 &&
                                        (Math.Sqrt(Math.Pow(((gravityList[i, j].getX() + i * PIXEL_SIZE) - (gravityList[one, two].getX() + one * PIXEL_SIZE + tmp* nodesPerWidth *PIXEL_SIZE)), 2) + Math.Pow(((gravityList[i, j].getY() + j * PIXEL_SIZE) - (gravityList[one, two].getY() + two * PIXEL_SIZE + tmp2 * nodesPerHeight * PIXEL_SIZE)), 2)))/PIXEL_SIZE <= radius)
                                    {
                                        neighbour[tab[one, two] - 1]++;
                                        //Console.WriteLine(Math.Sqrt(Math.Pow(((gravityList[i, j].getX() + i * PIXEL_SIZE) - (gravityList[one, two].getX() + one * PIXEL_SIZE)), 2) + Math.Pow(((gravityList[i, j].getY() + j * PIXEL_SIZE) - (gravityList[one, two].getY() + two * PIXEL_SIZE)), 2)));
                                    }
                                }
                            }
                        }

                        if (isEnergy)
                        {
                            int sum = 0;
                            int diferent = 0;
                            for (int z = 0; z < grains.Count; ++z)
                            {
                                if (tab[i, j] != grains[z].getIndex())
                                {
                                    diferent++;
                                    int tmp = neighbour[z];
                                    while (tmp > 0)
                                    {
                                        sum++;
                                        tmp--;
                                    }
                                }
                            }
                            int en = jgb * sum;
                            if (diferent > 0)
                            {
                                int next = 0;
                                bool a = true;
                                while (a)
                                {
                                    next = random.Next(0, grains.Count);
                                    if (neighbour[next] != 0) a = false;
                                }

                                int sum2 = 0;

                                for (int z = 0; z < grains.Count; ++z)
                                {
                                    if (grains[next].getIndex() != grains[z].getIndex())
                                    {

                                        int tmp = neighbour[z];
                                        while (tmp > 0)
                                        {
                                            sum2++;
                                            tmp--;
                                        }
                                    }
                                }
                                int en2 = jgb * sum2;

                                int deltaEn = en2 - en;
                                if (deltaEn <= 0)
                                {
                                    gravityList[i, j].setEnergy(en2);
                                    newtab[i, j] = grains[next].getIndex();
                                }
                                else
                                {
                                    double pos = Math.Exp(-deltaEn / kt);
                                    double rand = random.NextDouble();

                                    if (rand <= pos)
                                    {
                                        gravityList[i, j].setEnergy(en2);
                                        newtab[i, j] = grains[next].getIndex();
                                    }
                                }
                            }
                        }
                        else if (tab[i, j] == 0)
                        {

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
            
            if (showEnergy)
            {
                for (int i = 0; i < nodesPerWidth; i++)
                {
                    for (int j = 0; j < nodesPerHeight; j++)
                    {
                        int energy = gravityList[i, j].getEnergy();
                        if (energy == 0)
                        {
                            
                            g.FillRectangle(brushEnergy0, (i * PIXEL_SIZE), (j * PIXEL_SIZE), PIXEL_SIZE, PIXEL_SIZE);
                        }
                        else
                        {
                            SolidBrush brushEn = new SolidBrush(Color.FromArgb(0, 255 - energy * 32, 0));
                            g.FillRectangle(brushEn, (i * PIXEL_SIZE), (j * PIXEL_SIZE), PIXEL_SIZE, PIXEL_SIZE);
                        }
                        
                        g.Flush();
                    }
                }
            }
            else
            {

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
                        g.FillRectangle(brushx, i * PIXEL_SIZE + gravityList[i, j].getX(), j * PIXEL_SIZE + +gravityList[i, j].getY(), PIXEL_SIZE / 10, PIXEL_SIZE / 10);

                        if(dislocate && gravityList[i, j].getState() == true)
                        {
                            red = random.Next(0, 255);
                            g.FillRectangle(gravityList[i, j].getB(), (i * PIXEL_SIZE), (j * PIXEL_SIZE), PIXEL_SIZE, PIXEL_SIZE);
                        }
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
            timer1.Stop();

            isEnergy = false;

            for (int i = 0; i < nodesPerWidth; i++)
            {
                for (int j = 0; j < nodesPerHeight; j++)
                {
                    tab[i, j] = 0;
                    newtab[i, j] = 0;
                    gravityList[i, j].setEnergy(0);
                }
            }

            full = false;

            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox2.Enabled = false;
            checkBox3.Enabled = false;

            draw();
            button7.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;

            
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

            gravityList = new Gravity[nodesPerWidth, nodesPerHeight];
            gravityList2 = new bool[nodesPerWidth, nodesPerHeight];
            gravityList3 = new bool[nodesPerWidth, nodesPerHeight];

            for (int i = 0; i < nodesPerWidth; i++)
            {
                for (int j = 0; j < nodesPerHeight; j++)
                {
                    tab[i, j] = 0;
                    newtab[i, j] = 0;

                    int a = random.Next(0, PIXEL_SIZE);
                    int b = random.Next(0, PIXEL_SIZE);


                    gravityList[i, j] = new Gravity(a, b);
                    gravityList2[i, j] = false;
                    gravityList3[i, j] = false;

                }
            }

            button7.Enabled = false;

            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;

            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;

            draw();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
        }

        class Gravity
        {
            int x;
            int y;
            int energy = 0;
            double dislocation = 0;
            bool state = false;
            SolidBrush b;


            public Gravity(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public int getX() {
                return this.x;
            }

            public int getY()
            {
                return this.y;
            }

            public int getEnergy()
            {
                return this.energy;
            }

            public void setEnergy(int en)
            {
                this.energy = en;
            }

            public SolidBrush getB()
            {
                return this.b;
            }

            public void setB(int r)
            {
                this.b = new SolidBrush(Color.FromArgb(r, 0, 0));
            }

            public double getDislocation()
            {
                return this.dislocation;
            }

            public void setDislocation(double dis)
            {
                this.dislocation = dis;
            }

            public bool getState()
            {
                return this.state;
            }

            public void setState(bool st)
            {
                this.state = st;
            }
        }

        private double dislication(double time)
        {
            return(((double)A / (double)B) + (1.0 - ((double)A / (double)B)) * (double)Math.Exp(-(double)B * (double)time));
        }

        private double rozrzuc30(double val)
        {
            double wymiar = nodesPerHeight * nodesPerWidth;
            double tmpval = val;

            double pack = ((double)val / (double)wymiar) * 0.3;
            val -= val * 0.3;

            for(int i = 0; i < nodesPerWidth; i++)
            {
                for(int j = 0; j < nodesPerHeight; j++)
                {
                    gravityList[i, j].setDislocation(gravityList[i, j].getDislocation() + pack);
                    sumDis += pack;
                }
            }
            return val;
        }

        private void rozrzuc_rowno(double val, double percent)
        {
            double wymiar = nodesPerHeight * nodesPerWidth;
            double tmpval = val;

            double pack = ((double)val / (double)wymiar) * percent;

            while (val > 0)
            {
                int i = random.Next(0, nodesPerWidth);
                int j = random.Next(0, nodesPerHeight);
                int chance = random.Next(0, 100);
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
                int dif = 0;

                int chosen = listBox1.SelectedIndex;
                List<Location> neighbour;

                if (chosen == 0)
                {
                    neighbour = vonNeumannDis(i, j);
                }
                else if (chosen >= 2 && chosen <= 6)
                {
                    neighbour = pentagonalDis(i, j);
                }
                else if (chosen >= 7 && chosen <= 9)
                {
                    neighbour = hexagonalDis(i, j);
                }
                else
                {
                    neighbour = mooreDis(i, j);
                }

                for(int x = 0; x < neighbour.Count; x++)
                {
                    if(tab[i, j] != tab[neighbour[x].getX(), neighbour[x].getY()])
                    {
                        dif++;
                    }
                }


                if (chance < 80)
                {
                    if (dif != 0){
                        gravityList[i, j].setDislocation(gravityList[i, j].getDislocation() + pack);
                        val -= pack;
                        sumDis += pack;
                    }
                }
                else
                {
                    if (dif == 0){
                        gravityList[i, j].setDislocation(gravityList[i, j].getDislocation() + pack);
                        val -= pack;
                        sumDis += pack;
                    }
                }
            }
        }

        private void zapis_do_pliku()
        {
            

            double sum = 0;

            for (int i = 0; i < nodesPerWidth; i++)
            {
                for (int j = 0; j < nodesPerHeight; j++)
                {
                    sum += gravityList[i, j].getDislocation();
                }
            }
            sw.Write(sum + "\n");
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            dislocate = true;
            disMaxPerCell = (double)disMax / (double)(nodesPerHeight * nodesPerWidth);
        }

        private List<Location> vonNeumannDis(int i, int j) {
            
            if (checkBox1.Checked) { period = true; } else { period = false; }

            List<Location> neighbour = new List<Location>();
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

            neighbour.Add(new Location(left, j));
            neighbour.Add(new Location(right, j));
            neighbour.Add(new Location(i, up));
            neighbour.Add(new Location(i, down));
            

            return neighbour;
        }

        private List<Location> mooreDis(int i, int j)
        {
            if (checkBox1.Checked) { period = true; } else { period = false; }

            List<Location> neighbour = new List<Location>();
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

            neighbour.Add(new Location(left, j));
            neighbour.Add(new Location(right, j));
            neighbour.Add(new Location(i, up));
            neighbour.Add(new Location(i, down));
            neighbour.Add(new Location(left, down));
            neighbour.Add(new Location(right, down));
            neighbour.Add(new Location(left, up));
            neighbour.Add(new Location(right, up));

            return neighbour;
        }

        private List<Location> pentagonalDis(int i, int j)
        {
            if (checkBox1.Checked) { period = true; } else { period = false; }

            List<Location> neighbour = new List<Location>();
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
                neighbour.Add(new Location(left, j));
                neighbour.Add(new Location(right, j));
                neighbour.Add(new Location(i, up));
                neighbour.Add(new Location(left, up));
                neighbour.Add(new Location(right, up));
            }
            if (chosen == 4)
            {
                neighbour.Add(new Location(left, j));
                neighbour.Add(new Location(right, j));
                neighbour.Add(new Location(i, down));
                neighbour.Add(new Location(left, down));
                neighbour.Add(new Location(right, down));
            }
            if (chosen == 5)
            {
                neighbour.Add(new Location(left, j));
                neighbour.Add(new Location(i, up));
                neighbour.Add(new Location(i, down));
                neighbour.Add(new Location(left, down));
                neighbour.Add(new Location(left, up));
            }
            if (chosen == 6)
            {
                neighbour.Add(new Location(right, j));
                neighbour.Add(new Location(i, up));
                neighbour.Add(new Location(i, down));
                neighbour.Add(new Location(right, down));
                neighbour.Add(new Location(right, up));
            }
            
            return neighbour;
        }

        private List<Location> hexagonalDis(int i, int j)
        {
            if (checkBox1.Checked) { period = true; } else { period = false; }

            List<Location> neighbour = new List<Location>();
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
                chosen = random.Next(8, 9);
            }
            if (chosen == 8)
            {
                neighbour.Add(new Location(left, j));
                neighbour.Add(new Location(right, j));
                neighbour.Add(new Location(i, up));
                neighbour.Add(new Location(i, down));
                neighbour.Add(new Location(left, down));
                neighbour.Add(new Location(right, up));
            }
            if (chosen == 9)
            {
                neighbour.Add(new Location(left, j));
                neighbour.Add(new Location(right, j));
                neighbour.Add(new Location(i, down));
                neighbour.Add(new Location(i, up));
                neighbour.Add(new Location(left, up));
                neighbour.Add(new Location(right, down));
            }
            

            return neighbour;
        }



        private void button9_Click(object sender, EventArgs e)
        {
            secondForm.Show();
        }
    }


    public class Location
    {
        private int x;
        private int y;

        public Location(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int getX()
        {
            return this.x;
        }

        public int getY()
        {
            return this.y;
        }
    }
}
