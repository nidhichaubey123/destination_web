using System.ComponentModel.DataAnnotations;

namespace DMCPortal.Web.Models
{
    public class Agent
    {
  
        public int AgentId { get; set; }

      

        public string AgentName { get; set; }

        public string AgentPoc1 { get; set; }
        public string Agency_Company { get; set; }


        public string phoneno { get; set; }

        public string emailAddress { get; set; }
        public string Zone { get; set; }

        public string AgentAddress { get; set; }

    }

}
