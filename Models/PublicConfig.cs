using Newtonsoft.Json;
using System;
using System.Text;

namespace AndroidSideloader.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PublicConfig
    {
        [JsonProperty("baseUri")]
        public string BaseUri { get; set; }

        private string password;

        [JsonProperty("password")]
        public string Password
        {
            get => password;
            set => password = Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }
    }
}
