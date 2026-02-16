using System.Globalization;
using System.Text.RegularExpressions;
using Ardalis.GuardClauses;
using SkyTower.Application.MesoscaleDiscussions;
using SkyTower.Domain.MesoscaleDiscussions;

namespace SkyTower.Infrastructure.MesoscaleDiscussions;

public partial class MesoscaleDiscussionService : IMesoscaleDiscussionService
{
	public Task FetchNewDiscussions()
	{
		throw new NotImplementedException();
	}

	public Task<MesoscaleDiscussionSummary> GetDiscussion(int year, int number)
	{
		throw new NotImplementedException();
	}

	public MesoscaleDiscussionDetails ParseDiscussionText(string rawText)
	{
		var toParse = Guard.Against.NullOrWhiteSpace(rawText).Trim();
		var paragraphs = ParagraphSplit().Split(toParse).Select(s => s.Trim()).ToList();

		if (paragraphs.Count < 3)
		{
			throw new FormatException("Discussion text must contain at least three paragraphs.");
		}

		var issuedDate = ParseIssued(paragraphs[0].Trim());

		// validity period: ddhhmmZ - ddhhmmZ

		var details = new MesoscaleDiscussionDetails
		{
			Issued = issuedDate,
			ValidityPeriod = ParseValidityPeriod(paragraphs, issuedDate)
		};

		return details;
	}

	private static DateTimeOffset ParseIssued(string header)
	{
		var headerLines = header.Split('\n').Select(line => line.Trim()).ToArray();
		var issuedLine = headerLines[2];
		issuedLine = issuedLine.Replace(" CST ", " -06:00 ", StringComparison.Ordinal)
			.Replace(" CDT ", " -05:00 ", StringComparison.Ordinal);

		// 0919 AM CST Sun Jan 11 2026
		var issued = DateTimeOffset.ParseExact(issuedLine, "hhmm tt K ddd MMM dd yyyy", DateTimeFormatInfo.InvariantInfo);

		return issued;
	}

	private static ValidityPeriod ParseValidityPeriod(List<string> paragraphs, DateTimeOffset issued)
	{
		var validityLine = paragraphs.FirstOrDefault(p => p.StartsWith("Valid ", StringComparison.OrdinalIgnoreCase));
		if (validityLine == null)
		{
			throw new FormatException("Discussion text must contain a paragraph starting with 'Valid '.");
		}

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
}