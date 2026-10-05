using ASP_Lesson_14.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ASP_Lesson_14.ModelBinders
{
    public class CartBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
           return context.Metadata.ModelType == typeof(Cart) ? new CartModelBinder() : null;
        }
    }
}
