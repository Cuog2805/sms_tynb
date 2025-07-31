using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SMS_TYNB.Models.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace SMS_TYNB.Areas.Identity.Pages.Account.Manage
{
	[Authorize]
	public class ChangePasswordModel<TUser> : PageModel where TUser : class
	{
		private readonly UserManager<TUser> _userManager;
		private readonly SignInManager<TUser> _signInManager;
		private readonly ILogger<ChangePasswordModel<TUser>> _logger;

		public ChangePasswordModel(
			UserManager<TUser> userManager,
			SignInManager<TUser> signInManager,
			ILogger<ChangePasswordModel<TUser>> logger)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_logger = logger;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		[TempData]
		public string StatusMessage { get; set; }

		public class InputModel
		{
			[Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc")]
			[DataType(DataType.Password)]
			[Display(Name = "Mật khẩu hiện tại")]
			public string OldPassword { get; set; }

			[Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
			[StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự và tối đa {1} ký tự.", MinimumLength = 6)]
			[DataType(DataType.Password)]
			[Display(Name = "Mật khẩu mới")]
			public string NewPassword { get; set; }

			[DataType(DataType.Password)]
			[Display(Name = "Xác nhận mật khẩu mới")]
			[Compare("NewPassword", ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu không khớp.")]
			public string ConfirmPassword { get; set; }
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Không thể tải người dùng có ID '{_userManager.GetUserId(User)}'.");
			}

			var hasPassword = await _userManager.HasPasswordAsync(user);
			if (!hasPassword)
			{
				return RedirectToPage("./SetPassword");
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Không thể tải người dùng có ID '{_userManager.GetUserId(User)}'.");
			}

			var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
			if (!changePasswordResult.Succeeded)
			{
				foreach (var error in changePasswordResult.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
				return Page();
			}

			await _signInManager.RefreshSignInAsync(user);
			_logger.LogInformation("Người dùng đã thay đổi mật khẩu thành công.");
			StatusMessage = "Mật khẩu của bạn đã được thay đổi thành công.";

			return RedirectToPage();
		}
	}

	public class ChangePasswordModel : ChangePasswordModel<Users>
	{
		public ChangePasswordModel(
			UserManager<Users> userManager,
			SignInManager<Users> signInManager,
			ILogger<ChangePasswordModel<Users>> logger) : base(userManager, signInManager, logger)
		{
		}
	}
}

