namespace ThinkFast.Pages;

using Microsoft.Maui.Controls;
using System.Text.RegularExpressions;



public static class MarkdownFormatter
{
    public static FormattedString Parse(string input)
    {
        var formatted = new FormattedString();
        input = input.Replace("\\n", "\n");

        var boldPattern = @"\*\*(.*?)\*\*";
        var matches = Regex.Matches(input, boldPattern);

        int lastIndex = 0;
        foreach (Match match in matches)
        {
            if (match.Index > lastIndex)
            {
                formatted.Spans.Add(new Span
                {
                    Text = input.Substring(lastIndex, match.Index - lastIndex)
                });
            }

            formatted.Spans.Add(new Span
            {
                Text = match.Groups[1].Value,
                FontAttributes = FontAttributes.Bold
            });

            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < input.Length)
        {
            formatted.Spans.Add(new Span { Text = input.Substring(lastIndex) });
        }

        // Convert * bullets to •
        foreach (var span in formatted.Spans)
        {
            span.Text = Regex.Replace(span.Text, @"\n\* ?", "\n• ");
        }

        return formatted;
    }
}