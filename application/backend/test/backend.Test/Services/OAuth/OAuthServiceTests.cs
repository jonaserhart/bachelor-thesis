using System.Net;
using System.Net.Http.Headers;
using System.Web;
using backend.Model.Config;
using backend.Model.Rest;
using backend.Services.API;
using backend.Services.Database;
using backend.Services.OAuth;
using backend.Services.Users;
using backend.Test.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace backend.Test.Services.OAuth;

[TestFixture]
public class OAuthServiceTests
{

    private Mock<ILogger<OAuthService>>? _loggerMock;
    private Mock<IOptions<OAuthConfig>>? _configMock;
    private Mock<IUserService>? _userServiceMock;
    private Mock<IApiClientFactory>? _apiClientFactoryMock;
    private Mock<IHttpClientFactory>? _httpClientFactoryMock;
    private DataContext? _dbContext;
    private OAuthService? _oauthService;

    private static OAuthConfig DefaultOAuthConfig = new OAuthConfig
    {
        AuthorizationUri = "https://oauth.com/authorize",
        ClientId = "client-id",
        TokenUri = "https://oauth.com/token",
        ClientSecret = "secret",
        RedirectUri = "https://server/oauth-callback",
        Scope = "scope"
    };

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<OAuthService>>();
        _configMock = new Mock<IOptions<OAuthConfig>>();
        _userServiceMock = new Mock<IUserService>();
        _apiClientFactoryMock = new Mock<IApiClientFactory>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _dbContext = TestServices.GetDatabaseContext();

        _configMock.SetupGet(x => x.Value).Returns(DefaultOAuthConfig);

        _oauthService = new OAuthService(
            context: _dbContext,
            logger: _loggerMock.Object,
            oauthConfig: _configMock.Object,
            userService: _userServiceMock.Object,
            apiClientFactory: _apiClientFactoryMock.Object,
            httpClientFactory: _httpClientFactoryMock.Object
        );
    }

    [TearDown]
    public void Teardown()
    {
        if (_dbContext != null)
        {
            _dbContext.Dispose();
        }

        _dbContext = null;
        _loggerMock = null;
        _configMock = null;
        _userServiceMock = null;
        _apiClientFactoryMock = null;
        _httpClientFactoryMock = null;
    }

    [Test]
    public void GetNewAuthorizeUrl_WITH_GivenConfig_RETURNS_ValidUrl()
    {
        var state = Guid.NewGuid();

        var url = _oauthService!.GetNewAuthorizeUrl();

        var uri = new Uri(url);
        var queryParams = HttpUtility.ParseQueryString(uri.Query ?? string.Empty);

        Assert.Multiple(() =>
        {
            Assert.That(queryParams["client_id"], Is.EqualTo(DefaultOAuthConfig.ClientId));
            Assert.That(queryParams["response_type"], Is.EqualTo("Assertion"));
            Assert.That(Guid.TryParse(queryParams["state"], out _), Is.True);
            Assert.That(queryParams["scope"], Is.EqualTo(DefaultOAuthConfig.Scope));
            Assert.That(queryParams["redirect_uri"], Is.EqualTo(DefaultOAuthConfig.RedirectUri));
        });
    }

    [Test]
    public async Task HandleOAuthCallbackAsync_ValidParameters_ReturnsAuthenticationResponse()
    {
        // Arrange
        var url = _oauthService!.GetNewAuthorizeUrl();

        var uri = new Uri(url);
        var queryParams = HttpUtility.ParseQueryString(uri.Query ?? string.Empty);

        if (!Guid.TryParse(queryParams["state"], out var state))
        {
            Assert.Fail("Could not add new state to _oauthService");
        }

        var code = "response_code";
        var submission = new OAuthCallbackModel
        {
            Code = code,
            State = state
        };

        var tokenRequestUrl = DefaultOAuthConfig.TokenUri;
        var expectedResponse = new AuthenticationResponse();

        var tokenRequestMessage = new HttpRequestMessage(HttpMethod.Post, tokenRequestUrl);
        tokenRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
            { "client_assertion", DefaultOAuthConfig.ClientSecret },
            { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer" },
            { "assertion", code },
            { "redirect_uri", DefaultOAuthConfig.RedirectUri }
        });

        tokenRequestMessage.Content = formContent;

        var successStatusCode = HttpStatusCode.OK;
        var successResponseContent = @"{
            ""access_token"": ""your-access-token"",
            ""expires_in"": 3600,
            ""token_type"": ""Bearer""
        }";

        var httpResponseMessage = new HttpResponseMessage(successStatusCode)
        {
            Content = new StringContent(successResponseContent)
        };

        var matchesExpectedHttpRequestMessage = (HttpRequestMessage req) =>
                      req.Method == HttpMethod.Post &&
                      req.RequestUri?.AbsoluteUri == tokenRequestUrl &&
                      req.Headers.Accept.Contains(new MediaTypeWithQualityHeaderValue("application/json")) &&
                      req.Content?.ReadAsStringAsync().Result == formContent.ReadAsStringAsync().Result;

        var httpClientMock = new Mock<HttpClient>();
        httpClientMock
            .Setup<Task<HttpResponseMessage>>(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(httpResponseMessage)
            .Verifiable();

        _httpClientFactoryMock!
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClientMock.Object);

        // Act
        var response = await _oauthService!.HandleOAuthCallbackAsync(submission);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.User, Is.EqualTo(expectedResponse.User));
            Assert.That(response.Token, Is.EqualTo(expectedResponse.Token));

            Assert.That(() =>
            {
                httpClientMock.Verify();
            }, Throws.Nothing, "Verifying SendAsync failed!");
        });

    }

}