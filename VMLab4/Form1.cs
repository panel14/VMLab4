using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace VMLab4
{
    public partial class Form1 : Form
    {
        private static Point[] data;
        delegate Point[] ApproximateMethod(Point[] data, int approxNum);
        public Form1()
        {
            InitializeComponent();
            Visualizer.InitGraph(ref chart1);
            ChangeMainFunction();
        }

        private void ChangeMainFunction()
        {
            data = Solver.GetData((int)numericUpDown1.Value - 1);
            Visualizer.PrintPoints(data, ref chart1);
            panel1.Enabled = false;

            radioButton1.Checked = false; radioButton2.Checked = false; radioButton3.Checked = false;
            fApprox.Text = ""; sApprox.Text = "";
        }

        private void PrintEquats(ApproxType type)
        {
            double[] fCoefs; double[] sCoefs;
            if (type == ApproxType.linear)
            {
                fCoefs = Solver.GetLinearCoefs(0);
                fApprox.Text = "y = " + Math.Round(fCoefs[0], 3) + "x + " + Math.Round(fCoefs[1], 3);

                sCoefs = Solver.GetLinearCoefs(1);
                sApprox.Text = "y = " + Math.Round(sCoefs[0], 3) + "x + " + Math.Round(sCoefs[1], 3);
            }
            else if (type == ApproxType.exponential)
            {
                fCoefs = Solver.GetExponentialCoefs(0);
                fApprox.Text = "y = " + Math.Round(fCoefs[1], 3) + " * e^(x * " + Math.Round(fCoefs[0], 3) + ")";

                sCoefs = Solver.GetExponentialCoefs(1);
                sApprox.Text = "y = " + Math.Round(sCoefs[1], 3) + " * e^(x * " + Math.Round(sCoefs[0], 3) + ")";
            }
            else if (type == ApproxType.logarithmic)
            {
                fCoefs = Solver.GetLogarithmicFoefs(0);
                fApprox.Text = "y = " + Math.Round(fCoefs[0], 3) + "*ln(x) + " + Math.Round(fCoefs[1], 3);

                sCoefs = Solver.GetLogarithmicFoefs(1);
                sApprox.Text = "y = " + Math.Round(sCoefs[0], 3) + "*ln(x) + " + Math.Round(sCoefs[1], 3);
            }
            else
            {
                fCoefs = Solver.GetPolinomicalCoefs(0);
                fApprox.Text = "y = " + Math.Round(fCoefs[0], 3) + "x² + " + Math.Round(fCoefs[1], 3) + "x + " + Math.Round(fCoefs[2], 3);

                sCoefs = Solver.GetPolinomicalCoefs(1);
                sApprox.Text = "y = " + Math.Round(sCoefs[0], 3) + "x² + " + Math.Round(sCoefs[1], 3) + "x + " + Math.Round(sCoefs[2], 3);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            data = Solver.GetData((int)numericUpDown1.Value - 1);
            radioButton1.Checked = true;

            panel1.Enabled = true;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            ChangeMainFunction();
        }

        private void Approximate(ApproximateMethod method)
        {
            Visualizer.CleanArea(ref chart1);

            Point[] points = method(data, 0);
            Visualizer.PrintApproximation(points, ref chart1, 1);

            List<Point> temp = data.ToList();
            int index = Solver.FindMaxDeviate(data, points);
            Visualizer.HighlightPoint(ref chart1, index);

            temp.RemoveAt(index);
            Point[] newPoints = method(temp.ToArray(), 1);

            Visualizer.PrintApproximation(newPoints, ref chart1, 2);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                ApproximateMethod method = Solver.ApproximateExponential;
                Approximate(method);
                PrintEquats(ApproxType.exponential);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                ApproximateMethod method = Solver.ApproximateLinear;
                Approximate(method);
                PrintEquats(ApproxType.linear);
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                ApproximateMethod method = Solver.ApproximateLogarithmic;
                Approximate(method);
                PrintEquats(ApproxType.logarithmic);
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                ApproximateMethod method = Solver.ApproximatePolinomical;
                Approximate(method);
                PrintEquats(ApproxType.polinomical);
            }
        }
    }
}
