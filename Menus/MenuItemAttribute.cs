using System;

namespace Menus
{
	[AttributeUsage(AttributeTargets.Class)]
	public class MenuItemAttribute : Attribute
	{
		public string Title { get; set; }
		public Type Parent { get; set; }
		public string Url { get; set; }
		public MenuItemAttribute(string title, string url, Type parent = null)
		{
			Title = title;
			Parent = parent;
			Url = url;
		}
	}
}
