// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Exceptions.ErrorCodes
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Exceptions;

public static class ErrorCodes
{
  public const string UserNotFound = "USER_NOT_FOUND";
  public const string UserAlreadyExists = "USER_ALREADY_EXISTS";
  public const string AuthenticationFailed = "AUTHENTICATION_FAILED";
  public const string InvalidUserInput = "INVALID_USER_INPUT";
  public const string UserIsInactive = "USER_IS_INACTIVE";
  public const string LdapConnectionFailed = "LDAP_CONNECTION_FAILED";
  public const string LdapSearchFailed = "LDAP_SEARCH_FAILED";
  public const string LdapBindFailed = "LDAP_BIND_FAILED";
  public const string DatabaseOperationFailed = "DB_OPERATION_FAILED";
  public const string RecordNotFound = "RECORD_NOT_FOUND";
  public const string RecordAlreadyExists = "RECORD_ALREADY_EXISTS";
  public const string ForeignKeyViolation = "FOREIGN_KEY_VIOLATION";
  public const string FileNotFound = "FILE_NOT_FOUND";
  public const string FileUploadFailed = "FILE_UPLOAD_FAILED";
  public const string FileDownloadFailed = "FILE_DOWNLOAD_FAILED";
  public const string NetworkTimeout = "NETWORK_TIMEOUT";
  public const string ResourceUnavailable = "RESOURCE_UNAVAILABLE";
  public const string ApiInvalidRequest = "API_INVALID_REQUEST";
  public const string ApiUnauthorizedAccess = "API_UNAUTHORIZED_ACCESS";
  public const string ApiRateLimitExceeded = "API_RATE_LIMIT_EXCEEDED";
  public const string ApiInvalidToken = "API_INVALID_TOKEN";
  public const string ApiDataValidationFailed = "API_DATA_VALIDATION_FAILED";
  public const string ApiResourceNotFound = "API_RESOURCE_NOT_FOUND";
  public const string ApiOperationFailed = "API_OPERATION_FAILED";
  public const string InternalServerError = "INTERNAL_SERVER_ERROR";
  public const string BadRequest = "BAD_REQUEST";
  public const string Unauthorized = "UNAUTHORIZED";
  public const string Forbidden = "FORBIDDEN";
  public const string NotFound = "NOT_FOUND";
  public const string MethodNotAllowed = "METHOD_NOT_ALLOWED";
  public const string RateLimitExceeded = "RATE_LIMIT_EXCEEDED";
}