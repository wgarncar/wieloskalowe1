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
    public partial class Form2 : Form
    {

        private Graphics grav;
        public List<double> temp = new List<double>();
        public List<double> roList = new List<double>();
        public List<double> roListP = new List<double>();
        public int maxiter = 201;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //grav = pictureBox1.CreateGraphics();

            wykres();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void wykres()
        {
            StreamReader sr = new StreamReader("C:/Users/wgarncar/source/repos/wiel3/wiel3/Dane.txt");


            if (sr != null)
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "***") break;
                    Console.WriteLine("Odczyt");
                    Console.WriteLine(line);
                    var values = line.Split(' ');
                    temp.Add(Convert.ToDouble(values[0]));
                    Console.WriteLine(values[0]);
                    roList.Add(Convert.ToDouble(values[1]));
                    roListP.Add(Convert.ToDouble(values[2]));
                }
            }

            sr.Close();

            chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;





            Console.WriteLine("Wykres");
            Pen penCurrent = new Pen(Color.Red);
            Point[] curvePoints = new Point[maxiter];
            for (int i = 0; i < temp.Count; i++)
            {
                //curvePoints[i] = new Point(i, i);
                chart1.Series[0].Points.AddXY(temp[i], roList[i]);
                chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                Console.WriteLine(i);
            }
            //grav.Clear(Color.White);
            //grav.DrawCurve(penCurrent, curvePoints);
            //penCurrent.Dispose();
        }
    }
 

    public class PointDouble
    {
        public double x;
        public double y;

        public PointDouble(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
