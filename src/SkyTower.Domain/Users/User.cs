using System.Net.Mail;
using SkyTower.Domain.Abstractions;

namespace SkyTower.Domain.Users;

public sealed class User : Entity<User>, IAggregateRoot
{
	public MailAddress Email { get; private set; }

	public string Username { get; private set; }

	public string FirstName { get; private set; }
	
	public string LastName { get; private set; }
}