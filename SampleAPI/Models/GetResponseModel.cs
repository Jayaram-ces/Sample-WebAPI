namespace SampleAPI.Models
{
    public class GetResponseModel<T>
    {
        public int TotalRecordCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
