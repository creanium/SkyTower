using System.Collections.Immutable;
using System.Globalization;
using System.Text.RegularExpressions;
using Ardalis.GuardClauses;
using NetTopologySuite.Geometries;
using SkyTower.Domain.Extensions;
using SkyTower.Domain.MesoscaleDiscussions;

namespace SkyTower.Infrastructure.MesoscaleDiscussions;

public partial class MesoscaleDiscussionParser
{
	private readonly IReadOnlyList<string> _paragraphs;

	public MesoscaleDiscussionParser(string rawText)
	{
		Guard.Against.NullOrWhiteSpace(rawText);
		_paragraphs = ParseParagraphs(rawText);

		if (_paragraphs.Count < 3)
		{
			throw new FormatException("Discussion text must contain at least three paragraphs.");
		}
	}

	private static IReadOnlyList<string> ParseParagraphs(string toParse)
	{
		var paragraphs = ParagraphSplit().Split(toParse).Select(p => p.Trim()).Where(p => p.HasValue()).ToArray();
		var header = string.Join("\n", paragraphs[0].Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(line => line.Trim()));
		var contentParagraphs = paragraphs.Skip(1)
			.Select(p => string.Join(" ", p.Split('\n').Select(line => line.Trim())));

		return [header, ..contentParagraphs];
	}

	public DateTimeOffset ParseIssued()
	{
		var header = _paragraphs[0];
		var headerText = Guard.Against.NullOrWhiteSpace(header);
		var headerLines = headerText.Split('\n').Select(line => line.Trim()).ToArray();
		var issuedLine = headerLines[2];
		issuedLine = issuedLine.Replace(" CST ", " -06:00 ", StringComparison.Ordinal)
			.Replace(" CDT ", " -05:00 ", StringComparison.Ordinal);

		// 0919 AM CST Sun Jan 11 2026
		var issued = DateTimeOffset.ParseExact(issuedLine, "hhmm tt K ddd MMM dd yyyy", DateTimeFormatInfo.InvariantInfo);

		return issued;
	}

	public string? ParseAreasAffected()
	{
		const string linePrefix = "Areas affected...";
		var areasAffectedLine = _paragraphs.FirstOrDefault(p => p.StartsWith(linePrefix, StringComparison.OrdinalIgnoreCase));

		return areasAffectedLine?[linePrefix.Length..]?.Trim();
	}

	public string? ParseConcerning()
	{
		const string linePrefix = "Concerning...";
		var concerningLine = _paragraphs.FirstOrDefault(p => p.StartsWith(linePrefix, StringComparison.OrdinalIgnoreCase));

		return concerningLine?[linePrefix.Length..]?.Trim();
	}

	public int? ParseWatchProbability()
	{
		const string linePrefix = "Probability of Watch Issuance...";
		var watchProbabilityLine = _paragraphs.FirstOrDefault(p => p.StartsWith(linePrefix, StringComparison.OrdinalIgnoreCase));

		if (watchProbabilityLine == null)
		{
			return null;
		}

		var probabilityText = watchProbabilityLine[linePrefix.Length..].Trim();
		var probabilityString = probabilityText.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0].Trim();
		if (int.TryParse(probabilityString, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var probability))
		{
			return probability;
		}

		return null;
	}

	public string? ParseHeadline()
	{
		var index = _paragraphs.FindIndex(p => p.StartsWith("Valid ", StringComparison.OrdinalIgnoreCase));
		var targetIndex = index + 1;

		if (index < 0)
		{
			index = _paragraphs.FindIndex(p => p.StartsWith("SUMMARY...", StringComparison.OrdinalIgnoreCase));
			targetIndex = index - 1;
		}

		if (
			_paragraphs[targetIndex].StartsWith("SUMMARY...", StringComparison.OrdinalIgnoreCase)
			|| _paragraphs[targetIndex].StartsWith("Valid ", StringComparison.OrdinalIgnoreCase)
			|| _paragraphs[targetIndex].StartsWith("Probability of Watch Issuance...", StringComparison.OrdinalIgnoreCase)
		)
		{
			return null;
		}

		return _paragraphs[targetIndex];
	}

	public string? ParseSummary()
	{
		const string linePrefix = "SUMMARY...";
		var summaryLine = _paragraphs.FirstOrDefault(p => p.StartsWith(linePrefix, StringComparison.OrdinalIgnoreCase));

		return summaryLine?[linePrefix.Length..]?.Trim();
	}

	public IReadOnlyList<string>? ParseDiscussion()
	{
		const string linePrefix = "DISCUSSION...";
		var discussionIndex = _paragraphs.FindIndex(p => p.StartsWith(linePrefix, StringComparison.OrdinalIgnoreCase));

		if (discussionIndex < 0)
		{
			return null;
		}

		var discussionLines = _paragraphs.Skip(discussionIndex)
			.TakeWhile(p => !p.StartsWith("..", StringComparison.OrdinalIgnoreCase))
			.ToList();

		discussionLines[0] = discussionLines[0][linePrefix.Length..].Trim();

		return discussionLines.AsReadOnly();
	}

	public LinearRing ParseBoundary()
	{
		const string linePrefix = "LAT...LON";
		var boundaryLine = _paragraphs.FirstOrDefault(p => p.StartsWith(linePrefix, StringComparison.OrdinalIgnoreCase))
		                   ?? throw new FormatException("Boundary information not found in the discussion text.");

		var coordinatePairsText = boundaryLine[linePrefix.Length..].Trim();
		var coordinatePairs = coordinatePairsText.ReplaceLineEndings(" ")
			.Split(' ', StringSplitOptions.RemoveEmptyEntries)
			.Where(s => s.HasValue())
			.ToImmutableList();

		var coordinates = new List<Coordinate>(coordinatePairs.Count);

		foreach (var coordinate in coordinatePairs)
		{
			var parts = CoordinatePairRegex().Match(coordinate).Groups;
			if (parts.Count != 5)
			{
				throw new FormatException($"Invalid coordinate pair '{coordinate}'. Expected format is 'ddmmddmm' (latitude and longitude in degrees and minutes).");
			}

			var latString = $"{parts[1].Value}.{parts[2].Value}";
			var lonString = $"{parts[3].Value}.{parts[4].Value}";

			if (!double.TryParse(latString, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var latitude) ||
			    !double.TryParse(lonString, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var longitude))
			{
				throw new FormatException($"Invalid latitude or longitude in coordinate pair '{latString} {lonString}'.");
			}

			if (longitude < 50.0)
			{
				longitude += 100.0;
			}

			longitude = Math.Round(-longitude, 2, MidpointRounding.AwayFromZero);
			latitude = Math.Round(latitude, 2, MidpointRounding.AwayFromZero);

			coordinates.Add(new Coordinate(longitude, latitude));
		}

		return new LinearRing([.. coordinates]);
	}

	public ValidityPeriod ParseValidityPeriod(DateTimeOffset issued)
	{
		var validityLine = _paragraphs.FirstOrDefault(p => p.StartsWith("Valid ", StringComparison.OrdinalIgnoreCase))
		                   ?? throw new FormatException("Discussion text must contain a paragraph starting with 'Valid '.");

		// Valid 171455Z - 171800Z
		var match = ValidityLineRegex().Match(validityLine);
		if (!match.Success)
		{
			throw new FormatException("Validity line is not in the expected format 'Valid ddhhmmZ - ddhhmmZ'.");
		}

		var startYear = issued.Year;
		var startMonth = issued.Month;
		var startDay = Guard.Against.OutOfRange(int.Parse(match.Groups[1].Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo), nameof(validityLine), 1, 31);
		var startTime = TimeOnly.ParseExact(match.Groups[2].Value, "HHmm", DateTimeFormatInfo.InvariantInfo);

		var endMonth = issued.Month;
		var endYear = issued.Year;
		var endDay = Guard.Against.OutOfRange(int.Parse(match.Groups[3].Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo), nameof(validityLine), 1, 31);
		var endTime = TimeOnly.ParseExact(match.Groups[4].Value, "HHmm", DateTimeFormatInfo.InvariantInfo);

		// Issued date is usually in the local time zone, validity period is always in UTC.
		// If the start day of the validity period is less than the issued day,
		// then we assume the validity period extends into the next month.
		if (startDay < issued.Day)
		{
			startMonth = (startMonth % 12) + 1; // Wrap around to January if we're in December
			endMonth = startMonth;

			if (startMonth == 1)
			{
				startYear += 1; // If we wrapped around to January, we also need to increment the year
				endYear += 1;
			}
		}

		if (endDay < startDay)
		{
			// If the end day is less than the start day, we assume the validity period extends into the next month
			endMonth = (endMonth % 12) + 1; // Wrap around to January if we're in December
			if (endMonth == 1)
			{
				endYear += 1; // If we wrapped around to January, we also need to increment the year
			}
		}

		var validityStart = new DateTimeOffset(new DateOnly(startYear, startMonth, startDay), startTime, TimeSpan.Zero);
		var validityEnd = new DateTimeOffset(new DateOnly(endYear, endMonth, endDay), endTime, TimeSpan.Zero);

		return new ValidityPeriod(validityStart, validityEnd);
	}

	[GeneratedRegex(@"\n(\s+)?\n")]
	private static partial Regex ParagraphSplit();

	[GeneratedRegex(@"Valid (\d{2})(\d{4})Z - (\d{2})(\d{4})Z")]
	private static partial Regex ValidityLineRegex();

	[GeneratedRegex(@"(\d{2})(\d{2})(\d{2})(\d{2})")]
	private static partial Regex CoordinatePairRegex();
}