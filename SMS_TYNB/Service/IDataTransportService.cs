using SMS_TYNB.Service.Implement;

namespace SMS_TYNB.Service
{
	public interface IDataTransportService
	{
		Task<FileDownloadResult> DownloadSampleExcel(string fileName, string folderPath);
	}
}
