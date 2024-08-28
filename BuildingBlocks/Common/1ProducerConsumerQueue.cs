// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.ProducerConsumerQueueException
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Runtime.Serialization;

namespace BuildingBlocks.Common;

[Serializable]
public sealed class ProducerConsumerQueueException : Exception
{
  internal ProducerConsumerQueueException()
  {
  }

  internal ProducerConsumerQueueException(string message)
    : base(message)
  {
  }

  internal ProducerConsumerQueueException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  internal ProducerConsumerQueueException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}