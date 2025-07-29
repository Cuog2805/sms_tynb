using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SMS_TYNB.Common;
using SMS_TYNB.Helper;
using SMS_TYNB.Models.Identity;
using SMS_TYNB.Models.Master;
using SMS_TYNB.Service;
using SMS_TYNB.Service.Implement;
using SMS_TYNB.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SMS_TYNB.ViewModel.ApiModel.SmsApiViewModel;

namespace SMS_TYNB.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class MessageController : Controller
    {
        private readonly ILogger<MessageController> _logger;
        private readonly IWpSmsService _wpSmsService;
        private readonly ISmsConfigService _smsConfigService;
        private readonly UserManager<WpUsers> _userManager;
        private readonly IWpNhomService _wpNhomService;
        private readonly IWpCanboService _wpCanboService;
        private readonly IWpFileService _wpFileService;
        private readonly IWpDanhmucService _wpDanhmucService;

		public MessageController
        (
            IWpSmsService wpSmsService, 
            ISmsConfigService smsConfigService, 
            UserManager<WpUsers> userManager, 
            ILogger<MessageController> logger,
			IWpNhomService wpNhomService, 
            IWpCanboService wpCanboService, 
            IWpFileService wpFileService,
			IWpDanhmucService wpDanhmucService
		)
        {
            _wpSmsService = wpSmsService;
            _smsConfigService = smsConfigService;
            _userManager = userManager;
            _logger = logger;
            _wpNhomService = wpNhomService;
            _wpCanboService = wpCanboService;
            _wpFileService = wpFileService;
            _wpDanhmucService = wpDanhmucService;
		}
        public IActionResult SendMessage()
        {
            BaseFormViewModel<WpSms> formViewModel = new BaseFormViewModel<WpSms>();
            return View(formViewModel);
        }
		//    [HttpPost]
		//    public async Task<JsonResult> SendMessage(string Noidung, string Canbos, List<IFormFile> fileDinhKem, List<long> selectedFileIds)
		//    {
		//        try
		//        {
		//            // gán người gửi
		//            if (string.IsNullOrEmpty(Noidung))
		//            {
		//                return Json(new
		//                {
		//                    state = "error",
		//                    msg = "Không có nội dung tin nhắn!",
		//                    data = new { },
		//                });
		//            }
		//            var cb = new List<WpCanboViewModel>();
		//            if (string.IsNullOrEmpty(Canbos))
		//            {
		//                return Json(new
		//                {
		//                    state = "error",
		//                    msg = "Chưa chọn người nhận tin nhắn!",
		//                    data = new { },
		//                });
		//            }
		//            else
		//            {
		//                cb = JsonConvert.DeserializeObject<List<WpCanboViewModel>>(Canbos);
		//                if (cb == null || cb.Count == 0)
		//                    return Json(new
		//                    {
		//                        state = "error",
		//                        msg = "Chưa chọn người nhận tin nhắn!",
		//                        data = new { },
		//                    });
		//            }
		//            WpUsers? user = await _userManager.GetUserAsync(HttpContext.User);

		//            var model = new WpSmsViewModel()
		//            {
		//                Noidung = Noidung,
		//                WpCanbos = cb
		//            };

		//            if (user == null)
		//{
		//	return Json(new
		//	{
		//		state = "error",
		//		msg = "Lỗi khi thông tin người dùng!",
		//		data = new {},
		//	});
		//}

		//var data = await _wpSmsService.SendMessage(model, fileDinhKem, selectedFileIds, user);
		//_logger.LogInformation("SendMessage succeed");

		//            return Json(new
		//            {
		//                state = "success",
		//                msg = "Gửi tin nhắn thành công!",
		//                data = data,
		//            });
		//        }
		//        catch (Exception ex)
		//        {
		//            _logger.LogError(ex, "Error occurred while SendMessage");
		//            return Json(new
		//            {
		//                state = "error",
		//                msg = "Đã xảy ra lỗi khi gửi tin nhắn!" + ex.Message,
		//                data = new { },
		//            });
		//        }
		//    }
		[HttpPost]
		public async Task<IActionResult> SendMessage(string Noidung, string Canbos, List<IFormFile> fileDinhKem, List<long> selectedFileIds)
		{
			WpUsers? user = await _userManager.GetUserAsync(HttpContext.User);
			var model = new WpSmsViewModel()
			{
				Noidung = Noidung,
				WpCanbos = JsonConvert.DeserializeObject<List<WpCanboViewModel>>(Canbos) ?? new List<WpCanboViewModel>()
			};
			var result = await _wpSmsService.SendMessage(model, fileDinhKem, selectedFileIds, user);

			if (result.IsSuccess)
			{
				return Json(new
				{
					state = "success",
					data = result.Data,
					msg = "Gửi tin nhắn thành công"
				});
			}
			else
			{
				return Json(new
				{
					state = "error",
					msg = result.ErrorMessage
				});
			}
		}
		public async Task<IActionResult> MessageStatistical()
        {
			BaseFormViewModel<WpSmsSearchViewModel> formViewModel = new BaseFormViewModel<WpSmsSearchViewModel>()
			{
				Data = new WpSmsSearchViewModel(),
				SelectLists = await CreateSelectList()
			};
			return View(formViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> LoadData(WpSmsSearchViewModel model, Pageable pageable)
        {
            var datas = await _wpSmsService.SearchMessage(model, pageable);
            return Json(new
            {
                state = "success",
                msg = "LoadData thành công!",
                content = datas,
            });
        }

        [HttpPost]
        public async Task<IActionResult> MessageUpdateFile([FromForm] WpFile oldFile, IFormFile fileDinhKem)
        {
            await _wpSmsService.UpdateFile(oldFile, fileDinhKem);
            return Json(new
            {
                state = "success",
                msg = "Cập nhật file thành công!",
                content = new { },
            });
        }
		private async Task<Dictionary<string, SelectList>> CreateSelectList()
		{
			SelectList wpNhomSelectList = new SelectList(await _wpNhomService.GetAllWpNhom(), "IdNhom", "TenNhom");
			SelectList wpCanboSelectList = new SelectList(await _wpCanboService.GetAllWpCanbo(), "IdCanbo", "TenCanbo");
			SelectList wpFileSelectList = new SelectList(await _wpFileService.GetAllWpFile(), "IdFile", "TenFile");
			SelectList wpTrangThaiSelectList = new SelectList(await _wpDanhmucService.GetWpDanhmucByType("TRANGTHAI_TN"), "Value", "TenDanhmuc");

			var selectLists = new Dictionary<string, SelectList>
			{
				{ "wpNhomSelectList", wpNhomSelectList },
				{ "wpCanboSelectList", wpCanboSelectList },
				{ "wpFileSelectList", wpFileSelectList },
				{ "wpTrangThaiSelectList", wpTrangThaiSelectList }
			};
			return selectLists;
		}
	}
}
