using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VnptSmsBrandName.Common;
using VnptSmsBrandName.Models.Master;
using VnptSmsBrandName.Service;
using VnptSmsBrandName.ViewModel;

namespace VnptSmsBrandName.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ConfigController : Controller
    {
        private readonly IConfigService _configService;
        public ConfigController(IConfigService configService)
        {
            _configService = configService;
        }
        public async Task<IActionResult> Index()
        {
            var config = await _configService.FindByKey("file_delete_after");
            if (config == null)
            {
                config = new Config
                {
                    Key = "file_delete_after",
                    Value = "30",
                    IsUsed = 0
                };
            }
            var vm = config;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Config model)
        {
            var isUsedValues = Request.Form["IsUsed"].ToString();
            model.IsUsed = isUsedValues.Contains("1") ? 1 : 0;
            model.Key = "file_delete_after";
            int value = Convert.ToInt32(model.Value ?? "0");
            if (value < 1)
            {
                model.Value = "30";
            }
            try
            {
                var existing = await _configService.FindByKey(model.Key);
                if (existing == null)
                {
                    await _configService.Create(model);
                }
                else
                {
                    existing.IsUsed = model.IsUsed;
                    existing.Value = model.Value;
                    await _configService.Update(existing);
                }

                TempData["SuccessMessage"] = "Luu c?u hình thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có l?i x?y ra khi luu c?u hình.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
