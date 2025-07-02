using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Weavly.Core.Persistence.Configuration;

/// <summary>
///     Generates strongly typed IDs using Version 7 GUIDs
/// </summary>
/// <typeparam name="TId">Type of the ID</typeparam>
public class StronglyGuidGenerator<TId> : ValueGenerator<TId>
    where TId : struct
{
    public override TId Next(EntityEntry entry)
    {
        return (TId?)Activator.CreateInstance(typeof(TId), Guid.CreateVersion7().ToString())
            ?? throw new InvalidOperationException("Cannot create a strongly typed ID.");
    }

    public override bool GeneratesTemporaryValues => false;
}
