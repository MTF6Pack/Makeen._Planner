//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
//using System;
//using System.Globalization;
//using System.Threading.Tasks;

//namespace Infrastructure
//{
//    class PersianDateModelBinder : IModelBinder
//    {
//        public Task BindModelAsync(ModelBindingContext bindingContext)
//        {
//            ArgumentNullException.ThrowIfNull(bindingContext);

//            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

//            if (valueProviderResult == ValueProviderResult.None)
//                return Task.CompletedTask;

//            string value = valueProviderResult.FirstValue;

//            if (string.IsNullOrWhiteSpace(value))
//                return Task.CompletedTask;

//            try
//            {
//                DateTime convertedDate = DateHelper.ConvertPersianToGregorian(value);
//                bindingContext.Result = ModelBindingResult.Success(convertedDate);
//            }
//            catch (Exception)
//            {
//                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid Persian date format.");
//            }

//            return Task.CompletedTask;
//        }
//    }

//    public class PersianDateModelBinderProvider : IModelBinderProvider
//    {
//        public IModelBinder? GetBinder(ModelBinderProviderContext context)
//        {
//            if (context.Metadata.ModelType == typeof(DateTime) || context.Metadata.ModelType == typeof(DateTime?))
//            {
//                return new BinderTypeModelBinder(typeof(PersianDateModelBinder));
//            }
//            return null;
//        }
//    }
//}
