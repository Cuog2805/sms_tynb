using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Common;
using SMS_TYNB.Common.Enum;
using SMS_TYNB.Helper;
using SMS_TYNB.Models.Master;
using SMS_TYNB.Repository;
using SMS_TYNB.ViewModel;

namespace SMS_TYNB.Service
{
	public class MGroupService: BaseService, IMGroupService
	{
		private readonly MGroupRepository _mGroupRepository;
		private readonly MGroupEmployeeRepository _mGroupEmployeeRepository;
		private readonly MEmployeeRepository _mEmployeeRepository;
		private readonly IMEmployeeService _mEmployeeService;
		public MGroupService
		(
			ICurrentUserService currentUserService,
			MGroupRepository mGroupRepository,
			MGroupEmployeeRepository mGroupEmployeeRepository,
			MEmployeeRepository mEmployeeRepository,
			IMEmployeeService mEmployeeService
		): base(currentUserService)
		{
			_mGroupRepository = mGroupRepository;
			_mGroupEmployeeRepository = mGroupEmployeeRepository;
			_mEmployeeRepository = mEmployeeRepository;
			_mEmployeeService = mEmployeeService;
		}
		public async Task<MGroup> Create(MGroup model)
		{
			await SetCreateAudit(model);
			MGroup mGroup = await _mGroupRepository.Create(model);
			return mGroup;
		}

		public async Task<MGroup?> Update(MGroup model)
		{
			await SetUpdateAudit(model);
			MGroup? mGroup = await _mGroupRepository.Update(model.IdGroup, model);
			return mGroup;
		}

		public async Task Delete(MGroup model)
		{
			await _mGroupRepository.Delete(model.IdGroup);
		}

		public async Task<IEnumerable<MGroup>> GetAllMGroup()
		{
			var user = await _currentUserService.GetCurrentUser();
			IEnumerable<MGroup> mGroups = _mGroupRepository.Query().Where(item => item.IdOrganization == user.OrgId);
			return mGroups;
		}

		public async Task<MGroup?> GetById(int id)
		{
			MGroup? mGroup = await _mGroupRepository.FindById(id);
			return mGroup;
		}
		public async Task<PageResult<MGroupViewModel>> SearchMGroup(MGroupSearchViewModel model, Pageable pageable)
		{
			var user = await _currentUserService.GetCurrentUser();
			IQueryable<MGroup> mGroups = await _mGroupRepository.Search(model.searchInput, user.OrgId);
			IEnumerable<MGroup> mGroupsPage = await _mGroupRepository.GetPagination(mGroups, pageable);

			var deletedMapping = EnumHelper.ToDictionary<DeletedEnum>();
			var mGroupViewModels = from mgroup in mGroupsPage
								   join mgroupParent in mGroups on mgroup.IdGroupParent equals mgroupParent.IdGroup into mgroupGroup
								   from mgroupParent in mgroupGroup.DefaultIfEmpty()
								   where (mgroup.IsDeleted == model.IsDeleted || model.IsDeleted == null)
								   select new MGroupViewModel
								   {
									   IdGroup = mgroup.IdGroup,
									   IdGroupParent = mgroup.IdGroupParent,
									   Name = mgroup.Name,
									   ParentName = mgroupParent?.Name ?? "",
									   IsDeleted = deletedMapping[mgroup.IsDeleted],
								   };

			return new PageResult<MGroupViewModel>
			{
				Data = mGroupViewModels,
				Total = await mGroups.CountAsync()
			};
		}

		public async Task<List<MGroupViewModel>> GetAllMGroupEmployees(MGroupSearchViewModel model)
		{
			var user = await _currentUserService.GetCurrentUser();

			var employeeQuery = await _mEmployeeRepository.Search(model.searchInput, user.OrgId);

			var employees = await employeeQuery.ToListAsync();
			var mGroupList = await GetAllMGroup();
			var groupEmployees = await _mGroupEmployeeRepository.Query().ToListAsync();

			var res = (from mgroup in mGroupList
					   join mgroup_emp in groupEmployees on mgroup.IdGroup equals mgroup_emp.IdGroup into mgroupGroup
					   from mgroup_emp in mgroupGroup.DefaultIfEmpty()
					   join emp in employees on mgroup_emp?.IdEmployee equals emp.IdEmployee into empGroup
					   from emp in empGroup.DefaultIfEmpty()
					   where mgroup.IsDeleted == model.IsDeleted || model.IsDeleted == null
					   group new { emp, mgroup } by new { mgroup.IdGroup, mgroup.IdGroupParent, mgroup.Name } into mgroup_empGroup
					   select new MGroupViewModel
					   {
						   IdGroup = mgroup_empGroup.Key.IdGroup,
						   IdGroupParent = mgroup_empGroup.Key.IdGroupParent,
						   Name = mgroup_empGroup.Key.Name,
						   Employees = mgroup_empGroup.Where(item => item.emp != null)
							   .Select(item => new MEmployeeViewModel
							   {
								   IdEmployee = item.emp.IdEmployee,
								   Name = item.emp.Name,
								   PhoneNumber = item.emp.PhoneNumber,
								   Description = item.emp.Description,
								   IdGroup = mgroup_empGroup.Key.IdGroup,
								   GroupName = mgroup_empGroup.Key.Name,
							   }).ToList(),
					   }).ToList();

			return res;
		}

		public async Task<MGroupViewModel> GetGroupEmployeeById(int id)
		{
			var user = await _currentUserService.GetCurrentUser();

			var mGroupList = await GetAllMGroup();
			var mEmployeeList = await _mEmployeeService.GetAllMEmployee();
			var mGroup = await _mGroupRepository.FindById(id);

			var groupEmployees = await _mGroupEmployeeRepository.Query()
				.Where(ge => ge.IdGroup == mGroup.IdGroup)
				.ToListAsync();

			List<MEmployeeViewModel> employees = (from emp in mEmployeeList
												  join mgroup_emp in groupEmployees on emp.IdEmployee equals mgroup_emp.IdEmployee
												  select new MEmployeeViewModel
												  {
													  IdEmployee = emp.IdEmployee,
													  Name = emp.Name,
													  PhoneNumber = emp.PhoneNumber,
													  Description = emp.Description,
												  }).ToList();

			return new MGroupViewModel()
			{
				IdGroup = mGroup.IdGroup,
				Name = mGroup.Name,
				Employees = employees,
			};
		}

		public async Task<MGroupViewModel> Assign(MGroupViewModel model)
		{
			await _mGroupEmployeeRepository.DeleteByGroupId(model.IdGroup);
			foreach (var item in model.Employees)
			{
				var wpNhomCanbo = new MGroupEmployee
				{
					IdGroup = model.IdGroup,
					IdEmployee = item.IdEmployee.Value
				};

				await _mGroupEmployeeRepository.Create(wpNhomCanbo);
			}

			return model;
		}
	}

	public interface IMGroupService
	{
		Task<MGroup> Create(MGroup model);
		Task<MGroup?> Update(MGroup model);
		Task Delete(MGroup model);
		Task<IEnumerable<MGroup>> GetAllMGroup();
		Task<MGroup?> GetById(int id);
		Task<PageResult<MGroupViewModel>> SearchMGroup(MGroupSearchViewModel model, Pageable pageable);
		Task<List<MGroupViewModel>> GetAllMGroupEmployees(MGroupSearchViewModel model);
		Task<MGroupViewModel> GetGroupEmployeeById(int id);
		Task<MGroupViewModel> Assign(MGroupViewModel model);
	}
}
