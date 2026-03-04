namespace BetterTests.Application.DTOs;

public record UserProfileResponse(
    string Id,
    string Username,
    string Email,
    string? FirstName,
    string? LastName,
    IEnumerable<string> Roles,
    DateTime TokenIssuedAt,
    DateTime TokenExpiresAt
);

public record TokenValidationResponse(
    bool IsValid,
    string? Message,
    DateTime? ExpiresAt
);

public record RefreshTokenRequest(
    string RefreshToken
);

public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType = "Bearer"
);

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
);

public record ChangePasswordResponse(
    bool Success,
    string Message
);

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType = "Bearer"
);

public record PermissionsResponse(
    IEnumerable<string> Roles,
    IEnumerable<string> Permissions,
    bool CanCreateProjects,
    bool CanManageProjects,
    bool CanExecuteTests,
    bool CanViewReports
);

public record LogoutResponse(
    bool Success,
    string Message
);
