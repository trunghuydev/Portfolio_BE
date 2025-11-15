using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.PublicDto;
using ZEN.Domain.Common.Utils;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.PublicUC.Queries;

public class CheckUsernameQuery(string username) : IQuery<CheckUsernameResponseDto>
{
    public string Username = UsernameValidator.Normalize(username);
}

public class CheckUsernameQueryHandler(
    AppDbContext dbContext
) : IQueryHandler<CheckUsernameQuery, CheckUsernameResponseDto>
{
    public async Task<CTBaseResult<CheckUsernameResponseDto>> Handle(CheckUsernameQuery request, CancellationToken cancellationToken)
    {
        if (!UsernameValidator.IsValid(request.Username))
        {
            return new CTBaseResult<CheckUsernameResponseDto>(
                new CheckUsernameResponseDto
                {
                    available = false,
                    message = "Username is invalid. Must be 3-30 characters, only a-z, 0-9, -, _ allowed"
                }
            );
        }

        var exists = await dbContext.Users
            .AnyAsync(u => u.username != null && u.username.ToLower() == request.Username, cancellationToken);

        return new CTBaseResult<CheckUsernameResponseDto>(
            new CheckUsernameResponseDto
            {
                available = !exists,
                message = exists ? "Username already taken" : "Username is available"
            }
        );
    }
}

