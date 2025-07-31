using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VnptSmsBrandName.Models.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace VnptSmsBrandName.Areas.Identity.Pages.Account.Manage
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
			[Required(ErrorMessage = "M?t kh?u hi?n t?i là b?t bu?c")]
			[DataType(DataType.Password)]
			[Display(Name = "M?t kh?u hi?n t?i")]
			public string OldPassword { get; set; }

			[Required(ErrorMessage = "M?t kh?u m?i là b?t bu?c")]
			[StringLength(100, ErrorMessage = "M?t kh?u ph?i có ít nh?t {2} ký t? và t?i da {1} ký t?.", MinimumLength = 6)]
			[DataType(DataType.Password)]
			[Display(Name = "M?t kh?u m?i")]
			public string NewPassword { get; set; }

			[DataType(DataType.Password)]
			[Display(Name = "Xác nh?n m?t kh?u m?i")]
			[Compare("NewPassword", ErrorMessage = "M?t kh?u m?i và xác nh?n m?t kh?u không kh?p.")]
			public string ConfirmPassword { get; set; }
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Không th? t?i ngu?i dùng có ID '{_userManager.GetUserId(User)}'.");
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
				return NotFound($"Không th? t?i ngu?i dùng có ID '{_userManager.GetUserId(User)}'.");
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
			_logger.LogInformation("Ngu?i dùng dã thay d?i m?t kh?u thành công.");
			StatusMessage = "M?t kh?u c?a b?n dã du?c thay d?i thành công.";

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

