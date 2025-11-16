using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.CertificateDto.Response;
using ZEN.Domain.Common.Authenticate;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.CertificateUC.Queries
{
    public class GetAllCertificatesQuery : IQuery<List<ResCertificateDto>>
    {
    }

    public class GetAllCertificatesQueryHandler(
        AppDbContext dbContext,
        IUserIdentifierProvider provider
    ) : IQueryHandler<GetAllCertificatesQuery, List<ResCertificateDto>>
    {
        public async Task<CTBaseResult<List<ResCertificateDto>>> Handle(GetAllCertificatesQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await dbContext.Users
                .AsNoTracking()
                .Where(x => x.Id == provider.UserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentUser is null)
                throw new NotFoundException("Current user not found!");

            var certificates = await dbContext.Certificates
                .AsNoTracking()
                .Where(x => x.user_id == provider.UserId)
                .Select(x => new ResCertificateDto
                {
                    certificate_id = x.Id,
                    certificate_name = x.certificate_name,
                    user_id = x.user_id
                })
                .ToListAsync(cancellationToken);

            return new CTBaseResult<List<ResCertificateDto>>(certificates);
        }
    }
}

