// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Exceptions.Constants
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Exceptions;

public static class Constants
{
  public static class Guards
  {
    public const string CantBeNull = "{0} can't be null or empty.";
    public const string BothCantBeNull = "Both {0} and {1} can't be null or empty.";
    public const string CantBeTrue = "{0} can't be true for this method.";
    public const string CantBeFalse = "{0} can't be true for this method.";
    public const string AgainstMessage = "Assertion evaluation failed with 'false'.";
    public const string ImplementsMessage = "Type '{0}' must implement type '{1}'.";
    public const string InheritsFromMessage = "Type '{0}' must inherit from type '{1}'.";
    public const string IsTypeOfMessage = "Type '{0}' must be of type '{1}'.";
    public const string IsEqualMessage = "Compared objects must be equal.";
    public const string IsPositiveMessage = "Argument '{0}' must be a positive value. Value: '{1}'.";
    public const string IsTrueMessage = "True expected for '{0}' but the condition was False.";
    public const string NotNegativeMessage = "Argument '{0}' cannot be a negative value. Value: '{1}'.";
    public const string NotEmptyStringMessage = "String parameter '{0}' cannot be null or all whitespace.";
    public const string NotEmptyColMessage = "Collection cannot be null and must contain at least one item.";
    public const string NotEmptyGuidMessage = "Argument '{0}' cannot be an empty guid.";
    public const string InRangeMessage = "The argument '{0}' must be between '{1}' and '{2}'.";
    public const string NotOutOfLengthMessage = "Argument '{0}' cannot be more than {1} characters long.";
    public const string NotZeroMessage = "Argument '{0}' must be greater or less than zero. Value: '{1}'.";
    public const string IsEnumTypeMessage = "Type '{0}' must be a valid Enum type.";
    public const string IsEnumTypeMessage2 = "The value of the argument '{0}' provided for the enumeration '{1}' is invalid.";
    public const string IsSubclassOfMessage = "Type '{0}' must be a subclass of type '{1}'.";
    public const string HasDefaultConstructorMessage = "The type '{0}' must have a default parameterless constructor.";
  }

  public static class Stacky
  {
    public const string AtValue = "   at ";
    public const string CsFileExt = ".cs";
    public const string DefaultExceptionFileName = "SystemException";
    public const string StackDomainBoundary = "---";
    public const string NewDomainBoundaryTemplate = "=== Sub-stack {0} ===";
    public static readonly string[] NewLineArray = new string[1]
    {
      Environment.NewLine ?? ""
    };
    public static readonly string[] InArray = new string[1]
    {
      " in "
    };
    public static readonly string[] AtArray = new string[1]
    {
      "   at "
    };
    public static readonly string[] LineArray = new string[1]
    {
      ":line "
    };
  }
}