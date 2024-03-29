﻿using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Mnk.Library.WpfControls.Components.Drawings.Background;
using Mnk.Library.WpfControls.Components.Drawings.Graphics;
using Mnk.Library.WpfControls.Components.Drawings.Graphics.Painters;

namespace Mnk.Library.WpfControls.Components.Units
{
    /// <summary>
    /// Interaction logic for GraphUnit.xaml
    /// </summary>
    public partial class GraphUnit
    {
        private readonly CaptionedGridBackground background;
        private readonly ToolTip toolTip;

        public string OxCaption
        {
            get => background.OxCaption;
            set => background.OxCaption = value;
        }

        public string OyCaption
        {
            get => background.OyCaption;
            set => background.OyCaption = value;
        }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public GraphUnit()
        {
            InitializeComponent();
            var pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            background = new CaptionedGridBackground(pixelsPerDip, "x", "y");
            toolTip = new ToolTip { StaysOpen = true, Placement = PlacementMode.RelativePoint };
            GraphPlot.Add(new Graphic(new Pen(Brushes.DarkGreen, 1), new PolylinesStretchGraphicPainter()));
            GraphPlot.MouseMove += GraphPlotOnMouseMove;
            GraphPlot.ToolTip = toolTip;
            GraphPlot.Back = background;
            StartTime = DateTime.MinValue;
            EndTime = DateTime.MinValue;
        }

        private void GraphPlotOnMouseMove(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(GraphPlot);
            var position = (float)(p.X / GraphPlot.ActualWidth);
            var line = string.Format(CultureInfo.InvariantCulture, "{0:0.##}%", 100 * position) + Environment.NewLine +
                string.Join(Environment.NewLine, GraphPlot.GetValues(position).Select(x => string.Format(CultureInfo.InvariantCulture, "{0:0.##}", x)));
            if (StartTime != DateTime.MinValue)
            {
                var end = EndTime;
                if (end == DateTime.MinValue) end = DateTime.Now;
                var delta = (end - StartTime).TotalSeconds * position;
                line += Environment.NewLine + StartTime.AddSeconds(delta).ToString("T");
            }
            toolTip.Content = line;
            toolTip.HorizontalOffset = p.X + 20;
            toolTip.VerticalOffset = p.Y;
        }

        public void AddGrapic(IGraphic graphic)
        {
            GraphPlot.Add(graphic);
        }

        public void Clear()
        {
            GraphPlot.Clear();
        }

        public void Redraw()
        {
            if (GraphPlot.Any)
            {
                lMin.Content = GraphPlot.Min;
                lMax.Content = GraphPlot.Max;
                lAvg.Content = GraphPlot.Average;
                lCurrent.Content = GraphPlot.Last;
            }
            else
            {
                lMin.Content = 0;
                lMax.Content = 0;
                lAvg.Content = 0;
                lCurrent.Content = 0;
            }
            GraphPlot.Redraw();
        }

        public Graph Graph => GraphPlot;
        public IGraphic MainGraphic => GraphPlot.First;

        private void EdgeModeChanged(object sender, RoutedEventArgs e)
        {
            RenderOptions.SetEdgeMode(this,
                (Aliased.IsChecked == true) ? EdgeMode.Aliased : EdgeMode.Unspecified);
            if (Graph != null)
            {
                Graph.PointsDistance = (Aliased.IsChecked == true) ? 3 : 2;
                Redraw();
            }
        }
    }
}
