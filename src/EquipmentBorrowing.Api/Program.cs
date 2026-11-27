using System.Text.Json.Serialization;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Contracts;
using EquipmentBorrowing.Infrastructure.Persistence;
using EquipmentBorrowing.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow);
builder.Services.Configure<ApiBehaviorOptions>(o => o.InvalidModelStateResponseFactory = _ => new BadRequestObjectResult(new ApiError("invalid_request", "Check the JSON fields, required values, and date format (YYYY-MM-DD).")));
builder.Services.AddOpenApi();
builder.Services.AddDbContextFactory<EquipmentBorrowingDbContext>(o => o.UseNpgsql(EquipmentBorrowingDatabase.GetConnectionString()));
builder.Services.AddTransient<IStudentRepository, EfStudentRepository>();
builder.Services.AddTransient<IEquipmentRepository, EfEquipmentRepository>();
builder.Services.AddTransient<IBorrowingRepository, EfBorrowingRepository>();
builder.Services.AddTransient<StudentManagementService>();
builder.Services.AddTransient<EquipmentManagementService>();
builder.Services.AddTransient<BorrowingLookupService>();
builder.Services.AddTransient<ApiBorrowingService>();
var app = builder.Build();
app.Use(async (context, next) => {
    try { await next(); }
    catch (Exception ex) {
        var pg = ex as Npgsql.PostgresException ?? ex.InnerException as Npgsql.PostgresException;
        var (status, code, message) = ex switch {
            EquipmentBorrowing.Application.CommitOutcomeUnknownException => (500, "outcome_unknown", "Commit could not be confirmed. Refresh saved state before deciding whether to retry."),
            KeyNotFoundException => (404, "not_found", ex.Message),
            ArgumentException => (400, "invalid_request", ex.Message),
            EquipmentBorrowing.Application.BusinessConflictException => (409, "conflict", ex.Message),
            _ when pg?.SqlState is "23505" or "40001" or "40P01" => (409, "conflict", "The request conflicts with current data. Refresh before retrying."),
            _ => (500, "server_error", "The server could not complete the database operation.")
        };
        app.Logger.LogWarning("Request failed: {Code}; exception type: {Type}", code, ex.GetType().Name);
        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new ApiError(code, message));
    }
});
app.UseStatusCodePages(async ctx => {
    var status = ctx.HttpContext.Response.StatusCode;
    await ctx.HttpContext.Response.WriteAsJsonAsync(new ApiError(status == 404 ? "not_found" : "method_not_allowed", status == 404 ? "Route not found." : "HTTP method not allowed."));
});
app.MapControllers();
app.MapOpenApi();
if (args.Contains("--initialize")) {
    using var scope = app.Services.CreateScope();
    await using var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<EquipmentBorrowingDbContext>>().CreateDbContext();
    await DatabaseInitializer.InitializeAsync(db);
    return;
}
app.Run();

