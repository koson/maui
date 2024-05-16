#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Microsoft.Maui.Controls.StyleSheets
{
	/// <include file="../../../docs/Microsoft.Maui.Controls.StyleSheets/StyleSheet.xml" path="Type[@FullName='Microsoft.Maui.Controls.StyleSheets.StyleSheet']/Docs/*" />
	public sealed class StyleSheet : IStyle
	{
		StyleSheet()
		{
		}

		internal IDictionary<Selector, Style> Styles { get; set; } = new Dictionary<Selector, Style>();

		//used by code generated by XamlC. Has to stay public
		/// <include file="../../../docs/Microsoft.Maui.Controls.StyleSheets/StyleSheet.xml" path="//Member[@MemberName='FromResource']/Docs/*" />
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static StyleSheet FromResource(string resourcePath, Assembly assembly, IXmlLineInfo lineInfo = null)
		{
			var styleSheet = new StyleSheet();
			var resString = DependencyService.Get<IResourcesLoader>().GetResource(resourcePath, assembly, styleSheet, lineInfo);
			using (var textReader = new StringReader(resString))
			using (var cssReader = new CssReader(textReader))
			{
				Parse(styleSheet, cssReader);
			}

			return styleSheet;
		}

		//used by code generated by XamlC. Has to stay public
		/// <include file="../../../docs/Microsoft.Maui.Controls.StyleSheets/StyleSheet.xml" path="//Member[@MemberName='FromString']/Docs/*" />
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static StyleSheet FromString(string stylesheet)
		{
			if (stylesheet == null)
			{
				throw new ArgumentNullException(nameof(stylesheet));
			}

			using (var reader = new StringReader(stylesheet))
			{
				return FromReader(reader);
			}
		}

		/// <include file="../../../docs/Microsoft.Maui.Controls.StyleSheets/StyleSheet.xml" path="//Member[@MemberName='FromReader']/Docs/*" />
		public static StyleSheet FromReader(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}

			var sheet = new StyleSheet();
			using (var cssReader = new CssReader(reader))
			{
				Parse(sheet, cssReader);
			}

			return sheet;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void Parse(StyleSheet sheet, CssReader reader)
		{
			Style style = null;
			var selector = Selector.All;

			int p;
			bool inStyle = false;
			reader.SkipWhiteSpaces();
			while ((p = reader.Peek()) > 0)
			{
				switch ((char)p)
				{
					case '@':
						throw new NotSupportedException("AT-rules not supported");
					case '{':
						reader.Read();
						style = Style.Parse(reader, '}');
						inStyle = true;
						break;
					case '}':
						reader.Read();
						if (!inStyle)
						{
						{
							throw new Exception();
						}

						inStyle = false;
						sheet.Styles.Add(selector, style);
						style = null;
						selector = Selector.All;
						break;
					default:
						selector = Selector.Parse(reader, '{');
						break;
				}
			}
		}

		Type IStyle.TargetType => typeof(VisualElement);

		void IStyle.Apply(BindableObject bindable, SetterSpecificity setterspecificity)
		{
			if (!(bindable is Element styleable))
			{
				return;
			}

			Apply(styleable);
		}

		void Apply(Element styleable)
		{
			if (!(styleable is VisualElement visualStylable))
			{
				return;
			}

			foreach (var kvp in Styles)
			{
				var selector = kvp.Key;
				var style = kvp.Value;
				if (!selector.Matches(styleable))
				{
					continue;
				}

				style.Apply(visualStylable);
			}
		}

		void IStyle.UnApply(BindableObject bindable) => throw new NotImplementedException();
	}
}
