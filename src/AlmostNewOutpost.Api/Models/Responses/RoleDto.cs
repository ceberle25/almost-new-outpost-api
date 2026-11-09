namespace AlmostNewOutpost.Api.Models.Responses
{

    /// <summary>
    /// RoleDto stores a roles ID and name, it helps organize and transfer data.
    /// </summary>

    public class RoleDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
