﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public MessageController(IWpSmsService wpSmsService, ISmsConfigService smsConfigService, UserManager<WpUsers> userManager, ILogger<MessageController> logger)
        {
            _wpSmsService = wpSmsService;
            _smsConfigService = smsConfigService;
            _userManager = userManager;
            _logger = logger;
        }
        public IActionResult SendMessage()
        {
            BaseFormViewModel<WpSms> formViewModel = new BaseFormViewModel<WpSms>();
            return View(formViewModel);
        }
        [HttpPost]
        public async Task<JsonResult> SendMessage(string Noidung, string Canbos, List<IFormFile> fileDinhKem, List<long> selectedFileIds)
        {
            try
            {
                // gán người gửi
                if (string.IsNullOrEmpty(Noidung))
                {
                    return Json(new
                    {
                        state = "error",
                        msg = "Không có nội dung tin nhắn!",
                        data = new { },
                    });
                }
                var cb = new List<WpCanboViewModel>();
                if (string.IsNullOrEmpty(Canbos))
                {
                    return Json(new
                    {
                        state = "error",
                        msg = "Chưa chọn người nhận tin nhắn!",
                        data = new { },
                    });
                }
                else
                {
                    cb = JsonConvert.DeserializeObject<List<WpCanboViewModel>>(Canbos);
                    if (cb == null || cb.Count == 0)
                        return Json(new
                        {
                            state = "error",
                            msg = "Chưa chọn người nhận tin nhắn!",
                            data = new { },
                        });
                }
                WpUsers? user = await _userManager.GetUserAsync(HttpContext.User);

                var model = new WpSmsViewModel()
                {
                    Noidung = Noidung,
                    WpCanbos = cb
                };

                if (user != null) await _wpSmsService.SendMessage(model, fileDinhKem, selectedFileIds, user);
                _logger.LogInformation("SendMessage succeed");

                return Json(new
                {
                    state = "success",
                    msg = "Gửi tin nhắn thành công!",
                    data = new { },
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while SendMessage");
                return Json(new
                {
                    state = "error",
                    msg = "Đã xảy ra lỗi khi gửi tin nhắn!",
                    data = new { },
                });
            }
        }
        public async Task<IActionResult> MessageStatistical()
        {
            return View();
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

    }
}
