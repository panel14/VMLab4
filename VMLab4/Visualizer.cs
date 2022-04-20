using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace VMLab4
{
    internal class Visualizer
    {
        private static int highlightIndex = 1;
        private static Color def = Color.CornflowerBlue;
        public static void InitGraph(ref Chart graph)
        {
            graph.Series.Clear();
            String[] names = { "Function", "First app", "Second app" };

            for (int i = 0; i < 3; i++)
            {
                graph.Series.Add(new Series(names[i]));
                graph.Series[i].ChartType = SeriesChartType.Point;
                graph.Series[i].LegendText = names[i];
            }
            graph.ChartAreas[0].AxisX.Interval = 1;
            graph.ChartAreas[0].AxisX.LabelStyle.Format = "{0.00}";

            graph.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;

            graph.Series[0].MarkerStyle = MarkerStyle.Circle;

            graph.Series[1].IsVisibleInLegend = false;
            graph.Series[2].Color = Color.Green;
            graph.Series[2].IsVisibleInLegend = false;
        }

        public static void PrintPoints(Point[] points, ref Chart chart)
        {
            for (int i = 0; i < 3; i++)
            {
                chart.Series[i].Points.Clear();
                chart.Series[i].IsVisibleInLegend = false;
            }

            chart.Series[0].IsVisibleInLegend = true;

            for (int i = 0; i < points.Length; i++)
            {
                chart.Series[0].Points.AddXY(points[i].x, points[i].y);
            }
        }

        public static void HighlightPoint(ref Chart chart, int index)
        {
            highlightIndex = index;
            chart.Series[0].Points[index].Color = Color.Red;
        }

        public static void CleanArea(ref Chart chart)
        { 
            chart.Series[0].Points[highlightIndex].Color = def;
            chart.Series[1].Points.Clear();
            chart.Series[2].Points.Clear();
        }

        public static void PrintApproximation(Point[] points, ref Chart chart, int appNum)
        {
            chart.Series[appNum].ChartType = SeriesChartType.Spline;

            for (int i = 0; i < points.Length; i++)
            {
                chart.Series[appNum].Points.AddXY(points[i].x, points[i].y);
            }

            chart.Series[appNum].IsVisibleInLegend = true;
        }
    }
}
