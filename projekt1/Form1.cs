using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace projekt1
{
    public partial class Form1 : Form
    {

        Bitmap DrawArea;
        Graphics g;
        Pen pen;
        Brush brush;
        int y = 0;
        int rectSize = 10;
        int count = 500;
        int rule;
        string binaryRule;

        bool start = true;

        List<List<bool>> states;
        public Form1()
        {
            InitializeComponent();

            DrawArea = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = DrawArea;

            states = new List<List<bool>>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            g = Graphics.FromImage(DrawArea);
            pen = new Pen(Brushes.Black);
            brush = new SolidBrush(Color.Black);

            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 1;
        }

        private void calculate()
        {
            List<bool> newState = Enumerable.Repeat(false, count).ToList();
            int previousState = states.Count - 1;

            for (int i = 0; i < newState.Count; i++)
            {
                string binary = "";

                int previous = i - 1;
                if(previous < 0)
                {
                    previous = newState.Count - 1;
                }

                int next = i + 1;
                if(next >= newState.Count)
                {
                    next = 0;
                }

                binary += states[previousState][previous] ? "1" : "0";
                binary += states[previousState][i] ? "1" : "0";
                binary += states[previousState][next] ? "1" : "0";

                int index = Convert.ToInt32(binary, 2);

                if(binaryRule.Length > index && binaryRule[binaryRule.Length - index - 1] == '1')
                {
                    newState[i] = true;
                }
            }

            states.Add(newState);
        }

        private void draw()
        {
            int x = 0;
            foreach(bool state in states[states.Count - 1])
            {
                if (state)
                {
                    g.FillRectangle(brush, x, y, rectSize, rectSize);
                    g.Flush();
                }
                x += rectSize;
            }
            y += rectSize;
            pictureBox1.Image = DrawArea;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            if (start == false)
            {

                DrawArea = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
                pictureBox1.Image = DrawArea;
                g.Clear(Color.White);
                g = Graphics.FromImage(DrawArea);

                states.Clear();
                y = 0;
                
            }
            start = false;

            if (!Int32.TryParse(textBox2.Text, out rule) || rule<0)
            {
                rule = 90;
            }
            

            binaryRule = Convert.ToString(rule, 2);

            if(!Int32.TryParse(textBox1.Text, out count) || count < 0)
            {
                count = pictureBox1.Width;
            }

            rectSize = pictureBox1.Size.Width / count;

            List<bool> initialState = Enumerable.Repeat(false, count).ToList();
            initialState[count / 2] = true;
            states.Add(initialState);
            draw();
            

            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            calculate();
            draw();
        }
    }
}
