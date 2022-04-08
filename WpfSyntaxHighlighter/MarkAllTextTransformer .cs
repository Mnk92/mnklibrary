using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using Brush = System.Drawing.Brush;
using Brushes = System.Drawing.Brushes;

namespace Mnk.Library.WpfSyntaxHighlighter
{
    internal class MarkAllTextTransformer : DocumentColorizingTransformer
    {
        private readonly string textToMark;
        private readonly StringComparison comparison;
        public int Replacements { get; set; }
        public MarkAllTextTransformer(string textToMark, bool matchCase)
        {
            this.textToMark = textToMark;
            this.comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }
        protected override void ColorizeLine(DocumentLine line)
        {
            var lineStartOffset = line.Offset;
            var text = CurrentContext.Document.GetText(line);
            var start = 0;
            int index;
            while ((index = text.IndexOf(textToMark, start, comparison)) >= 0)
            {
                var selection = lineStartOffset + index;
                ++Replacements;
                ChangeLinePart(
                    selection,
                    selection + textToMark.Length,
                    (element) =>
                    {
                        element.TextRunProperties.SetBackgroundBrush(System.Windows.Media.Brushes.LimeGreen);
                    });
                start = index + 1;
            }
        }
    }
}
