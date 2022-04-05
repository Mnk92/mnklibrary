using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Mnk.Library.WpfControls.Components.Drawings.Background;

namespace Mnk.Library.WpfControls.Components.Drawings.Graphics
{
    public class Graph : GraphPanel
    {
        private readonly IList<IGraphic> graphics = new List<IGraphic>();

        public Graph()
        {
            var pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            Back = new CaptionedGridBackground(pixelsPerDip, "x", "y");
            Counts = Array.Empty<int>();
            PointsDistance = 3;
        }

        public void Clear()
        {
            Counts = Array.Empty<int>();
            Min = Max = 0;
            graphics.Clear();
            Redraw();
        }

        public void Add(IGraphic graphic)
        {
            Counts = new int[graphics.Count + 1];
            graphics.Add(graphic);
        }

        protected override void Update(Rect rect)
        {
            Counts = graphics.Select(x => x.Count).ToArray();
            for (var i = 0; i < graphics.Count; ++i)
            {
                graphics[i].CalcParameters(Counts[i], rect, PointsDistance);
            }
            Min = graphics.Min(x => x.Min);
            Max = graphics.Max(x => x.Max);
        }

        protected override void Render(DrawingContext dc, Rect rect)
        {
            if (Counts.Min() < 2) return;
            for (var i = 0; i < graphics.Count; ++i)
            {
                graphics[i].Paint(dc, rect, Min, Max, Counts[i]);
            }
        }

        public IEnumerable<float> GetValues(float position)
        {
            return Any ? graphics.Select((g, i) => g.GetValue(position, Counts[i])) : Array.Empty<float>();
        }

        public int[] Counts { get; private set; }
        public float Min { get; private set; }
        public float Max { get; private set; }
        public override bool Any { get { return graphics.Count > 0 && graphics.All(x => x.Any); } }
        public float Average { get { return graphics.Average(x => x.Average); } }
        public float Last => First.Last;
        public IGraphic First => graphics.First();
        public int PointsDistance { get; set; }
    }
}
