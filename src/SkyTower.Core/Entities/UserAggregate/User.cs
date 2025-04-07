using System.Net.Mail;
using SkyTower.Core.Abstractions;
using SkyTower.Core.Interfaces;

namespace SkyTower.Core.Entities.UserAggregate;

public sealed class User : Entity<User>, IAggregateRoot
{
	public MailAddress Email { get; private set; }

	public string Username { get; private set; }

	public string FirstName { get; private set; }
	
	public string LastName { get; private set; }
}