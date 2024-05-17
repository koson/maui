namespace Microsoft.Maui.Graphics
{
	public static class IFontExtensions
	{
		public static string GetSvgWeight(this IFont font)
		{
			if (font == null)
			{
				return null;
			}

			if (font.Weight == FontWeights.Normal)

/* Unmerged change from project 'Graphics(net8.0-windows10.0.19041)'
Before:
				return "normal";
After:
			{
				return "normal";
			}
*/
			
/* Unmerged change from project 'Graphics(net8.0-windows10.0.19041)'
Before:
			if (font.Weight == FontWeights.Bold)
				return "bold";
After:
			if (font.Weight == FontWeights.Regular)
			{
				return "normal";
			}

			if (font.Weight == FontWeights.Bold)
			{
				return "bold";
			}
*/
{
				return "normal";
			}

			if (font.Weight == FontWeights.Regular)
			{
				return "normal";
			}

			if (font.Weight == FontWeights.Bold)
			{
				return "bold";
			}

			return font.Weight.ToInvariantString();
		}

		public static string GetSvgStyle(this IFont font)
		{
			if (font == null)
			{
				return null;
			}

			if (font.StyleType == FontStyleType.Italic)
			{
				return "italic";
			}

			if (font.StyleType == FontStyleType.Oblique)
			{
				return "oblique";
			}

			return "normal";
		}
	}
}
