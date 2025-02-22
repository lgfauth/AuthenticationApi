﻿using Domain.Models;
using Domain.Models.Envelope;

namespace ServicesApplication.Interfaces
{
    public interface IAuthService
    {
        Task<IResponse<AuthResponse>> LoginAsync(LoginRequest request);
        IResponse<bool> ValidateToken(string token);
    }
}