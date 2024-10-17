using CSharpFunctionalExtensions;
using ValueObject = BuildingBlocks.SeedWork.ValueObject;

namespace BuildingBlocks.Types;

public class Username : ValueObject
{
  private readonly 
#nullable disable
    string _value;

  private Username(string value) => this._value = value;

  public static Result<Username> Create(string username)
  {
    if (string.IsNullOrWhiteSpace(username))
      return Result.Failure<Username>("Username can't be empty");
    return username.Length > 100 ? Result.Failure<Username>("Username is too long") : Result.Success<Username>(new Username(username));
  }

  protected override IEnumerable<IComparable> GetEqualityComponents()
  {
    yield return (IComparable) this._value;
  }

  public override string ToString() => this._value;
}