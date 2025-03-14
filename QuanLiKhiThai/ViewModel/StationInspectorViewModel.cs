namespace QuanLiKhiThai.ViewModel
{
    public class StationInspectorViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public string Status => IsActive ? "Active" : "Inactive";
        public DateTime? AssignedDate { get; set; }
        public string Notes { get; set; }
    }
}
