using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Arpg.Client.Extensions;

public static class LayoutExtensions
{
    extension(Visual)
    {
        /// <summary>
        /// Mede a altura necessária para um texto dado uma largura disponível e estilos.
        /// </summary>
        public static double MeasureTextHeight(string? text, double availableWidth, double fontSize, FontWeight fontWeight = FontWeight.Normal, TextWrapping wrapping = TextWrapping.Wrap)
        {
            if (string.IsNullOrEmpty(text)) return 0;

            var textBlock = new TextBlock
            {
                Text = text,
                FontSize = fontSize,
                FontWeight = fontWeight,
                TextWrapping = wrapping
            };

            textBlock.Measure(new Size(availableWidth > 0 ? availableWidth : double.PositiveInfinity, double.PositiveInfinity));
            return textBlock.DesiredSize.Height;
        }

        /// <summary>
        /// Mede a largura necessária para um texto dado estilos.
        /// </summary>
        public static double MeasureTextWidth(string? text, double fontSize, FontWeight fontWeight = FontWeight.Normal, FontFamily? fontFamily = null, FontStyle fontStyle = FontStyle.Normal)
        {
            if (string.IsNullOrEmpty(text)) return 0;

            var textBlock = new TextBlock
            {
                Text = text,
                FontSize = fontSize,
                FontWeight = fontWeight,
                FontFamily = fontFamily ?? FontFamily.Default,
                FontStyle = fontStyle
            };

            textBlock.Measure(Size.Infinity);
            return textBlock.DesiredSize.Width;
        }
    }

    /// <summary>
    /// Mede a altura necessária para o conteúdo de um Layoutable.
    /// </summary>
    public static double MeasureContentHeight(this Layoutable? layout, double availableWidth, Thickness extraPadding = default)
    {
        if (layout == null) return 0;

        layout.Measure(new Size(availableWidth > 0 ? availableWidth : double.PositiveInfinity, double.PositiveInfinity));
        return layout.DesiredSize.Height + extraPadding.Top + extraPadding.Bottom;
    }
}
