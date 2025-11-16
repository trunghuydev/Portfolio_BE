using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.SkillDto.Response;
using ZEN.Domain.Common.Authenticate;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.SkillUC.Queries
{
    public class GetSkillByIdQuery(string skill_id) : IQuery<ResSkillDto>
    {
        public string Skill_Id = skill_id;
    }

    public class GetSkillByIdQueryHandler(
        IUserIdentifierProvider provider,
        AppDbContext dbContext
    ) : IQueryHandler<GetSkillByIdQuery, ResSkillDto>
    {
        public async Task<CTBaseResult<ResSkillDto>> Handle(GetSkillByIdQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await dbContext.Users
                .AsNoTracking()
                .Where(x => x.Id == provider.UserId)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (currentUser is null) 
                throw new NotFoundException("Current user not found!");

            // Kiểm tra skill thuộc về user hiện tại
            var userSkill = await dbContext.UserSkills
                .AsNoTracking()
                .Include(x => x.Skill)
                .Where(x => x.user_id == currentUser.Id && x.skill_id == request.Skill_Id)
                .Select(x => new ResSkillDto
                {
                    skill_id = x.skill_id,
                    skill_name = x.Skill.skill_name,
                    position = x.Skill.position
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (userSkill == null)
                throw new NotFoundException("Skill not found or you don't have permission to access it!");

            return new CTBaseResult<ResSkillDto>(userSkill);
        }
    }
}

