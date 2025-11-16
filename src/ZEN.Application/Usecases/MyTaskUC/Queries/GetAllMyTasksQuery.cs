using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.MyTaskDto.Response;
using ZEN.Domain.Common.Authenticate;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.MyTaskUC.Queries
{
    public class GetAllMyTasksQuery : IQuery<List<ResMyTaskDto>>
    {
    }

    public class GetAllMyTasksQueryHandler(
        AppDbContext dbContext,
        IUserIdentifierProvider provider
    ) : IQueryHandler<GetAllMyTasksQuery, List<ResMyTaskDto>>
    {
        public async Task<CTBaseResult<List<ResMyTaskDto>>> Handle(GetAllMyTasksQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await dbContext.Users
                .AsNoTracking()
                .Where(x => x.Id == provider.UserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentUser is null)
                throw new NotFoundException("Current user not found!");

            // Lấy tất cả tasks thông qua WorkExperience của user
            var tasks = await dbContext.MyTasks
                .AsNoTracking()
                .Include(x => x.WorkExperience)
                .Where(x => x.WorkExperience != null && x.WorkExperience.user_id == provider.UserId)
                .Select(x => new ResMyTaskDto
                {
                    mt_id = x.Id,
                    we_id = x.we_id,
                    task_description = x.task_description
                })
                .ToListAsync(cancellationToken);

            return new CTBaseResult<List<ResMyTaskDto>>(tasks);
        }
    }
}

