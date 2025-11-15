using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.PublicDto;
using ZEN.Domain.Common.Utils;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.PublicUC.Queries;

public class GetPublicSkillsQuery(string username) : IQuery<List<PublicSkillDto>>
{
    public string Username = UsernameValidator.Normalize(username);
}

public class GetPublicSkillsQueryHandler(
    AppDbContext dbContext
) : IQueryHandler<GetPublicSkillsQuery, List<PublicSkillDto>>
{
    public async Task<CTBaseResult<List<PublicSkillDto>>> Handle(GetPublicSkillsQuery request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Where(u => u.username != null && u.username.ToLower() == request.Username && u.is_public)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("Portfolio not found");
        }

        var skills = await dbContext.UserSkills
            .Where(us => us.user_id == user.Id)
            .Include(us => us.Skill)
            .Select(us => new PublicSkillDto
            {
                skill_id = us.Skill.Id,
                skill_name = us.Skill.skill_name,
                position = us.Skill.position
            })
            .ToListAsync(cancellationToken);

        return new CTBaseResult<List<PublicSkillDto>>(skills);
    }
}

