namespace VnptSmsBrandName.Common
{
	/// <summary>
	/// k?t qu? tr? v? t? service, service ch? 1 lo?i d? li?u
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
