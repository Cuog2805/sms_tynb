using Microsoft.EntityFrameworkCore;
using VnptSmsBrandName.Common;
using VnptSmsBrandName.Common.Enum;
using VnptSmsBrandName.Helper;
using VnptSmsBrandName.Models.Identity;
using VnptSmsBrandName.Models.Master;
using VnptSmsBrandName.Repository;
using VnptSmsBrandName.ViewModel;

namespace VnptSmsBrandName.Service
{
	public class MEmployeeService: IMEmployeeService
	{
		private readonly MEmployeeRepository _employeeRepository;
		public MEmployeeService(MEmployeeRepository employeeRepository)
		{
			_employeeRepository = employeeRepository;
		}
		public async Task<IEnumerable<MEmployee>> GetAllMEmployee(long orgId)
		{
			IEnumerable<MEmployee> mEmployees = _employeeRepository.GetAllByOrgId(orgId);
			return mEmployees;
		}
		public async Task<MEmployee?> GetByIdAndOrgId(long mEmployeeId, long orgId)
		{
			MEmployee? mEmployee = await _employeeRepository.FindByIdAndOrgId(mEmployeeId, orgId);
			return mEmployee;
		}
		public async Task<MEmployee> Create(MEmployee mEmployee, Users user)
		{
			if(!CommonHelper.IsValidPhoneNumber(mEmployee.PhoneNumber))
			{
				throw new ArgumentNullException(nameof(mEmployee.PhoneNumber), "Số điện thoại không đúng định dạng");
			}
			AuditHelper.SetCreateAudit(mEmployee, user);
			MEmployee mEmployeeNew = await _employeeRepository.Create(mEmployee);
			return mEmployeeNew;
		}
		public async Task<MEmployee?> Update(MEmployee mEmployee, Users user)
		{
			AuditHelper.SetUpdateAudit(mEmployee, user);
			MEmployee? mEmployeeUpdated = await _employeeRepository.Update(mEmployee.EmployeeId, mEmployee);
			return mEmployeeUpdated;
		}

		public async Task<PageResult<MEmployeeViewModel>> SearchMEmployee(MEmployeeSearchViewModel model, Pageable pageable, long orgId)
		{
			IQueryable<MEmployee> mEmployees = await _employeeRepository.Search(model.searchInput, orgId);

			if (model.IsDeleted.HasValue)
			{
				mEmployees = mEmployees.Where(item => item.IsDeleted == model.IsDeleted);
			}

			var mEmployeesPage = await _employeeRepository.GetPagination(mEmployees, pageable);

			var genderMapping = EnumHelper.ToDictionary<GenderEnum>();
			var deletedMapping = EnumHelper.ToDictionary<DeletedEnum>();

			var mEmployeesViewModel = from mEmployee in mEmployeesPage
									  select new MEmployeeViewModel
									  {
										  EmployeeId = mEmployee.EmployeeId,
										  Name = mEmployee.Name,
										  PhoneNumber = mEmployee.PhoneNumber,
										  Description = mEmployee.Description,
										  Gender = genderMapping[mEmployee.Gender],
										  IsDeleted = deletedMapping[mEmployee.IsDeleted]
									  };

			int total = await mEmployees.CountAsync();

			return new PageResult<MEmployeeViewModel>
			{
				Data = mEmployeesViewModel,
				Total = total,
			};
		}
		
		public async Task<MEmployeeCreateRangeViewModel> CreateMulti(List<MEmployee> mEmployees, Users user)
		{
			var phoneNumbers = mEmployees.Select(item => item.PhoneNumber).ToList();

			var existedCanbos = await _employeeRepository.FindByPhoneNumbersAndIdOrganization(phoneNumbers, user.OrganizationId);
			var existedPhoneNumbers = existedCanbos.Select(cb => cb.PhoneNumber).ToList();

			var genderMapping = EnumHelper.ToDictionary<GenderEnum>();
			var newCanbos = mEmployees.Where(cb => !existedPhoneNumbers.Contains(cb.PhoneNumber)).ToList();
			// audit created
			foreach (var employee in newCanbos)
			{
				AuditHelper.SetCreateAudit(employee, user);
			}

			var duplicateCanbos = mEmployees.Where(cb => existedPhoneNumbers.Contains(cb.PhoneNumber)).ToList();

			newCanbos = await _employeeRepository.CreateRange(newCanbos);

			return new MEmployeeCreateRangeViewModel()
			{
				MEmployeeNew = newCanbos.Select(cb => new MEmployeeViewModel()
				{
					Name = cb.Name,
					PhoneNumber = cb.PhoneNumber,
					Gender = genderMapping[cb.Gender],
					Description = cb.Description,
				})
				.ToList(),
				MEmployeeDupplicate = duplicateCanbos.Select(cb => new MEmployeeViewModel()
				{
					Name = cb.Name,
					PhoneNumber = cb.PhoneNumber,
					Gender = genderMapping[cb.Gender],
					Description = cb.Description,
				})
				.ToList()
			};
		}
	}

	public interface IMEmployeeService
	{
		Task<IEnumerable<MEmployee>> GetAllMEmployee(long orgId);
		Task<PageResult<MEmployeeViewModel>> SearchMEmployee(MEmployeeSearchViewModel model, Pageable pageable, long orgId);
		Task<MEmployee?> GetByIdAndOrgId(long mEmployeeId, long orgId);
		Task<MEmployee> Create(MEmployee mEmployee, Users user);
		Task<MEmployee?> Update(MEmployee mEmployee, Users user);
		Task<MEmployeeCreateRangeViewModel> CreateMulti(List<MEmployee> mEmployees, Users user);
	}
}
