﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SMS_TYNB.Common;
using SMS_TYNB.Helper;
using SMS_TYNB.Models.Identity;
using SMS_TYNB.Models.Master;
using SMS_TYNB.Repository;
using SMS_TYNB.ViewModel;
using System.Collections.Generic;
using static SMS_TYNB.ViewModel.ApiModel.SmsApiViewModel;

namespace SMS_TYNB.Service.Implement
{
    public class WpSmsService : IWpSmsService
    {
        private readonly WpSmsRepository _wpSmsRepository;
        private readonly WpSmsCanboRepository _wpSmsCanboRepository;
        private readonly WpUsersRepository _wpUsersRepository;
        private readonly WpCanboRepository _wpCanboRepository;
        private readonly IWpFileService _wpFileService;
        private readonly ISmsConfigService _smsConfigService;
        private readonly ILogger<WpSmsService> _logger;
        private readonly IWebHostEnvironment _environment;
        public WpSmsService
        (
            WpSmsRepository wpSmsRepository,
            WpSmsCanboRepository wpSmsCanboRepository,
            IWpFileService wpFileService,
            WpUsersRepository wpUsersRepository,
            ILogger<WpSmsService> logger,
            ISmsConfigService smsConfigService,
            WpCanboRepository wpCanboRepository,
            IWebHostEnvironment environment
        )
        {
            _wpSmsRepository = wpSmsRepository;
            _wpSmsCanboRepository = wpSmsCanboRepository;
            _wpFileService = wpFileService;
            _wpUsersRepository = wpUsersRepository;
            _logger = logger;
            _smsConfigService = smsConfigService;
            _wpCanboRepository = wpCanboRepository;
            _environment = environment;

        }
        public async Task SendMessage(WpSmsViewModel model, List<IFormFile> fileDinhKem, List<long> selectedFileIds, WpUsers user)
        {
            if (model == null || user == null)
                return;

            WpSms wpSms = new WpSms()
            {
                Noidung = model.Noidung,
                Ngaygui = DateTime.Now,
                IdNguoigui = user.Id,
                SoTn = model.WpCanbos.Count,
                SoTnLoi = 0
            };
            wpSms = await _wpSmsRepository.Create(wpSms);

            int errorCount = 0;
            int successCount = 0;

            // Xử lý file đính kèm
            var smsConfig = _smsConfigService.GetSmsConfigActive(true);
            var fileUrls = await HandleFileAttachments(fileDinhKem, selectedFileIds, user, wpSms.IdSms, smsConfig.Domain);
            if (smsConfig?.Id > 0 && model.WpCanbos.Any())
            {
                foreach (var canbo in model.WpCanbos)
                {
                    try
                    {
                        var cb = _wpCanboRepository.FindById(canbo.IdCanbo ?? 0).Result;
                        if (cb != null && cb.IdCanbo > 0)
                        {
                            var msgContent = string.Empty;
                            msgContent = !string.IsNullOrEmpty(fileUrls) ? $"{model.Noidung} {fileUrls}" : model.Noidung;
                            var res = SmsHelper.SendSms(smsConfig, msgContent, cb.SoDTGui);
                            await SendMessageToCanbo(canbo, wpSms.IdSms, res);

                            if (res?.RPLY?.ERROR == "0")
                                successCount++;
                            else
                                errorCount++;
                        }
                    }
                    catch (Exception)
                    {
                        errorCount++;
                    }
                }
            }
            else
            {
                errorCount = model.WpCanbos?.Count ?? 0;
                successCount = 0;
            }
            wpSms.SoTn = successCount;
            wpSms.SoTnLoi = errorCount;
            await _wpSmsRepository.Update(wpSms.IdSms, wpSms);
        }
        private async Task<string> HandleFileAttachments(List<IFormFile> fileDinhKem, List<long> selectedFileIds, WpUsers user, long smsId, string domain)
        {
            var res = string.Empty;
            try
            {
                List<string> fileUrls = new List<string>();
                // Xử lý file đính kèm mới
                if (fileDinhKem != null && fileDinhKem.Count > 0)
                {
                    foreach (var file in fileDinhKem)
                    {
                        if (file.Length > 0)
                        {
                            var savedFile = await _wpFileService.SaveFile(file, user, "wp_sms", smsId);
                            if (savedFile != null)
                            {
                                fileUrls.Add(savedFile.FileUrl);
                            }
                        }
                    }
                }

                // Xử lý files đã chọn từ selectedFileIds
                if (selectedFileIds != null && selectedFileIds.Count > 0)
                {
                    var createdFiles = await _wpFileService.CreateFromFileExisted(selectedFileIds, user, "wp_sms", smsId);
                    fileUrls.AddRange(createdFiles.Select(f => f.FileUrl));
                }
                if (fileUrls != null && fileUrls.Count > 0)
                {
                    res = string.Join(" ", fileUrls.Select(f => (domain + f).Replace("\\", "/")));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xử lý file đính kèm: " + ex.Message);
            }
            return res;
        }

        private async Task SendMessageToCanbo(WpCanboViewModel canbo, long smsId, SmsRes res)
        {
            if (canbo != null && canbo.IdCanbo.HasValue && canbo.IdNhom.HasValue)
            {
                WpSmsCanbo wpSmsCanbo = new WpSmsCanbo()
                {
                    IdSms = smsId,
                    IdCanbo = canbo.IdCanbo.Value,
                    IdNhom = canbo.IdNhom.Value,
                    REQID = res.RPLY.REQID,
                    name = res.RPLY.name,
                    ERROR = res.RPLY.ERROR,
                    ERROR_DESC = res.RPLY.ERROR_DESC
                };
                await _wpSmsCanboRepository.Create(wpSmsCanbo);
            }
        }
        public async Task<PageResult<WpSmsViewModel>> SearchMessage(WpSmsSearchViewModel model, Pageable pageable)
        {
            IQueryable<WpSms> wpSms = await _wpSmsRepository.Search(model.searchInput);

            if (model.dateFrom.HasValue && model.dateTo.HasValue)
            {
                wpSms = wpSms.Where(wps => wps.Ngaygui >= model.dateFrom && wps.Ngaygui <= model.dateTo);
            }

            int total = await wpSms.CountAsync();

            IEnumerable<WpSms> wpSmsPage = await _wpSmsRepository.GetPagination(wpSms, pageable);

            var wpFileList = await _wpFileService.GetByBangLuuFile("wp_sms");
            var wpUserList = await _wpUsersRepository.GetAll();

            var wpSmsViewModel = from wps in wpSmsPage
                                 join wpf in wpFileList on wps.IdSms equals wpf.BangLuuFileId into wpsWithFileGroup
                                 from gwpf in wpsWithFileGroup.DefaultIfEmpty()
                                 join wpu in wpUserList on wps.IdNguoigui equals wpu.Id
                                 group new { wps, gwpf, wpu } by new { wps, wpu } into wpsGroup
                                 select new WpSmsViewModel
                                 {
                                     IdSms = wpsGroup.Key.wps.IdSms,
                                     Noidung = wpsGroup.Key.wps.Noidung,
                                     FileDinhKem = wpsGroup.Where(x => x.gwpf != null).Select(x => x.gwpf).ToList(),
                                     IdNguoigui = wpsGroup.Key.wps.IdNguoigui,
                                     TenNguoigui = wpsGroup.Key.wpu.UserName,
                                     Ngaygui = wpsGroup.Key.wps.Ngaygui,
                                     SoTn = wpsGroup.Key.wps.SoTn,
                                     SoTnLoi = wpsGroup.Key.wps.SoTnLoi
                                 };

            return new PageResult<WpSmsViewModel>
            {
                Data = wpSmsViewModel,
                Total = total,
            };
        }

        public async Task UpdateFile(WpFile oldFile, IFormFile fileDinhKem)
        {
            await _wpFileService.UpdateContentFile(fileDinhKem, oldFile, new WpUsers());
        }

    }
}
