using System.Linq;

namespace blazorclient.Services
{
	public static class StringExtension
	{
		public static bool Contains(this string value, char[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				if (value.Contains(values.ElementAt(i)))
					return true;
			}
			return false;
		}
	}
}
