using Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDatabase(builder.Configuration)
    .AddOpenApi()
    .AddCors(options =>
        options.AddDefaultPolicy(policy =>
            policy.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()))
    .AddKeycloakAuth(builder.Configuration, builder.Environment)
    .AddAuthorizationBuilder()
        .AddDefaultPolicy("AuthenticatedUsers", policy =>
            policy.RequireAuthenticatedUser());

builder.Services.AddHttpsRedirection(options => options.HttpsPort = 7289);
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/healthz");
app.MapUserEndpoints();

app.Run();

public partial class Program { }
