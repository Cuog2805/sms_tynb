using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VnptSmsBrandName.Common;
using VnptSmsBrandName.Models.Identity;
using VnptSmsBrandName.Models.Master;
using VnptSmsBrandName.Service;
using VnptSmsBrandName.ViewModel;

namespace VnptSmsBrandName.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ConfigController : BaseController
	{
        private readonly IConfigService _configService;
        public ConfigController
        (
            IConfigService configService,
			IHttpContextAccessor httpContextAccessor,
			UserManager<Users> userManager
        ) : base(userManager, httpContextAccessor)
        {
            _configService = configService;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentUser();
			var config = await _configService.FindByKeyAndOrgId("file_delete_after", currentUser.OrganizationId);
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
            var currentUser = await GetCurrentUser();
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
                var existing = await _configService.FindByKeyAndOrgId(model.Key, currentUser.OrganizationId);
                if (existing == null)
                {
                    await _configService.Create(model, currentUser);
                }
                else
                {
                    existing.IsUsed = model.IsUsed;
                    existing.Value = model.Value;
                    await _configService.Update(existing, currentUser);
                }

                TempData["SuccessMessage"] = "Lưu cấu hình thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi lưu cấu hình.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
