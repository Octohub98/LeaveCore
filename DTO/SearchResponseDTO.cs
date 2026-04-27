namespace LeaveCore.DTO
{
    public class SearchResponseDTO
    {
        public int Total { get; set; }
        public object Data { get; set; } = default!;
    }
}
