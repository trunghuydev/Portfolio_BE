using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.ProfileDto;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Definition;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.UserUC.Commands;

public class UpdateVisibilityCommand(UpdateVisibilityDto dto) : ICommand<OkResponse>
{
    public UpdateVisibilityDto Dto = dto;
}

public class UpdateVisibilityCommandHandler(
    IUnitOfWork unitOfWork,
    IUserIdentifierProvider provider,
    AppDbContext dbContext
) : ICommandHandler<UpdateVisibilityCommand, OkResponse>
{
    public async Task<CTBaseResult<OkResponse>> Handle(UpdateVisibilityCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == provider.UserId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        user.is_public = request.Dto.is_public;

        if (await unitOfWork.SaveChangeAsync(cancellationToken) > 0)
        {
            return new CTBaseResult<OkResponse>(
                new OkResponse($"Visibility updated successfully. Portfolio is now {(request.Dto.is_public ? "public" : "private")}")
            );
        }

        return CTBaseResult.ErrorServer(CTErrors.FAIL_TO_SAVE);
    }
}

