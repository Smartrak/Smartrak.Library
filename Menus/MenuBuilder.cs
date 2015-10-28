using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Menus
{
	public static class MenuBuilder
	{
		public static IEnumerable<MenuItem> BuildMenus(Assembly entry)
		{
			var rootMenuItems = new Dictionary<Type, MenuItem>();
			var allMenuItems = new Dictionary<Type, MenuItem>();

			foreach (var typeWithAttribute in entry
				.GetTypes()
				.SelectMany(t => t.GetCustomAttributes().Where(a => a is MenuItemAttribute).Select(a => new { Type = t, Attribute = a as MenuItemAttribute }))
				.OrderBy(x => x.Attribute.Parent == null))
			{
				var menuItem = new MenuItem { Title = typeWithAttribute.Attribute.Title, Url = typeWithAttribute.Attribute.Url };
				allMenuItems.Add(typeWithAttribute.Type, menuItem);
				if (typeWithAttribute.Attribute.Parent == null)
				{
					rootMenuItems.Add(typeWithAttribute.Type, menuItem);
				}
				else
				{
					if (!rootMenuItems.ContainsKey(typeWithAttribute.Type))
					{
						//TODO: support nesting.
						throw new NotImplementedException("Only one level of menu is supported");
					}
					rootMenuItems[typeWithAttribute.Type].Children.Add(menuItem);
				}
			}

			return rootMenuItems.Values;
		}
	}


}
