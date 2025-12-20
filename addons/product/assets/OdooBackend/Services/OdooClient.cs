using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OdooBackend.Models;

namespace OdooBackend.Services;

public class OdooClient
{
    private readonly HttpClient _client;
    private readonly HttpClientHandler _handler;
    private readonly OdooConfig _config;

    public string? SessionId { get; private set; }
    public int? Uid { get; private set; }

    public OdooClient(OdooConfig config)
    {
        _config = config;

        _handler = new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = new CookieContainer()
        };

        _client = new HttpClient(_handler);
    }

    public async Task<bool> AuthenticateAsync()
    {
        var endpoint = $"{_config.Url.TrimEnd('/')}/web/session/authenticate";

        var payload = new
        {
            jsonrpc = "2.0",
            method = "call",
            params_ = new
            {
                db = _config.Db,
                login = _config.Login,
                password = _config.Password
            },
            id = new Random().Next(1, 1_000_000)
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        string json = JsonSerializer.Serialize(payload, options)
                                      .Replace("\"params_\"", "\"params\"");

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(endpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Erreur HTTP : {response.StatusCode}");
            return false;
        }

        var body = await response.Content.ReadAsStringAsync();

        JsonRpcResponse<OdooAuthResult>? authResponse;
        try
        {
            authResponse = JsonSerializer.Deserialize<JsonRpcResponse<OdooAuthResult>>(body);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur de désérialisation JSON : {ex.Message}");
            return false;
        }

        if (authResponse == null)
        {
            Console.WriteLine("Réponse JSON invalide ou vide.");
            return false;
        }

        if (authResponse.Error != null)
        {
            var msg = authResponse.Error.Data?.Message
                      ?? authResponse.Error.Message
                      ?? "Erreur inconnue lors de l'authentification.";
            Console.WriteLine($"Erreur Odoo : {msg}");
            return false;
        }

        if (authResponse.Result == null || authResponse.Result.Uid <= 0)
        {
            Console.WriteLine("Authentification invalide (uid manquant).");
            return false;
        }

        Uid = authResponse.Result.Uid;

        var baseUri = new Uri(_config.Url);
        var cookies = _handler.CookieContainer.GetCookies(baseUri);
        var sessionCookie = cookies["session_id"];

        if (sessionCookie == null)
        {
            Console.WriteLine("Aucun cookie session_id trouvé.");
            SessionId = null;
            return false;
        }

        SessionId = sessionCookie.Value;
        Console.WriteLine($"Authentification OK (uid={Uid}, session_id={SessionId})");
        return true;
    }

    public async Task<JsonRpcResponse<TResult>?> CallKwAsync<TResult>(
        string model,
        string method,
        object[] args,
        object? kwargs = null,
        bool retryOnAuthError = true)
    {
        if (SessionId == null || Uid == null)
        {
            throw new InvalidOperationException("Le client n'est pas authentifié.");
        }

        var endpoint = $"{_config.Url.TrimEnd('/')}/web/dataset/call_kw";

        var payload = new
        {
            jsonrpc = "2.0",
            method = "call",
            params_ = new
            {
                model = model,
                method = method,
                args = args,
                kwargs = kwargs ?? new { },
                context = new
                {
                    uid = Uid,
                    lang = "en_US",
                    tz = "UTC",
                    @params = new { }
                }
            },
            id = new Random().Next(1, 1_000_000)
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        string json = JsonSerializer.Serialize(payload, options)
                                      .Replace("\"params_\"", "\"params\"");

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response;
        try
        {
            response = await _client.PostAsync(endpoint, content);
        }
        catch (Exception ex)
        {
            throw new Exception("Erreur réseau lors de l'appel à Odoo (call_kw).", ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Erreur HTTP Odoo (call_kw) : {response.StatusCode}");
        }

        var body = await response.Content.ReadAsStringAsync();

        JsonRpcResponse<TResult>? rpcResponse;
        try
        {
            rpcResponse = JsonSerializer.Deserialize<JsonRpcResponse<TResult>>(body);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erreur de désérialisation JSON (call_kw) : {ex.Message}", ex);
        }

        if (rpcResponse == null)
        {
            throw new Exception("Réponse JSON-RPC vide ou invalide (call_kw).");
        }

        if (rpcResponse.Error != null)
        {
            var msg = rpcResponse.Error.Data?.Message
                      ?? rpcResponse.Error.Message
                      ?? "Erreur JSON-RPC Odoo.";

            var isSessionExpired =
                msg.Contains("Session expired", StringComparison.OrdinalIgnoreCase) ||
                msg.Contains("Invalid session id", StringComparison.OrdinalIgnoreCase);

            if (isSessionExpired && retryOnAuthError)
            {
                var ok = await AuthenticateAsync();
                if (!ok)
                {
                    throw new Exception("Session expirée et ré-authentification impossible.");
                }

                return await CallKwAsync<TResult>(model, method, args, kwargs, retryOnAuthError: false);
            }

            throw new Exception($"Erreur Odoo (call_kw) : {msg}");
        }

        return rpcResponse;
    }

    public async Task<List<T>?> SearchReadAsync<T>(
        string model,
        object[] domain,
        string[]? fields = null,
        int? limit = null,
        int? offset = null,
        string? order = null)
    {
        var args = new object[]
        {
            domain
        };

        var kwargs = new Dictionary<string, object>();

        if (fields != null && fields.Length > 0)
            kwargs["fields"] = fields;

        if (limit.HasValue)
            kwargs["limit"] = limit.Value;

        if (offset.HasValue)
            kwargs["offset"] = offset.Value;

        if (!string.IsNullOrWhiteSpace(order))
            kwargs["order"] = order;

        var response = await CallKwAsync<List<T>>(
            model,
            "search_read",
            args,
            kwargs.Count > 0 ? kwargs : null
        );

        return response?.Result;
    }
}
