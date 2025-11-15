using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.PublicDto;
using ZEN.Domain.Common.Utils;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.PublicUC.Queries;

public class GetPublicWorkExpQuery(string username, int pageIndex, int pageSize) : IQuery<PublicWorkExpResponseDto>
{
    public string Username = UsernameValidator.Normalize(username);
    public int PageIndex = pageIndex;
    public int PageSize = pageSize;
}

public class GetPublicWorkExpQueryHandler(
    AppDbContext dbContext
) : IQueryHandler<GetPublicWorkExpQuery, PublicWorkExpResponseDto>
{
    public async Task<CTBaseResult<PublicWorkExpResponseDto>> Handle(GetPublicWorkExpQuery request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Where(u => u.username != null && u.username.ToLower() == request.Username && u.is_public)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("Portfolio not found");
        }

        var query = dbContext.WorkExperiences
            .Where(we => we.user_id == user.Id)
            .Include(we => we.MyTasks);

        var totalItem = await query.CountAsync(cancellationToken);

        var workExps = await query
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(we => new PublicWorkExpDto
            {
                we_id = we.Id,
                company_name = we.company_name,
                position = we.position,
                duration = we.duration,
                description = we.description,
                project_id = we.project_id,
                tasks = we.MyTasks.Select(t => new PublicTaskDto
                {
                    mt_id = t.Id,
                    task_description = t.task_description
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        var response = new PublicWorkExpResponseDto
        {
            total_item = totalItem,
            data = workExps
        };

        return new CTBaseResult<PublicWorkExpResponseDto>(response);
    }
}

