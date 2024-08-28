// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Exceptions.DomainException
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Runtime.Serialization;

namespace BuildingBlocks.Exceptions;

[Serializable]
public class DomainException : Exception
{
  public DomainException()
  {
  }

  public DomainException(string message)
    : base(message)
  {
  }

  public DomainException(string messageFormat, params object[] args)
    : base(string.Format(messageFormat, args))
  {
  }

  protected DomainException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }

  public DomainException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}