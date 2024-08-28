// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Exceptions.ErrorMessages
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Exceptions;

public static class ErrorMessages
{
  public const string UserNotFound = "User not found.";
  public const string UserAlreadyExists = "User already exists.";
  public const string AuthenticationFailed = "Authentication failed.";
  public const string InvalidUserInput = "Invalid user input.";
  public const string UserIsInactive = "User is inactive.";
  public const string LdapConnectionFailed = "Failed to connect to LDAP server.";
  public const string LdapSearchFailed = "LDAP search failed.";
  public const string LdapBindFailed = "LDAP bind failed.";
  public const string DatabaseOperationFailed = "Database operation failed.";
  public const string RecordNotFound = "Record not found.";
  public const string RecordAlreadyExists = "Record already exists.";
  public const string ForeignKeyViolation = "Foreign key violation.";
  public const string FileNotFound = "File not found.";
  public const string FileUploadFailed = "File upload failed.";
  public const string FileDownloadFailed = "File download failed.";
  public const string NetworkTimeout = "Network timeout.";
  public const string ResourceUnavailable = "Resource unavailable.";
  public const string ApiInvalidRequest = "Invalid API request.";
  public const string ApiUnauthorizedAccess = "Unauthorized API access.";
  public const string ApiRateLimitExceeded = "API rate limit exceeded.";
  public const string ApiInvalidToken = "Invalid API token.";
  public const string ApiDataValidationFailed = "API data validation failed.";
  public const string ApiResourceNotFound = "API resource not found.";
  public const string ApiOperationFailed = "API operation failed.";
  public const string InternalServerError = "An internal server error occurred.";
  public const string BadRequest = "Bad request.";
  public const string Unauthorized = "Unauthorized.";
  public const string Forbidden = "Forbidden.";
  public const string NotFound = "Not found.";
  public const string MethodNotAllowed = "Method not allowed.";
  public const string RateLimitExceeded = "Rate limit exceeded.";
}