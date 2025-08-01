using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VnptSmsBrandName.Common;
using VnptSmsBrandName.Common.Enum;
using VnptSmsBrandName.Helper;
using VnptSmsBrandName.Models.Identity;
using VnptSmsBrandName.Models.Master;
using VnptSmsBrandName.Service;
using VnptSmsBrandName.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using static VnptSmsBrandName.ViewModel.ApiModel.SmsApiViewModel;

namespace VnptSmsBrandName.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class MessageController : BaseController
    {
        private readonly IMSmsService _mSmsService;
        private readonly IMGroupService _mGroupService;
        private readonly IMEmployeeService _mEmployeeService;
        private readonly IMFileService _mFileService;

		public MessageController
        (
            IMSmsService mSmsService, 
			IMGroupService mGroupService, 
            IMEmployeeService mEmployeeService, 
            IMFileService mFileService,
			IHttpContextAccessor httpContextAccessor,
			UserManager<Users> userManager
		): base(userManager, httpContextAccessor)
        {
			_mSmsService = mSmsService;
			_mGroupService = mGroupService;
			_mEmployeeService = mEmployeeService;
			_mFileService = mFileService;
		}

        public IActionResult SendMessage()
        {
            BaseFormViewModel<MSms> formViewModel = new BaseFormViewModel<MSms>();
            return View(formViewModel);
        }
		
		[HttpPost]
		public async Task<IActionResult> SendMessage(string content, string canbos, List<IFormFile> fileDinhKem, List<long> selectedFileIds)
		{
			var currentUser = await GetCurrentUser();
			var model = new MSmsViewModel()
			{
				Content = content,
				Employees = JsonConvert.DeserializeObject<List<MEmployeeViewModel>>(canbos) ?? new List<MEmployeeViewModel>()
			};
			var result = await _mSmsService.SendMessage(model, fileDinhKem, selectedFileIds, currentUser);

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
			BaseFormViewModel<MSmsSearchViewModel> formViewModel = new BaseFormViewModel<MSmsSearchViewModel>()
			{
				Data = new MSmsSearchViewModel(),
				SelectLists = await CreateSelectList()
			};
			return View(formViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> LoadData(MSmsSearchViewModel model, Pageable pageable)
        {
			var currentUser = await GetCurrentUser();
			var datas = await _mSmsService.SearchMessage(model, pageable, currentUser.OrganizationId);
            return Json(new
            {
                state = "success",
                msg = "LoadData thành công!",
                content = datas,
            });
        }
		[HttpGet]
		public async Task<IActionResult> LoadDetail(MSmsSearchViewModel model)
		{
			var currentUser = await GetCurrentUser();
			var datas = await _mSmsService.GetSmsEmployeesById(model, currentUser.OrganizationId);
			return Json(new
			{
				state = "success",
				msg = "LoadDetail thành công!",
				data = datas,
			});
		}

		[HttpPost]
        public async Task<IActionResult> MessageUpdateFile([FromForm] long oldFileId, IFormFile fileDinhKem)
        {
			var currentUser = await GetCurrentUser();
			await _mFileService.UpdateContentFile(fileDinhKem, oldFileId, currentUser);
            return Json(new
            {
                state = "success",
                msg = "Cập nhật file thành công!",
                content = new { },
            });
        }
		private async Task<Dictionary<string, SelectList>> CreateSelectList()
		{
			var currentUser = await GetCurrentUser();
			SelectList mGroupSelectList = new SelectList(await _mGroupService.GetMGroupList(currentUser.OrganizationId), "GroupId", "Name");
			SelectList mEmployeeSelectList = new SelectList(await _mEmployeeService.GetAllMEmployee(currentUser.OrganizationId), "EmployeeId", "Name");
			SelectList mFileSelectList = new SelectList(await _mFileService.GetAllFile(currentUser.OrganizationId), "FileId", "Name");
			SelectList statusSelectList = new SelectList(EnumHelper.ToSelectListItem<SmsStatusEnum>(), "Value", "Text");

			var selectLists = new Dictionary<string, SelectList>
			{
				{ "mGroupSelectList", mGroupSelectList },
				{ "mEmployeeSelectList", mEmployeeSelectList },
				{ "mFileSelectList", mFileSelectList },
				{ "statusSelectList", statusSelectList }
			};
			return selectLists;
		}
	}
}
