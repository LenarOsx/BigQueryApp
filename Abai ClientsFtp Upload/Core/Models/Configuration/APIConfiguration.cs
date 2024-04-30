using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Configuration
{
    public class APIConfiguration
    {
        public string? Version { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? TermsOfService { get; set; }
        public APIContact Contact { get; set; }
        public APILicense License { get; set; }
        public APIAuthentication Authentication { get; set; }

        public APIConfiguration()
        {
            Version = "";
            Title = "";
            Description = "";
            TermsOfService = "";

            Contact = new();
            License = new();
            Authentication = new();
        }
    }

    public class APIContact
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Url { get; set; }

        public APIContact()
        {
            Name = "";
            Email = "";
            Url = "";
        }
    }

    public class APILicense
    {
        public string? Name { get; set; }
        public string? Url { get; set; }

        public APILicense()
        {
            Name = "";
            Url = "";
        }
    }

    public class APIAuthentication
    {
        public int Type { get; set; }
        public APIAuthenticationBasic Basic { get; set; }
        public string? Description { get; set; }

        public APIAuthentication()
        {
            Description = "";
            Basic = new();
        }
    }

    public class APIAuthenticationBasic
    {
        public string? Username { get; set; }
        public string? Password { get; set; }

        public APIAuthenticationBasic()
        {
            Username = "";
            Password = "";
        }
    }
}

