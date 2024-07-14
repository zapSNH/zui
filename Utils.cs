
using System.Text.RegularExpressions;

namespace ZUI {
	internal static class Utils {
		// https://stackoverflow.com/a/4489046
		// https://creativecommons.org/licenses/by-sa/4.0/
		internal static string CamelCaseToHumanReadable(this string str) {
			return Regex.Replace(
				Regex.Replace(
					str,
					@"(\P{Ll})(\P{Ll}\p{Ll})",
					"$1 $2"
				),
				@"(\p{Ll})(\P{Ll})",
				"$1 $2"
			);
		}
	}
}