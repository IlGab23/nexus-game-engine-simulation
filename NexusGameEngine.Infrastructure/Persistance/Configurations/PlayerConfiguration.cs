using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusGameEngine.Domain.Entities;

namespace NexusGameEngine.Infrastructure.Persistance.Configurations;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        throw new NotImplementedException();
    }

}
