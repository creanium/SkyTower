using MediatR;

namespace SkyTower.UseCases.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
