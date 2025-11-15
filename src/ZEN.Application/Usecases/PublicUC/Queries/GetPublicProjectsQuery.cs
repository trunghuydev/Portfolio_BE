using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.PublicDto;
using ZEN.Domain.Common.Utils;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.PublicUC.Queries;

public class GetPublicProjectsQuery(string username, int pageIndex, int pageSize) : IQuery<PublicProjectResponseDto>
{
    public string Username = UsernameValidator.Normalize(username);
    public int PageIndex = pageIndex;
    public int PageSize = pageSize;
}

public class GetPublicProjectsQueryHandler(
    AppDbContext dbContext
) : IQueryHandler<GetPublicProjectsQuery, PublicProjectResponseDto>
{
    public async Task<CTBaseResult<PublicProjectResponseDto>> Handle(GetPublicProjectsQuery request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Where(u => u.username != null && u.username.ToLower() == request.Username && u.is_public)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("Portfolio not found");
        }

        var query = dbContext.Projects
            .Where(p => p.UserProjects.Any(up => up.user_id == user.Id))
            .Include(p => p.Teches);

        var totalItem = await query.CountAsync(cancellationToken);

        var projects = await query
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PublicProjectDto
            {
                project_id = p.Id,
                project_name = p.project_name,
                description = p.description,
                project_type = p.project_type,
                is_Reality = p.is_Reality,
                duration = p.duration,
                from = p.from,
                to = p.to,
                url_project = p.url_project,
                url_demo = p.url_demo,
                url_github = p.url_github,
                img_url = p.img_url,
                url_contract = p.url_contract,
                url_excel = p.url_excel,
                teches = p.Teches.Select(t => new PublicTechDto
                {
                    tech_name = t.tech_name ?? ""
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        var response = new PublicProjectResponseDto
        {
            total_item = totalItem,
            data = projects
        };

        return new CTBaseResult<PublicProjectResponseDto>(response);
    }
}

