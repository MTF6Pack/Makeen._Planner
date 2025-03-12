using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;

namespace Persistence
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
                        // Check if the property name is "Alarm" and handle it differently
                        if (property.Name == "Alarm")
                        {
                            var converterType = typeof(EnumToNumberConverter<,>)
                                .MakeGenericType(property.ClrType, typeof(int)); // Store as int

                            property.SetValueConverter(Activator.CreateInstance(converterType) as ValueConverter);
                        }
                        else
                        {
                            var converterType = typeof(EnumToStringConverter<>)
                                .MakeGenericType(property.ClrType); // Store as string

                            property.SetValueConverter(Activator.CreateInstance(converterType) as ValueConverter);
                        }
                    }
                }
            }
            return modelBuilder;
        }
    }
}