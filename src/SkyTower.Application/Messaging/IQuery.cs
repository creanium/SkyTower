namespace SkyTower.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
