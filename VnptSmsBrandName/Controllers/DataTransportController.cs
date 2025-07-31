using Microsoft.AspNetCore.Mvc;
using VnptSmsBrandName.Service;
using System.Threading.Tasks;

namespace VnptSmsBrandName.Controllers
{
	public class DataTransportController : Controller
	{
		private readonly IDataTransportService _dataTransportService;
		public DataTransportController(IDataTransportService dataTransportService)
		{
			_dataTransportService = dataTransportService;
		}
		[HttpGet]
		public async Task<IActionResult> DownloadSample(string filename)
		{
			var data = await _dataTransportService.DownloadSampleExcel(filename, "template");
			//return Json(new
			//{
			//	state = "success",
			//	msg = "DownloadSample thành công!",
			//	content = new
			//	{
			//		data.FileContent,
			//		data.ContentType,
			//		data.FileName
			//	}
			//});
			return File(data.FileContent, data.ContentType, data.FileName);
		}


	}
}
