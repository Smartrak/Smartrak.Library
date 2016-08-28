using System;
using System.Collections.Generic;

namespace Menus
{
	public class MenuItem
	{
		public string Title { get; set; }
		public string Url { get; set; }
		public Type Type { get; set; }
		public List<MenuItem> Children { get; set; } = new List<MenuItem>();
	}
}
