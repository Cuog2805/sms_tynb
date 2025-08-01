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
	public class MGroupService: IMGroupService
	{
		private readonly MGroupRepository _mGroupRepository;
		private readonly MGroupEmployeeRepository _mGroupEmployeeRepository;
		private readonly MEmployeeRepository _mEmployeeRepository;
		private readonly IMEmployeeService _mEmployeeService;
		public MGroupService
		(
			MGroupRepository mGroupRepository,
			MGroupEmployeeRepository mGroupEmployeeRepository,
			MEmployeeRepository mEmployeeRepository,
			IMEmployeeService mEmployeeService
		)
		{
			_mGroupRepository = mGroupRepository;
			_mGroupEmployeeRepository = mGroupEmployeeRepository;
			_mEmployeeRepository = mEmployeeRepository;
			_mEmployeeService = mEmployeeService;
		}
		public async Task<MGroup> Create(MGroup model, Users user)
		{
			AuditHelper.SetCreateAudit(model, user);
			MGroup mGroup = await _mGroupRepository.Create(model);
			return mGroup;
		}

		public async Task<MGroup?> Update(MGroup model, Users user)
		{
			AuditHelper.SetUpdateAudit(model, user);
			MGroup? mGroup = await _mGroupRepository.Update(model.GroupId, model);
			return mGroup;
		}

		public async Task<IEnumerable<MGroup>> GetMGroupList(long orgId)
		{
			IEnumerable<MGroup> mGroups = _mGroupRepository.GetAllByOrgId(orgId);
			return mGroups;
		}

		public async Task<MGroup?> GetByIdAndOrgId(long id, long orgId)
		{
			MGroup? mGroup = await _mGroupRepository.FindByIdAndOrgId(id, orgId);
			return mGroup;
		}
		public async Task<PageResult<MGroupViewModel>> SearchMGroup(MGroupSearchViewModel model, Pageable pageable, long orgId)
		{
			IQueryable<MGroup> mGroups = await _mGroupRepository.Search(model.searchInput, orgId);
			IEnumerable<MGroup> mGroupsPage = await _mGroupRepository.GetPagination(mGroups, pageable);

			var deletedMapping = EnumHelper.ToDictionary<DeletedEnum>();
			var mGroupViewModels = from mgroup in mGroupsPage
								   join mgroupParent in mGroups on mgroup.GroupParentId equals mgroupParent.GroupId into mgroupGroup
								   from mgroupParent in mgroupGroup.DefaultIfEmpty()
								   where (mgroup.IsDeleted == model.IsDeleted || model.IsDeleted == null)
								   select new MGroupViewModel
								   {
									   GroupId = mgroup.GroupId,
									   GroupParentId = mgroup.GroupParentId,
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

		public async Task<List<MGroupViewModel>> GetAllMGroupEmployees(MGroupSearchViewModel model, long orgId)
		{
			IQueryable<MEmployee> mEmployees = await _mEmployeeRepository.Search(model.searchInput, orgId);
			var mGroups = _mGroupRepository.GetAllByOrgId(orgId);

			if (model.IsDeleted.HasValue)
			{
				mEmployees = mEmployees.Where(item => item.IsDeleted == model.IsDeleted);
				mGroups = mGroups.Where(item => item.IsDeleted == model.IsDeleted);
			}

			var groupEmployees = _mGroupEmployeeRepository.GetAllByOrgId(orgId);

			var res = (from mgroup in mGroups
					   join mgroup_emp in groupEmployees on mgroup.GroupId equals mgroup_emp.GroupId into mgroupGroup
					   from mgroup_emp in mgroupGroup.DefaultIfEmpty()
					   join emp in mEmployees on mgroup_emp.EmployeeId equals emp.EmployeeId into empGroup
					   from emp in empGroup.DefaultIfEmpty()
					   group new { emp, mgroup } by new { mgroup.GroupId, mgroup.GroupParentId, mgroup.Name } into mgroup_empGroup
					   select new MGroupViewModel
					   {
						   GroupId = mgroup_empGroup.Key.GroupId,
						   GroupParentId = mgroup_empGroup.Key.GroupParentId,
						   Name = mgroup_empGroup.Key.Name,
						   Employees = mgroup_empGroup.Where(item => item.emp != null)
							   .Select(item => new MEmployeeViewModel
							   {
								   EmployeeId = item.emp.EmployeeId,
								   Name = item.emp.Name,
								   PhoneNumber = item.emp.PhoneNumber,
								   Description = item.emp.Description,
								   IdGroup = mgroup_empGroup.Key.GroupId,
								   GroupName = mgroup_empGroup.Key.Name,
							   }).ToList(),
					   }).ToList();

			return res;
		}

		public async Task<MGroupViewModel> GetGroupEmployeeByIdAndOrgId(long id, long orgId)
		{
			var mGroups = _mGroupRepository.GetAllByOrgId(orgId);
			var mEmployees = _mEmployeeRepository.GetAllByOrgId(orgId);
			var mGroup = await _mGroupRepository.FindById(id);

			var groupEmployees = _mGroupEmployeeRepository.GetAllByOrgId(orgId);

			List<MEmployeeViewModel> employees = (from emp in mEmployees
												  join mgroup_emp in groupEmployees on emp.EmployeeId equals mgroup_emp.EmployeeId
												  select new MEmployeeViewModel
												  {
													  EmployeeId = emp.EmployeeId,
													  Name = emp.Name,
													  PhoneNumber = emp.PhoneNumber,
													  Description = emp.Description,
												  }).ToList();

			return new MGroupViewModel()
			{
				GroupId = mGroup.GroupId,
				Name = mGroup.Name,
				Employees = employees,
			};
		}

		public async Task<MGroupViewModel> Assign(MGroupViewModel model, Users user)
		{
			await _mGroupEmployeeRepository.DeleteByGroupId(model.GroupId);
			foreach (var item in model.Employees)
			{
				var nhomCanbo = new MGroupEmployee
				{
					GroupId = model.GroupId,
					EmployeeId = item.EmployeeId.Value,
					OrganizationId = user.OrganizationId,
				};
				AuditHelper.SetCreateAudit(nhomCanbo, user);
				await _mGroupEmployeeRepository.Create(nhomCanbo);
			}

			return model;
		}
	}

	public interface IMGroupService
	{
		Task<MGroup> Create(MGroup model, Users user);
		Task<MGroup?> Update(MGroup model, Users user);
		Task<IEnumerable<MGroup>> GetMGroupList(long orgId);
		Task<MGroup?> GetByIdAndOrgId(long id, long orgId);
		Task<PageResult<MGroupViewModel>> SearchMGroup(MGroupSearchViewModel model, Pageable pageable, long orgId);
		Task<List<MGroupViewModel>> GetAllMGroupEmployees(MGroupSearchViewModel model, long orgId);
		Task<MGroupViewModel> GetGroupEmployeeByIdAndOrgId(long id, long orgId);
		Task<MGroupViewModel> Assign(MGroupViewModel model, Users user);
	}
}
