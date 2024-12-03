using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;

namespace Makeen.Planner.Persistence
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder UseEnumToStringConverter(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.GetProperties();

                foreach (var property in properties)
                {
                    if (property.ClrType.IsEnum)
                    {
                        var converterType = typeof(EnumToStringConverter<>).MakeGenericType(property.ClrType);
                        property.SetValueConverter(Activator.CreateInstance(converterType, (object?)null) as ValueConverter);
                    }
                }
            }
            return modelBuilder;
        }
    }
}
