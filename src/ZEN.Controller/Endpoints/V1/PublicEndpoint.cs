using Asp.Versioning.Builder;
using CTCore.DynamicQuery.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ZEN.Application.Usecases.PublicUC.Queries;
using ZEN.Controller.Extensions;

namespace ZEN.Controller.Endpoints.V1;

public class PublicEndpoint : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints, ApiVersionSet version)
    {
        var publicGroup = endpoints
            .MapGroup($"{EndpointExntensions.BASE_ROUTE}/public")
            .WithDisplayName("Public API")
            .WithApiVersionSet(version)
            .HasApiVersion(1);

        // Public profile endpoints
        publicGroup.MapGet("/profile/{username}", GetPublicProfile);
        publicGroup.MapGet("/profile/{username}/workexp", GetPublicWorkExp);
        publicGroup.MapGet("/profile/{username}/projects", GetPublicProjects);
        publicGroup.MapGet("/profile/{username}/skills", GetPublicSkills);
        publicGroup.MapGet("/profile/{username}/certificates", GetPublicCertificates);
        publicGroup.MapGet("/check-username/{username}", CheckUsername);

        return endpoints;
    }

    private async Task<IResult> GetPublicProfile(
        [FromServices] IMediator mediator,
        [FromRoute] string username
    )
    {
        try
        {
            return (await mediator.Send(new GetPublicProfileQuery(username))).ToOk(e => Results.Ok(e));
        }
        catch (NotFoundException ex)
        {
            return Results.Json(new { error = "USER_NOT_FOUND", message = ex.Message }, statusCode: 404);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Results.Json(new { error = "PORTFOLIO_PRIVATE", message = ex.Message }, statusCode: 403);
        }
    }

    private async Task<IResult> GetPublicWorkExp(
        [FromServices] IMediator mediator,
        [FromRoute] string username,
        [FromQuery] int page_index = 1,
        [FromQuery] int page_size = 10
    )
    {
        try
        {
            return (await mediator.Send(new GetPublicWorkExpQuery(username, page_index, page_size))).ToOk(e => Results.Ok(e));
        }
        catch (NotFoundException ex)
        {
            return Results.Json(new { error = "USER_NOT_FOUND", message = ex.Message }, statusCode: 404);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Results.Json(new { error = "PORTFOLIO_PRIVATE", message = ex.Message }, statusCode: 403);
        }
    }

    private async Task<IResult> GetPublicProjects(
        [FromServices] IMediator mediator,
        [FromRoute] string username,
        [FromQuery] int page_index = 1,
        [FromQuery] int page_size = 10
    )
    {
        try
        {
            return (await mediator.Send(new GetPublicProjectsQuery(username, page_index, page_size))).ToOk(e => Results.Ok(e));
        }
        catch (NotFoundException ex)
        {
            return Results.Json(new { error = "USER_NOT_FOUND", message = ex.Message }, statusCode: 404);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Results.Json(new { error = "PORTFOLIO_PRIVATE", message = ex.Message }, statusCode: 403);
        }
    }

    private async Task<IResult> GetPublicSkills(
        [FromServices] IMediator mediator,
        [FromRoute] string username
    )
    {
        try
        {
            return (await mediator.Send(new GetPublicSkillsQuery(username))).ToOk(e => Results.Ok(e));
        }
        catch (NotFoundException ex)
        {
            return Results.Json(new { error = "USER_NOT_FOUND", message = ex.Message }, statusCode: 404);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Results.Json(new { error = "PORTFOLIO_PRIVATE", message = ex.Message }, statusCode: 403);
        }
    }

    private async Task<IResult> GetPublicCertificates(
        [FromServices] IMediator mediator,
        [FromRoute] string username
    )
    {
        try
        {
            return (await mediator.Send(new GetPublicCertificatesQuery(username))).ToOk(e => Results.Ok(e));
        }
        catch (NotFoundException ex)
        {
            return Results.Json(new { error = "USER_NOT_FOUND", message = ex.Message }, statusCode: 404);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Results.Json(new { error = "PORTFOLIO_PRIVATE", message = ex.Message }, statusCode: 403);
        }
    }

    private async Task<IResult> CheckUsername(
        [FromServices] IMediator mediator,
        [FromRoute] string username
    )
    {
        return (await mediator.Send(new CheckUsernameQuery(username))).ToOk(e => Results.Ok(e));
    }
}

