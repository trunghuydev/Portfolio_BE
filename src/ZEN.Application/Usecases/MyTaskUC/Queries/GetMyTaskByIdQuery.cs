using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.MyTaskDto.Response;
using ZEN.Domain.Common.Authenticate;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.MyTaskUC.Queries
{
    public class GetMyTaskByIdQuery(string mt_id) : IQuery<ResMyTaskDto>
    {
        public string Mt_Id = mt_id;
    }

    public class GetMyTaskByIdQueryHandler(
        AppDbContext dbContext,
        IUserIdentifierProvider provider
    ) : IQueryHandler<GetMyTaskByIdQuery, ResMyTaskDto>
    {
        public async Task<CTBaseResult<ResMyTaskDto>> Handle(GetMyTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var task = await dbContext.MyTasks
                .AsNoTracking()
                .Include(x => x.WorkExperience)
                .Where(x => x.Id == request.Mt_Id 
                         && x.WorkExperience != null 
                         && x.WorkExperience.user_id == provider.UserId)
                .Select(x => new ResMyTaskDto
                {
                    mt_id = x.Id,
                    we_id = x.we_id,
                    task_description = x.task_description
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (task == null)
                throw new NotFoundException("Task not found or you don't have permission to access it!");

            return new CTBaseResult<ResMyTaskDto>(task);
        }
    }
}

