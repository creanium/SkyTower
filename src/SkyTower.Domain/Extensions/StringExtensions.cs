using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace SkyTower.Domain.Extensions;

public static partial class StringExtensions
{
	[ContractAnnotation("null => false")]
	public static bool HasValue(this string? value) => !string.IsNullOrWhiteSpace(value);

	[ContractAnnotation("null => true")]
	public static bool IsNullOrWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value);

	[ContractAnnotation("null => true")]
	public static bool IsNullOrEmpty(this string? value) => string.IsNullOrEmpty(value);

	/// <summary>
	/// Given a person's first and last name, we'll make our best guess to extract up to two initials, hopefully
	/// representing their first and last name, skipping any middle initials, Jr/Sr/III suffixes, etc. The letters 
	/// will be returned together in ALL CAPS, e.g. "TW". 
	/// 
	/// The way it parses names for many common styles:
	/// 
	/// Mason Zhwiti                -> MZ
	/// mason lowercase zhwiti      -> MZ
	/// Mason G Zhwiti              -> MZ
	/// Mason G. Zhwiti             -> MZ
	/// John Queue Public           -> JP
	/// John Q. Public, Jr.         -> JP
	/// John Q Public Jr.           -> JP
	/// Thurston Howell III         -> TH
	/// Thurston Howell, III        -> TH
	/// Malcolm X                   -> MX
	/// A Ron                       -> AR
	/// A A Ron                     -> AR
	/// Madonna                     -> M
	/// Chris O'Donnell             -> CO
	/// Malcolm McDowell            -> MM
	/// Robert "Rocky" Balboa, Sr.  -> RB
	/// 1Bobby 2Tables              -> BT
	/// Éric Ígor                   -> ÉÍ
	/// 행운의 복숭아                 -> 행복
	/// 
	/// </summary>
	/// <param name="name">The full name of a person.</param>
	/// <returns>One to two uppercase initials, without punctuation.</returns>
	public static string ExtractInitialsFromName(this string name)
	{
		// first remove all: punctuation, separator chars, control chars, and numbers (unicode style regexes)
		var initials = PunctuationAndNumbersRegex().Replace(name, "");

		// Replacing all possible whitespace/separator characters (unicode style), with a single, regular ascii space.
		initials = WhitespaceNormalizationRegex().Replace(initials, " ");

		// Remove all Sr, Jr, I, II, III, IV, V, VI, VII, VIII, IX at the end of names
		initials = NameSuffixRegex().Replace(initials.Trim(), "");

		// Extract up to 2 initials from the remaining cleaned name.
		initials = TwoCharacterInitialRegex().Replace(initials, "$1$2").Trim();

		if (initials.Length > 2)
		{
			// Worst case scenario, everything failed, just grab the first two letters of what we have left.
			// ReSharper disable once ReplaceSubstringWithRangeIndexer
			initials = initials.Substring(0, 2);
		}

		return initials.ToUpperInvariant();
	}

    [GeneratedRegex(@"[\p{P}\p{S}\p{C}\p{N}]+")]
    private static partial Regex PunctuationAndNumbersRegex();
    [GeneratedRegex(@"\p{Z}+")]
    private static partial Regex WhitespaceNormalizationRegex();
    [GeneratedRegex(@"\s+(?:[JS]R|I{1,3}|I[VX]|VI{0,3})$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex NameSuffixRegex();
    [GeneratedRegex(@"^(\p{L})[^\s]*(?:\s+(?:\p{L}+\s+(?=\p{L}))?(?:(\p{L})\p{L}*)?)?$")]
    private static partial Regex TwoCharacterInitialRegex();
}