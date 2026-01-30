using MediatR;

namespace SkyTower.UseCases.Messaging;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>, IRequest<Result<TResponse>>
{
}
