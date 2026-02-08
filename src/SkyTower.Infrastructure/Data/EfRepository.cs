using SkyTower.Domain.Abstractions;
using Ardalis.Specification.EntityFrameworkCore;

namespace SkyTower.Infrastructure.Data;

public class EfRepository<T>(AppDbContext dbContext) : RepositoryBase<T>(dbContext)
	where T : Entity<T>, IAggregateRoot;