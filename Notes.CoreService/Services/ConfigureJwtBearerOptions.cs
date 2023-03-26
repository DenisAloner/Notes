using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Notes.CoreService.DTO;
using Notes.CoreService.Options;
using System.Security.Cryptography;

namespace Notes.CoreService.Services;

public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly KeyCloakOptions _keyCloakOptions;

    public ConfigureJwtBearerOptions(IOptions<KeyCloakOptions> keyCloakOptions)
    {
        _keyCloakOptions = keyCloakOptions.Value;
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(JwtBearerDefaults.AuthenticationScheme, options);
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        var host = _keyCloakOptions.Host;
        var realm = _keyCloakOptions.Realm;

        var httpClient = new HttpClient(new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback =
                (_, _, _, _) => true
        });

        var response = httpClient.GetAsync($"{host}realms/{realm}").Result;

        response.EnsureSuccessStatusCode();

        var realmInformation = response.Content.ReadFromJsonAsync<KeyCloakRealmInformation>().Result;

        var rsa = RSA.Create();

        rsa.ImportSubjectPublicKeyInfo(
            Convert.FromBase64String(realmInformation.PublicKey),
            out _
        );

        var issuerSigningKey = new RsaSecurityKey(rsa);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidIssuers = new[] { $"{host}realms/{realm}" },
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = issuerSigningKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        var unauthorized = ReasonPhrases.GetReasonPhrase(StatusCodes.Status401Unauthorized);
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "text/plain";
                return context.Response.WriteAsync(unauthorized);
            }
        };
    }
}