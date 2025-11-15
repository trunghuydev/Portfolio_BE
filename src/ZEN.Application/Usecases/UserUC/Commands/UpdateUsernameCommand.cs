using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.ProfileDto;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Common.Utils;
using ZEN.Domain.Definition;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.UserUC.Commands;

public class UpdateUsernameCommand(UpdateUsernameDto dto) : ICommand<OkResponse>
{
    public UpdateUsernameDto Dto = dto;
}

public class UpdateUsernameCommandHandler(
    IUnitOfWork unitOfWork,
    IUserIdentifierProvider provider,
    AppDbContext dbContext
) : ICommandHandler<UpdateUsernameCommand, OkResponse>
{
    public async Task<CTBaseResult<OkResponse>> Handle(UpdateUsernameCommand request, CancellationToken cancellationToken)
    {
        var normalizedUsername = UsernameValidator.Normalize(request.Dto.username);

        // Validate username
        if (!UsernameValidator.IsValid(normalizedUsername))
        {
            throw new BadHttpRequestException("Username must be 3-30 characters, only a-z, 0-9, -, _ allowed");
        }

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == provider.UserId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        // Check if username already exists
        var existingUser = await dbContext.Users
            .FirstOrDefaultAsync(u => u.username != null && u.username.ToLower() == normalizedUsername && u.Id != provider.UserId, cancellationToken);

        if (existingUser != null)
        {
            throw new BadHttpRequestException("Username already taken");
        }

        // Check username change limit (3 times per year)
        var now = DateTime.UtcNow;
        var oneYearAgo = now.AddYears(-1);

        if (user.last_username_change_date.HasValue && user.last_username_change_date.Value >= oneYearAgo)
        {
            // Check if changed more than 3 times in the last year
            if (user.username_changed_count >= 3)
            {
                throw new BadHttpRequestException("You can only change username 3 times per year");
            }

            // Check if changed within 30 days
            var daysSinceLastChange = (now - user.last_username_change_date.Value).TotalDays;
            if (daysSinceLastChange < 30)
            {
                throw new BadHttpRequestException("You can only change username once every 30 days");
            }
        }
        else
        {
            // Reset counter if last change was more than a year ago
            user.username_changed_count = 0;
        }

        // Update username
        user.username = normalizedUsername;
        user.slug = UsernameValidator.GenerateSlug(normalizedUsername);
        user.username_changed_count++;
        user.last_username_change_date = now;

        if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
        {
            return new CTBaseResult<OkResponse>(new OkResponse($"Username updated successfully to {normalizedUsername}"));
        }

        return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
    }
}

