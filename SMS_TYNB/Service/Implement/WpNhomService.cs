using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Common;
using SMS_TYNB.Models.Master;
using SMS_TYNB.Repository;
using SMS_TYNB.ViewModel;
using System.Collections.Generic;

namespace SMS_TYNB.Service.Implement
{
	public class WpNhomService : IWpNhomService
	{
		private readonly WpNhomRepository _wpNhomRepository;
		private readonly WpNhomCanboRepository _wpNhomCanboRepository;
		private readonly WpDanhmucRepository _wpDanhmucRepository;
		private readonly WpCanboRepository _wpCanboRepository;
		public WpNhomService
		(
			WpNhomRepository wpNhomRepository,
			WpNhomCanboRepository wpNhomCanboRepository,
			WpDanhmucRepository wpDanhmucRepository,
			WpCanboRepository wpCanboRepository
		)
		{
			_wpNhomRepository = wpNhomRepository;
			_wpNhomCanboRepository = wpNhomCanboRepository;
			_wpDanhmucRepository = wpDanhmucRepository;
			_wpCanboRepository = wpCanboRepository;
		}
		public async Task<WpNhom> Create(WpNhom model)
		{
			WpNhom wpNhom = await _wpNhomRepository.Create(model);
			return wpNhom;
		}

		public async Task Delete(WpNhom model)
		{
			await _wpNhomRepository.Delete(model.IdNhom);
		}

		public async Task<IEnumerable<WpNhom>> GetAllWpNhom()
		{
			IEnumerable<WpNhom> wpNhoms = await _wpNhomRepository.GetAll();
			return wpNhoms;
		}

		public async Task<WpNhom?> GetById(int id)
		{
			WpNhom? wpNhom = await _wpNhomRepository.FindById(id);
			return wpNhom;
		}

		public async Task<List<WpNhomViewModel>> GetAllWpNhomCanbos(WpNhomSearchViewModel model)
		{
			var wpCanboSearch = _wpCanboRepository.Search(model.searchInput);
			var wpNhomCanbos = (from wpn in await _wpNhomRepository.GetAll()
								join wpcbn in await _wpNhomCanboRepository.GetAll() on wpn.IdNhom equals wpcbn.IdNhom into wpnGroup
								from wpcbn in wpnGroup.DefaultIfEmpty()
								join wpcb in await wpCanboSearch on wpcbn?.IdCanbo equals wpcb.IdCanbo into wpcbGroup
								from wpcb in wpcbGroup.DefaultIfEmpty()
								where (wpn.TrangThai == model.TrangThai || model.TrangThai == null)
								group new { wpcb, wpn } by new { wpn.IdNhom, wpn.IdNhomCha, wpn.TenNhom } into wpNhomCanboGroup
								select new WpNhomViewModel
								{
									IdNhom = wpNhomCanboGroup.Key.IdNhom,
									IdNhomCha = wpNhomCanboGroup.Key.IdNhomCha,
									TenNhom = wpNhomCanboGroup.Key.TenNhom,
									WpCanbos = wpNhomCanboGroup.Where(item => item.wpcb != null)
										.Select(item => new WpCanboViewModel
										{
											IdCanbo = item.wpcb.IdCanbo,
											TenCanbo = item.wpcb.TenCanbo,
											SoDt = item.wpcb.SoDt,
											Mota = item.wpcb.Mota,
											IdNhom = wpNhomCanboGroup.Key.IdNhom,
											TenNhom = wpNhomCanboGroup.Key.TenNhom,
										}).ToList(),
								}).ToList();
			return wpNhomCanbos;
		}

		public async Task<WpNhomViewModel> GetWpNhomCanbosById(int id)
		{
			WpNhom wpNhom = await _wpNhomRepository.FindById(id);
			List<WpCanboViewModel> wpcanbos = (List<WpCanboViewModel>)(from wpcb in await _wpCanboRepository.GetAll()
													 join wpcbn in await _wpNhomCanboRepository.GetAll() on wpcb.IdCanbo equals wpcbn.IdCanbo
													 where wpcbn.IdNhom == wpNhom.IdNhom
													 select new WpCanboViewModel
													 {
														 IdCanbo = wpcb.IdCanbo,
														 MaCanbo = wpcb.MaCanbo,
														 TenCanbo = wpcb.TenCanbo,
														 SoDt = wpcb.SoDt,
														 Mota = wpcb.Mota,
													 }).ToList();
			WpNhomViewModel wpNhomViewModel = new WpNhomViewModel()
			{
				IdNhom = wpNhom.IdNhom,
				TenNhom = wpNhom.TenNhom,
				WpCanbos = wpcanbos,
			};

			return wpNhomViewModel;
		}

		public async Task<PageResult<WpNhomViewModel>> SearchWpNhom(WpNhomSearchViewModel model, Pageable pageable)
		{
			IQueryable<WpNhom> wpNhoms = await _wpNhomRepository.Search(model.searchInput);
			IEnumerable<WpNhom> wpNhomsPage = await _wpNhomRepository.GetPagination(wpNhoms, pageable);

			var wpNhomsViewModel = from wpn in wpNhomsPage
								   join wpdm in await _wpDanhmucRepository.GetByType("TRANGTHAI") on wpn.TrangThai equals wpdm.MaDanhmuc
								   join wpnCha in wpNhoms on wpn.IdNhomCha equals wpnCha.IdNhom into groupWpn
								   from wpnCha in groupWpn.DefaultIfEmpty()
								   where (wpn.TrangThai == model.TrangThai || model.TrangThai == null)
								   select new WpNhomViewModel
								   {
									   IdNhom = wpn.IdNhom,
									   IdNhomCha = wpn.IdNhomCha,
									   TenNhom = wpn.TenNhom,
									   TenNhomCha = wpnCha?.TenNhom ?? "",
									   TrangThai = wpdm.TenDanhmuc,
								   };

			int total = await wpNhoms.CountAsync();

			return new PageResult<WpNhomViewModel>
			{
				Data = wpNhomsViewModel,
				Total = total,
			};
		}

		public async Task<WpNhom?> Update(WpNhom model)
		{
			WpNhom? wpNhom = await _wpNhomRepository.Update(model.IdNhom, model);
			return wpNhom;
		}

		public async Task<WpNhomViewModel> Assign(WpNhomViewModel model)
		{
			await _wpNhomCanboRepository.DeleteByWpNhomId(model.IdNhom);
			foreach (var item in model.WpCanbos)
			{
				var wpNhomCanbo = new WpNhomCanbo
				{
					IdNhom = model.IdNhom,
					IdCanbo = item.IdCanbo.Value
				};

				await _wpNhomCanboRepository.Create(wpNhomCanbo);
			}

			return model;
		}

	}
}
