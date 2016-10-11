using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json.Linq;

namespace Microsoft.Owin.Security.WeChat
{
    public class WeChatAuthenticatedContext : BaseContext
    {
        public WeChatAuthenticatedContext(IOwinContext context,string openId, JObject user, string accessToken)
            :base(context)
        {
            IDictionary<string, JToken> userAsDictionary = user;

            User = user;
            AccessToken = accessToken;

            Id = openId;
            Name = PropertyValueIfExists("nickname", userAsDictionary);
            City = PropertyValueIfExists("city", userAsDictionary);
            Province = PropertyValueIfExists("province", userAsDictionary);
            Gendar= PropertyValueIfExists("sex", userAsDictionary);
            Country = PropertyValueIfExists("country", userAsDictionary);
            AvtorUrl = PropertyValueIfExists("headimgurl", userAsDictionary);
        }

        public JObject User { get; private set; }
        public string AccessToken { get; private set; }
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string City { get; private set; }
        public string Province { get; private set; }
        public string Country { get; set; }
        public string Gendar { get; private set; }
        public string AvtorUrl { get; private set; }
        public ClaimsIdentity Identity { get; set; }
        public AuthenticationProperties Properties { get; set; }

        private static string PropertyValueIfExists(string property, IDictionary<string, JToken> dictionary)
        {
            return dictionary.ContainsKey(property) ? dictionary[property].ToString() : null;
        }
    }
}
