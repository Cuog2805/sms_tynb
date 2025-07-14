namespace SMS_TYNB.Common
{
	public class Pageable
	{
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public string? Sort { get; set; }
	}

	public class PageResult<T>
	{
		public IEnumerable<T> Data { get; set; } = [];
		public int Total { get; set; }
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public int TotalPages => Total > 0 ? (int)Math.Ceiling((double)Total / PageSize) : 0;
	}
}
