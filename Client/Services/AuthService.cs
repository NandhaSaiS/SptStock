using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using StockManagementSystem.Shared.Models;

namespace StockManagementSystem.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<LoginResponse> Login(LoginRequest loginRequest)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (loginResponse != null && loginResponse.Success)
        {
            await _localStorage.SetItemAsync("authToken", loginResponse.Token);
            await _localStorage.SetItemAsync("username", loginResponse.Username);
            await _localStorage.SetItemAsync("role", loginResponse.Role);
            
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", loginResponse.Token);

            await ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(loginResponse.Token);
        }

        return loginResponse ?? new LoginResponse { Success = false, Message = "Unknown error" };
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("username");
        await _localStorage.RemoveItemAsync("role");
        
        _httpClient.DefaultRequestHeaders.Authorization = null;

        await ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
    }

    public async Task<string?> GetToken()
    {
        return await _localStorage.GetItemAsync<string>("authToken");
    }

    public async Task<string?> GetRole()
    {
        return await _localStorage.GetItemAsync<string>("role");
    }

    public async Task<string?> GetUsername()
    {
        return await _localStorage.GetItemAsync<string>("username");
    }
}
