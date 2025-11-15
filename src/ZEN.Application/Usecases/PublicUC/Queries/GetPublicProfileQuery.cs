using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.PublicDto;
using ZEN.Domain.Common.Utils;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.PublicUC.Queries;

public class GetPublicProfileQuery(string username) : IQuery<PublicProfileDto>
{
    public string Username = UsernameValidator.Normalize(username);
}

public class GetPublicProfileQueryHandler(
    AppDbContext dbContext
) : IQueryHandler<GetPublicProfileQuery, PublicProfileDto>
{
    public async Task<CTBaseResult<PublicProfileDto>> Handle(GetPublicProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Where(u => u.username != null && u.username.ToLower() == request.Username)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("Portfolio not found");
        }

        if (!user.is_public)
        {
            throw new UnauthorizedAccessException("This portfolio is private");
        }

        var certificates = await dbContext.Certificates
            .Where(c => c.user_id == user.Id)
            .Select(c => new PublicCertificateDto
            {
                id = c.Id,
                certificate_name = c.certificate_name ?? ""
            })
            .ToListAsync(cancellationToken);

        var profile = new PublicProfileDto
        {
            username = user.username ?? "",
            fullname = user.fullname,
            email = user.Email,
            phone_number = user.phone_number,
            address = user.address,
            position_career = user.position_career,
            background = user.background,
            mindset = user.mindset,
            avatar = user.avatar,
            github = user.github,
            linkedin_url = user.linkedin_url,
            facebook_url = user.facebook_url,
            university_name = user.university_name,
            gpa = user.GPA,
            expOfYear = user.expOfYear,
            dob = user.dob,
            certificates = certificates
        };

        return new CTBaseResult<PublicProfileDto>(profile);
    }
}

