using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.WEDTO.Response;
using ZEN.Domain.Common.Authenticate;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.WorkExperienceUC.Queries
{
    public class GetWorkExperienceByIdQuery(string we_id) : IQuery<ResWEDto>
    {
        public string We_Id = we_id;
    }

    public class GetWorkExperienceByIdQueryHandler(
        AppDbContext dbContext,
        IUserIdentifierProvider provider
    ) : IQueryHandler<GetWorkExperienceByIdQuery, ResWEDto>
    {
        public async Task<CTBaseResult<ResWEDto>> Handle(GetWorkExperienceByIdQuery request, CancellationToken cancellationToken)
        {
            var workExperience = await dbContext.WorkExperiences
                .AsNoTracking()
                .Include(x => x.MyTasks)
                .Where(x => x.Id == request.We_Id && x.user_id == provider.UserId)
                .Select(x => new ResWEDto
                {
                    we_id = x.Id,
                    user_id = x.user_id,
                    company_name = x.company_name,
                    position = x.position,
                    duration = x.duration,
                    description = x.description,
                    project_id = x.project_id,
                    tasks = x.MyTasks.Select(t => new ResMyTask(t.Id, t.task_description)).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (workExperience == null)
                throw new NotFoundException("Work experience not found or you don't have permission to access it!");

            return new CTBaseResult<ResWEDto>(workExperience);
        }
    }
}

