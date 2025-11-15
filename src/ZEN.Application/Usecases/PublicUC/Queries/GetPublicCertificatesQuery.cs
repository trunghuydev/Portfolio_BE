using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.PublicDto;
using ZEN.Domain.Common.Utils;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.PublicUC.Queries;

public class GetPublicCertificatesQuery(string username) : IQuery<List<PublicCertificateDto>>
{
    public string Username = UsernameValidator.Normalize(username);
}

public class GetPublicCertificatesQueryHandler(
    AppDbContext dbContext
) : IQueryHandler<GetPublicCertificatesQuery, List<PublicCertificateDto>>
{
    public async Task<CTBaseResult<List<PublicCertificateDto>>> Handle(GetPublicCertificatesQuery request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Where(u => u.username != null && u.username.ToLower() == request.Username && u.is_public)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("Portfolio not found");
        }

        var certificates = await dbContext.Certificates
            .Where(c => c.user_id == user.Id)
            .Select(c => new PublicCertificateDto
            {
                id = c.Id,
                certificate_name = c.certificate_name ?? ""
            })
            .ToListAsync(cancellationToken);

        return new CTBaseResult<List<PublicCertificateDto>>(certificates);
    }
}

