namespace DMCPortal.Web.Models
{
    public class LoginResponse
    {
        public Guid SessionId { get; set; }
        public List<string> Operations { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
