using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Mediators.Interfaces;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using ZEN.Contract.CertificateDto.Response;
using ZEN.Domain.Common.Authenticate;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Application.Usecases.CertificateUC.Queries
{
    public class GetCertificateByIdQuery(string certificate_id) : IQuery<ResCertificateDto>
    {
        public string Certificate_Id = certificate_id;
    }

    public class GetCertificateByIdQueryHandler(
        AppDbContext dbContext,
        IUserIdentifierProvider provider
    ) : IQueryHandler<GetCertificateByIdQuery, ResCertificateDto>
    {
        public async Task<CTBaseResult<ResCertificateDto>> Handle(GetCertificateByIdQuery request, CancellationToken cancellationToken)
        {
            var certificate = await dbContext.Certificates
                .AsNoTracking()
                .Where(x => x.Id == request.Certificate_Id && x.user_id == provider.UserId)
                .Select(x => new ResCertificateDto
                {
                    certificate_id = x.Id,
                    certificate_name = x.certificate_name,
                    user_id = x.user_id
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (certificate == null)
                throw new NotFoundException("Certificate not found or you don't have permission to access it!");

            return new CTBaseResult<ResCertificateDto>(certificate);
        }
    }
}

