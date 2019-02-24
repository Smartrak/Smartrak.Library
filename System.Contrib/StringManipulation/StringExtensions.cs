using System.Globalization;
using System.Text;

namespace System.Contrib.StringManipulation
{
	/// <summary>
	/// Extensions for <see cref="string"/>.
	/// </summary>
	public static class StringExtensions
	{
		private static readonly char[] NewLineChars = { '\r', '\n' };
		private static readonly char[] IndentationChars = { '\t', ' ' };

		/// <summary>
		/// Expands a camelCase or PascalCase string by inserting spaces between each word.
		/// </summary>
		/// <example>
		/// "MyEnumValue" -> "My Enum Value"
		/// "IEnumerable" -> "I Enumerable"
		/// "FooID" -> "Foo ID"
		/// </example>
		public static string ExpandCamelCase(this string str)
		{
			if (str == null)
				return string.Empty;

			var builder = new StringBuilder();

			for (var i = 0; i < str.Length; i++)
			{
				var c1 = str[i];
				var c2 = i < str.Length - 1 ? str[i + 1] : (char?) null;
				var c3 = i < str.Length - 2 ? str[i + 2] : (char?) null;

				builder.Append(i == 0 ? char.ToUpper(c1) : c1);

				// "aB" -> "a B"
				if (c2.HasValue && char.IsLower(c1) && char.IsUpper(c2.Value))
					builder.Append(' ');
				// "ABc" -> "A Bc"
				else if (c2.HasValue && c3.HasValue && char.IsUpper(c1) && char.IsUpper(c2.Value) && char.IsLower(c3.Value))
					builder.Append(' ');
			}

			return builder.ToString();
		}

		/// <summary>
		/// Returns the input string as Title Case.
		/// The first letter of each word is capitalized, while the remaining characters are lower cased.
		/// Words consisting entirely of uppercase characters are not modified.
		/// </summary>
		/// <example>
		/// "hello world" -> "Hello World"
		/// "FOO bAr" -> "FOO Bar"
		/// </example>
		public static string ToTitleCase(this string str)
		{
			if (str == null)
				return string.Empty;

			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
		}

		/// <summary>
		/// Removes leading and trailing whitespace, and reduces multiple internal consecutive whitespace to a single space.
		/// </summary>
		/// <example>
		/// " foo bar	  zip " -> "foo bar zip"
		/// </example>
		public static string TrimInternal(this string str)
		{
			if (str == null)
				return string.Empty;

			var haveSpace = false;
			var builder = new StringBuilder();

			foreach (var chr in str.Trim())
			{
				if (char.IsWhiteSpace(chr))
				{
					if (haveSpace)
						continue;

					builder.Append(' ');
					haveSpace = true;
				}
				else
				{
					builder.Append(chr);
					haveSpace = false;
				}
			}

			return builder.ToString();
		}

		/// <summary>
		/// Removes leading indentation (tabs, spaces) and newlines (\r, \n) from this string
		/// </summary>
		public static string TrimFormatting(this string str)
		{
			if (str == null)
				return string.Empty;

			var builder = new StringBuilder();

			foreach (var line in str.Split(NewLineChars, StringSplitOptions.RemoveEmptyEntries))
				builder.Append(line.TrimStart(IndentationChars));

			return builder.ToString();
		}
	}
}
