// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Types.Email
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using System.Text.RegularExpressions;
using BuildingBlocks.Exceptions;
using CSharpFunctionalExtensions;

namespace BuildingBlocks.Types;

public class Email : BuildingBlocks.SeedWork.ValueObject
{
  private readonly 
#nullable disable
    string _value;
    
  private Email(string value) => this._value = value;

  public static Result<Email, Error> Create(string email)
  {
    if (string.IsNullOrWhiteSpace(email))
      return Result.Failure<Email, Error>(new Error("email.validation", "E-mail can't be empty"));
    if (email.Length > 100)
      return Result.Failure<Email, Error>(new Error("email.validation", "E-mail is too long"));
    return !Regex.IsMatch(email, "^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$") ? Result.Failure<Email, Error>(new Error("email.validation", "E-mail is invalid")) : (Result<Email, Error>) new Email(email);
  }

  public override string ToString() => this._value;

  protected override IEnumerable<IComparable> GetEqualityComponents()
  {
    yield return (IComparable) this._value;
  }
}