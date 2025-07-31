using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Common;
using SMS_TYNB.Common.Enum;
using SMS_TYNB.Helper;
using SMS_TYNB.Models.Master;
using SMS_TYNB.Repository;
using SMS_TYNB.ViewModel;

namespace SMS_TYNB.Service
{
	public class MEmployeeService: BaseService, IMEmployeeService
	{
		private readonly MEmployeeRepository _employeeRepository;
		public MEmployeeService(MEmployeeRepository employeeRepository, ICurrentUserService currentUserService) : base(currentUserService)
		{
			_employeeRepository = employeeRepository;
		}
		public async Task<IEnumerable<MEmployee>> GetAllMEmployee()
		{
			var user = await _currentUserService.GetCurrentUser();
			IEnumerable<MEmployee> mEmployees = _employeeRepository.Query().Where(x => x.IdOrganization == user.OrgId);
			return mEmployees;
		}
		public async Task<MEmployee?> GetById(int mEmployeeId)
		{
			MEmployee? mEmployee = await _employeeRepository.FindById(mEmployeeId);
			return mEmployee;
		}
		public async Task<MEmployee> Create(MEmployee mEmployee)
		{
			await SetCreateAudit(mEmployee);

			MEmployee mEmployeeNew = await _employeeRepository.Create(mEmployee);
			return mEmployeeNew;
		}
		public async Task<MEmployee?> Update(MEmployee mEmployee)
		{
			await SetUpdateAudit(mEmployee);

			MEmployee? mEmployeeUpdated = await _employeeRepository.Update(mEmployee.IdEmployee, mEmployee);
			return mEmployeeUpdated;
		}

		public async Task<PageResult<MEmployeeViewModel>> SearchMEmployee(MEmployeeSearchViewModel model, Pageable pageable)
		{
			var user = await _currentUserService.GetCurrentUser();
			IQueryable<MEmployee> mEmployees = await _employeeRepository.Search(model.searchInput, user.OrgId);
			var mEmployeesPage = await _employeeRepository.GetPagination(mEmployees, pageable);

			var genderMapping = EnumHelper.ToDictionary<GenderEnum>();
			var deletedMapping = EnumHelper.ToDictionary<DeletedEnum>();

			var mEmployeesViewModel = from mEmployee in mEmployeesPage
									  where mEmployee.IsDeleted == model.IsDeleted || model.IsDeleted == null
									  select new MEmployeeViewModel
									  {
										  IdEmployee = mEmployee.IdEmployee,
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
		
		public async Task<MEmployeeCreateRangeViewModel> CreateMulti(List<MEmployee> mEmployees)
		{
			var user = await _currentUserService.GetCurrentUser();
			var phoneNumbers = mEmployees.Select(item => item.PhoneNumber).ToList();

			var existedCanbos = await _employeeRepository.FindByPhoneNumbersAndIdOrganization(phoneNumbers, user.OrgId);
			var existedPhoneNumbers = existedCanbos.Select(cb => cb.PhoneNumber).ToList();

			var genderMapping = EnumHelper.ToDictionary<GenderEnum>();
			var newCanbos = mEmployees.Where(cb => !existedPhoneNumbers.Contains(cb.PhoneNumber)).ToList();
			// audit created
			foreach (var employee in newCanbos)
			{
				await SetCreateAudit(employee);
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
		Task<IEnumerable<MEmployee>> GetAllMEmployee();
		Task<PageResult<MEmployeeViewModel>> SearchMEmployee(MEmployeeSearchViewModel model, Pageable pageable);
		Task<MEmployee?> GetById(int mEmployeeId);
		Task<MEmployee> Create(MEmployee mEmployee);
		Task<MEmployee?> Update(MEmployee mEmployee);
		Task<MEmployeeCreateRangeViewModel> CreateMulti(List<MEmployee> mEmployees);
	}
}
