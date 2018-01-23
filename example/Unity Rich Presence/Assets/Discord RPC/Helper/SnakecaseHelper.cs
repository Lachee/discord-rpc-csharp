using System;
using System.Linq;

public static class SnakecaseHelper
{
	/// <summary>
	/// Converts the current enum value into a lower_snake_case string using <see cref="ToSnakeCase(string)"/>. 
	/// </summary>
	/// <param name="value">The value to convert</param>
	/// <returns>Returns lower_snake_case of the value</returns>
	public static string ToSnakeCase(this Enum value)
	{
		return value.ToString().ToSnakeCase();
	}

	/// <summary>
	/// Converts a CamelCaseString to a snake_case_string.
	/// </summary>
	/// <param name="str">The string to convert</param>
	/// <returns>Returns lower_snake_case of the value.</returns>
	public static string ToSnakeCase(this string str)
	{
		if (str == null) return null;
		return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
	}
}
