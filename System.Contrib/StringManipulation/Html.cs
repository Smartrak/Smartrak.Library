namespace System.Contrib.StringManipulation
{
	public static class Html
	{
		/// <summary>
		/// Removes HTML tags from the string and returns it
		/// </summary>
		public static string RemoveHtmlTags(this string source)
		{
			//ref http://www.dotnetperls.com/remove-html-tags

			char[] array = new char[source.Length];
			int arrayIndex = 0;
			bool inside = false;

			for (int i = 0; i < source.Length; i++)
			{
				char letter = source[i];
				if (letter == '<')
				{
					inside = true;
					continue;
				}
				if (letter == '>')
				{
					inside = false;
					continue;
				}
				if (!inside)
				{
					array[arrayIndex] = letter;
					arrayIndex++;
				}
			}
			return new string(array, 0, arrayIndex);
		}
	}
}
