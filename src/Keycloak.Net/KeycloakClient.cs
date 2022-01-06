using System;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Keycloak.Net.Common.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Keycloak.Net
{
    public partial class KeycloakClient
    {
        private ISerializer _serializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
        });

        private readonly Url _url;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _clientSecret;
        private readonly string _client_id;

        private readonly Func<string> _getToken;

        private KeycloakClient(string url)
        {
            _url = new Url(url);
        }

        private KeycloakClient(Url url)
        {
            _url = url;
        }

            [ObsoleteAttribute]
        public KeycloakClient(string url, string userName, string password)
            : this(url)
        {
            _userName = userName;
            _password = password;
        }

        public KeycloakClient(Url url, string clientSecret, string client_id)
            : this(url)
        {
            _clientSecret = clientSecret;
            _client_id = client_id;
        }

        public KeycloakClient(string url, Func<string> getToken)
            : this(url)
        {
            _getToken = getToken;
        }

        public void SetSerializer(ISerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        private IFlurlRequest GetBaseUrl(string authenticationRealm) => new Url(_url)
            .AppendPathSegment("/auth")
            .ConfigureRequest(settings => settings.JsonSerializer = _serializer)
            .WithAuthentication(_getToken, _url, authenticationRealm, _userName, _password, _clientSecret, _client_id);
    }
}
