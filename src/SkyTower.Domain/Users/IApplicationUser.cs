namespace SkyTower.Domain.Users;

public interface IApplicationUser
{
	string UserName { get; }
	string Email { get; }
	string Name { get; }
	Uri? Picture { get; }
}