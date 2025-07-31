using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS_TYNB.Models.Identity
{
	public class Users : IdentityUser
	{
		public long OrgId { get; set; }
		public long UserId { get; set; } 
		public string UserRole { get; set; } 
		public string FullName { get; set; } 
		public int State { get; set; }
		
	}
}
