using SampleImplementation.mailworxAPI;

namespace SampleImplementation.Common {
    public class EmsServiceAgent : MailworxWebServiceAgentSoapClient {
        private SecurityContext _securityContext;
        private string _language = "EN";

        /// <summary>
        /// Initializes a new instance of the <see cref="EmsServiceAgent"/> class.
        /// </summary>
        public EmsServiceAgent() : base(EndpointConfiguration.MailworxWebServiceAgentSoap12) {
        }


        /// <summary>
        /// Sets the service url.
        /// </summary>
        /// <param name="serviceUrl">The service url.</param>
        /// <returns>The EmsServiceAgent instance.</returns>
        public EmsServiceAgent UseServiceUrl(string serviceUrl) {
            this.Endpoint.Address = new System.ServiceModel.EndpointAddress(serviceUrl);

            return this;
        }

        /// <summary>
        /// Sets the language.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <returns>The EmsServiceAgent instance.</returns>
        public EmsServiceAgent UseLanguage(string language) {
            _language = language;

            return this;
        }

        /// <summary>
        /// Sets the credentials for your requests.
        /// </summary>
        /// <param name="account">Account name (Mandant) of the eMS to login</param>
        /// <param name="username">User name to use to login</param>
        /// <param name="password">The user's password</param>
        /// <param name="application">The name of the registered application</param>
        /// <returns>The EmsServiceAgent instance.</returns>
        public EmsServiceAgent UseCredentials(string account, string username, string password, string application) {
            _securityContext = new SecurityContext() {
                Account = account,
                Username = username,
                Password = password,
                Source = application
            };

            return this;
        }

        /// <summary>
        /// Creates a request with the given type and sets the security context and the language, as these always need to be sent.
        /// </summary>
        /// <typeparam name="TRequest">The type of the new request.</typeparam>
        /// <param name="request">The request to create.</param>
        /// <returns>The updated request.</returns>
        public TRequest CreateRequest<TRequest>(TRequest request) {
            if (request is SecureRequestMessage) {
                (request as SecureRequestMessage).SecurityContext = _securityContext;
                (request as SecureRequestMessage).Language = _language;
            }

            return request;
        }
    }
}
