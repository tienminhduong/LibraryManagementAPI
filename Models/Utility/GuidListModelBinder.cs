using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LibraryManagementAPI.Models.Utility
{
    public class GuidListModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var modelName = bindingContext.ModelName;

            // Lấy tất cả giá trị từ Form Data (hoặc Query)
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var values = valueProviderResult.Values;
            var list = new List<Guid>();

            foreach (var value in values)
            {
                if (Guid.TryParse(value, out var guid))
                {
                    list.Add(guid);
                }
                else
                {
                    // Nếu không phân tích được, thêm lỗi vào ModelState
                    bindingContext.ModelState.TryAddModelError(modelName,
                        $"'{value}' không phải là một GUID hợp lệ.");
                    return Task.CompletedTask;
                }
            }

            bindingContext.Result = ModelBindingResult.Success(list);
            return Task.CompletedTask;
        }
    }
}
