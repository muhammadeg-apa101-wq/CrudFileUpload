using EntityFrameworkProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace EntityFrameworkProject.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly LayoutService _layoutService;
        public FooterViewComponent(LayoutService layoutService)
        {
            _layoutService = layoutService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var settings = await _layoutService.GetSettings();
            return await Task.FromResult(View(settings));
        }
    }
}
