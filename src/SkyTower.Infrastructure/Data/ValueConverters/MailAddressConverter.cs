using System.Net.Mail;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkyTower.Domain.Extensions;

namespace SkyTower.Infrastructure.Data.ValueConverters;

public class MailAddressConverter() : ValueConverter<MailAddress, string>(
	v => (v.ToString().HasValue() ? v.ToString() : null) ?? string.Empty,
	v => new MailAddress(v)
);