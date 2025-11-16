using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.ProjectDto.Response;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Interfaces;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.ProjectUC.Query
{
    public class GetProjectByIdQuery(string project_id) : IQuery<ResProjectDto>
    {
        public string Project_Id = project_id;
    }
    
    public class GetProjectByIdQueryHandler(
        AppDbContext dbContext,
        IUserIdentifierProvider provider
    ) : IQueryHandler<GetProjectByIdQuery, ResProjectDto>
    {
        public async Task<CTBaseResult<ResProjectDto>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            // Kiểm tra permission
            var hasPermission = await dbContext.UserProjects
                .AnyAsync(x => x.project_id == request.Project_Id
                            && x.user_id == provider.UserId, cancellationToken);

            if (!hasPermission)
            {
                throw new UnauthorizedAccessException("You have no permission!");
            }

            // Lấy project với tech
            var project = await dbContext.Projects
                .AsNoTracking()
                .Include(x => x.Teches)
                .Where(x => x.Id == request.Project_Id)
                .Select(x => new ResProjectDto
                {
                    project_id = x.Id,
                    project_name = x.project_name,
                    description = x.description,
                    img_url = x.img_url,
                    project_type = x.project_type,
                    is_Reality = x.is_Reality,
                    url_project = x.url_project,
                    url_demo = x.url_demo,
                    url_github = x.url_github,
                    duration = x.duration,
                    from = x.from,
                    to = x.to,
                    url_contract = x.url_contract,
                    url_excel = x.url_excel,
                    teches = x.Teches.Select(t => new Contract.ProjectDto.Request.TechDto
                    {
                        tech_name = t.tech_name!
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (project == null)
            {
                throw new NotFoundException("Project not found!");
            }

            return new CTBaseResult<ResProjectDto>(project);
        }
    }
}

