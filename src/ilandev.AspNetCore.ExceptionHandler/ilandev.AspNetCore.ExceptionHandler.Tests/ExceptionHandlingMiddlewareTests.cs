using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ilandev.AspNetCore.ExceptionHandler.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ilandev.AspNetCore.ExceptionHandler.Tests;

public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task NoHandlers_ShouldReturnDefaultResponse()
    {
        using var host = await GetHost();
        using var client = host.GetTestClient();

        using var response = await client.GetAsync("/");

        response.Should()
                .HaveStatusCode(HttpStatusCode.InternalServerError);

        var responseContent = await response.Content.ReadAsStringAsync();

        responseContent.Should()
                       .BeNullOrEmpty();
    }

    [Fact]
    public async Task NoHandlers_DevelopmentMode_ShouldReturnDefaultResponseWithExceptionDetails()
    {
        const string ExpectedMessage = "This is an exception";

        using var host = await GetHost(
            o => o.WriteExceptionDetails(true),
            e => e.Map("/", _ => throw new Exception(ExpectedMessage))
        );
        using var client = host.GetTestClient();

        using var response = await client.GetAsync("/");

        response.Should()
                .HaveStatusCode(HttpStatusCode.InternalServerError);

        var responseContent = await response.Content.ReadFromJsonAsync<ExceptionDetailsResponse>();

        responseContent.Should()
                       .NotBeNull();

        responseContent!.Message
                        .Should()
                        .Be(ExpectedMessage);
    }

    [Fact]
    public async Task HandlerRegistered_ResultNull_ShouldWriteDefaultResponse()
    {
        using var host = await GetHost(c => c.AddHandler<Exception>(_ => null!));
        using var client = host.GetTestClient();

        using var response = await client.GetAsync("/");

        response.Should()
                .HaveStatusCode(HttpStatusCode.InternalServerError);

        var responseContent = await response.Content.ReadAsStringAsync();

        responseContent.Should()
                       .BeNullOrEmpty();
    }

    [Fact]
    public async Task HandlerRegistered_ResponseNull_ShouldSkipWritingResponse()
    {
        const HttpStatusCode ExpectedStatusCode = HttpStatusCode.Conflict;

        using var host = await GetHost(
            c => c.AddHandler<Exception>(
                _ => new()
                {
                    Response = null,
                    StatusCode = ExpectedStatusCode
                }
            )
        );
        using var client = host.GetTestClient();

        using var response = await client.GetAsync("/");

        response.Should()
                .HaveStatusCode(ExpectedStatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();

        responseContent.Should()
                       .BeNullOrEmpty();
    }

    [Fact]
    public async Task HandlerRegistered_ResultHasValue_ShouldWriteValueToResponse()
    {
        const HttpStatusCode ExpectedStatusCode = HttpStatusCode.Conflict;
        const string ExpectedResponse = "Test";

        using var host = await GetHost(
            c => c.AddHandler<Exception>(
                _ => new()
                {
                    Response = ExpectedResponse,
                    StatusCode = ExpectedStatusCode
                }
            )
        );
        using var client = host.GetTestClient();

        using var response = await client.GetAsync("/");

        response.Should()
                .HaveStatusCode(ExpectedStatusCode);

        var responseContent = await response.Content.ReadFromJsonAsync<string>();

        responseContent.Should()
                       .Be(ExpectedResponse);
    }

    [Fact]
    public async Task HandlerRegistered_TaskCanceledException_ShouldIgnoreByDefault()
    {
        using var host = await GetHost(endpoints: e => e.Map("/", _ => throw new TaskCanceledException()));
        using var client = host.GetTestClient();

        using var response = await client.GetAsync("/");

        response.Should()
                .HaveStatusCode(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();

        responseContent.Should()
                       .BeNullOrEmpty();
    }

    [Fact]
    public async Task HandlerRegistered_ExceptionInheritanceDisabled_ShouldWriteDefaultResponse()
    {
        using var host = await GetHost(
            c => c
                 .AllowExceptionInheritance(false)
                 .AddHandler<Exception>(
                     _ => new()
                     {
                         Response = null,
                         StatusCode = HttpStatusCode.OK
                     }
                 ),
            e => e.Map("/", _ => throw new InvalidOperationException())
        );

        using var client = host.GetTestClient();

        using var response = await client.GetAsync("/");

        response.Should()
                .HaveStatusCode(HttpStatusCode.InternalServerError);

        var responseContent = await response.Content.ReadAsStringAsync();

        responseContent.Should()
                       .BeNullOrEmpty();
    }

    [Fact]
    public async Task HandlerRegistered_ResponseStarted_ShouldSkipHandling()
    {
        using var host = await GetHost(
            c => c
                .AddHandler<Exception>(
                    _ => new()
                    {
                        Response = null,
                        StatusCode = HttpStatusCode.OK
                    }
                ),
            e => e.Map(
                "/",
                async context =>
                {
                    context.Response.StatusCode = (int) HttpStatusCode.BadGateway;
                    await context.Response.WriteAsync("boo");

                    throw new InvalidOperationException();
                }
            )
        );

        using var client = host.GetTestClient();

        using var response = await client.GetAsync("/");

        response.Should()
                .HaveStatusCode(HttpStatusCode.BadGateway);
    }

    private static Task<IHost> GetHost(
        Action<ExceptionHandlingOptions>? options = default,
        Action<IEndpointRouteBuilder>? endpoints = default,
        Action<IApplicationBuilder>? configureApp = default
    )
    {
        return new HostBuilder()
               .ConfigureWebHost(
                   builder =>
                   {
                       static void ConfigureDefaultEndpoints(IEndpointRouteBuilder endpoints)
                       {
                           endpoints.Map(
                               "/",
                               _ => throw new Exception("Test exception")
                           );
                       }

                       builder.UseTestServer()
                              .ConfigureServices(
                                  svc =>
                                  {
                                      svc.AddRouting();

                                      svc.AddExceptionHandlingMiddleware(options);
                                  }
                              )
                              .Configure(
                                  app =>
                                  {
                                      app.UseRouting();

                                      app.UseExceptionHandlingMiddleware();

                                      configureApp?.Invoke(app);

                                      app.UseEndpoints(endpoints ?? ConfigureDefaultEndpoints);
                                  }
                              );
                   }
               )
               .StartAsync();
    }
}