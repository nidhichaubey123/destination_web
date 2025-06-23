namespace DMCPortal.Web.Models
{
    public class LoginResponse
    {
        public Guid SessionId { get; set; }
        public List<string> Operations { get; set; }
    }
}
