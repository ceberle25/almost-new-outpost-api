namespace AlmostNewOutpost.Api.Models.Responses
{

    /// <summary>
    /// AdminUserDto stores a users ID, username, role ID, role name,
    /// archived date, and it helps organize and transfer data.
    /// </summary>
    public class AdminUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime? ArchivedAtUtc { get; set; }
    }
}
