using System.Security.Claims;

using Asp.Versioning;

using BetterTests.Application.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterTests.Api.Presentation.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet("profile")]
    [Authorize(Policy = "AuthenticatedUsers")]
    public ActionResult<UserProfileResponse> GetProfile()
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("preferred_username")?.Value;
        var username = User.FindFirst("preferred_username")?.Value;
        var email = User.FindFirst("email")?.Value;
        var firstName = User.FindFirst("given_name")?.Value;
        var lastName = User.FindFirst("family_name")?.Value;

        var roles = User.FindAll("roles")
            .Select(c => c.Value)
            .Union(User.FindAll(ClaimTypes.Role).Select(c => c.Value))
            .Union(User.FindAll("realm_access").SelectMany(c => ExtractRealmRoles(c.Value)))
            .Distinct()
            .ToList();

        var expClaim = User.FindFirst("exp");
        var issuedAtClaim = User.FindFirst("iat");

        var expiresAt = DateTime.UnixEpoch.AddSeconds(long.Parse(expClaim?.Value ?? "0"));
        var issuedAt = DateTime.UnixEpoch.AddSeconds(long.Parse(issuedAtClaim?.Value ?? "0"));

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Invalid token: missing user ID");

        var response = new UserProfileResponse(
            Id: userId,
            Username: username ?? "unknown",
            Email: email ?? "",
            FirstName: firstName,
            LastName: lastName,
            Roles: roles,
            TokenIssuedAt: issuedAt,
            TokenExpiresAt: expiresAt
        );

        return Ok(response);
    }

    [HttpPost("validate")]
    [Authorize(Policy = "AuthenticatedUsers")]
    public ActionResult<TokenValidationResponse> ValidateToken()
    {
        var expClaim = User.FindFirst("exp");
        if (expClaim == null)
            return BadRequest(new TokenValidationResponse(false, "No expiration claim found", null));

        var expiresAt = DateTime.UnixEpoch.AddSeconds(long.Parse(expClaim.Value));
        var isValid = expiresAt > DateTime.UtcNow;

        var response = new TokenValidationResponse(
            IsValid: isValid,
            Message: isValid ? "Token is valid" : "Token is expired",
            ExpiresAt: expiresAt
        );

        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize(Policy = "AuthenticatedUsers")]
    public ActionResult<LogoutResponse> Logout()
    {
        var username = User.FindFirst("preferred_username")?.Value ?? "unknown";

        var response = new LogoutResponse(
            Success: true,
            Message: $"User {username} logged out successfully. Please delete the stored token from client."
        );

        return Ok(response);
    }

    [HttpGet("permissions")]
    [Authorize(Policy = "AuthenticatedUsers")]
    public ActionResult<PermissionsResponse> GetPermissions()
    {
        var roles = User.FindAll("roles")
            .Select(c => c.Value)
            .Union(User.FindAll(ClaimTypes.Role).Select(c => c.Value))
            .Distinct()
            .ToList();

        var canCreateProjects = roles.Contains("admin") || roles.Contains("manager");
        var canManageProjects = roles.Contains("admin") || roles.Contains("manager");
        var canExecuteTests = roles.Contains("admin") || roles.Contains("tester") || roles.Contains("manager");
        var canViewReports = roles.Contains("admin") || roles.Contains("tester") || roles.Contains("manager") || roles.Contains("viewer");

        var permissions = new List<string>();
        if (canCreateProjects)
            permissions.Add("create_projects");
        if (canManageProjects)
            permissions.Add("manage_projects");
        if (canExecuteTests)
            permissions.Add("execute_tests");
        if (canViewReports)
            permissions.Add("view_reports");
        permissions.Add("view_own_profile");

        var response = new PermissionsResponse(
            Roles: roles,
            Permissions: permissions,
            CanCreateProjects: canCreateProjects,
            CanManageProjects: canManageProjects,
            CanExecuteTests: canExecuteTests,
            CanViewReports: canViewReports
        );

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
            return BadRequest("Refresh token is required");

        try
        {
            return BadRequest(new
            {
                error = "token_refresh_not_implemented",
                message = "Token refresh should be handled by calling Keycloak's token endpoint directly: POST /realms/{realm}/protocol/openid-connect/token",
                note = "Use grant_type=refresh_token with your refresh_token and client credentials"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                error = "token_refresh_failed",
                message = ex.Message
            });
        }
    }

    [HttpPost("change-password")]
    [Authorize(Policy = "AuthenticatedUsers")]
    public async Task<ActionResult<ChangePasswordResponse>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (string.IsNullOrEmpty(request.CurrentPassword) || string.IsNullOrEmpty(request.NewPassword))
            return BadRequest(new ChangePasswordResponse(false, "Current password and new password are required"));

        if (request.NewPassword != request.ConfirmPassword)
            return BadRequest(new ChangePasswordResponse(false, "New passwords do not match"));

        if (request.NewPassword.Length < 8)
            return BadRequest(new ChangePasswordResponse(false, "New password must be at least 8 characters"));

        try
        {
            var userId = User.FindFirst("sub")?.Value;

            return BadRequest(new ChangePasswordResponse(
                false,
                "Password change requires Keycloak admin API integration. Configure admin credentials in appsettings and call Keycloak's /admin/realms/{realm}/users/{userId}/reset-password endpoint"
            ));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ChangePasswordResponse(false, $"Password change failed: {ex.Message}"));
        }
    }

    private IEnumerable<string> ExtractRealmRoles(string realmAccessJson)
    {
        try
        {
            return [];
        }
        catch
        {
            return [];
        }
    }
}
