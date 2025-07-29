namespace SMS_TYNB.Common
{
	/// <summary>
	/// kết quả trả về từ service, service chỉ 1 loại dữ liệu
	/// </summary>
	public class ServiceResult<T> where T : class
	{
		public bool IsSuccess { get; set; }
		public string ErrorMessage { get; set; }
		public T Data { get; set; }
		public Exception Exception { get; set; }

		public static ServiceResult<T> Success(T data)
		{
			return new ServiceResult<T> { IsSuccess = true, Data = data };
		}

		public static ServiceResult<T> Failure(string errorMessage, Exception ex = null)
		{
			return new ServiceResult<T>
			{
				IsSuccess = false,
				ErrorMessage = errorMessage,
				Exception = ex
			};
		}
	}
}
