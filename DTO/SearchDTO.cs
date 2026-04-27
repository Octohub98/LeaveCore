namespace LeaveCore.DTO
{
    public class SearchDTO
    {
        private int _recordPerPage = 20;
        public int Page { get; set; } = 1;
        public int RecordPerPage
        {
            get => _recordPerPage;
            set => _recordPerPage = value > 150 ? 150 : value;
        }
        public int Take => RecordPerPage;
        public int Skip => (Page - 1) * RecordPerPage;
    }
}
