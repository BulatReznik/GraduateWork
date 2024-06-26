﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace YandexTrackerApi.AppLogic.Core
{
    public static class JwtBearerConfiguration
    {
        public static AuthenticationBuilder AddJwtBearerConfiguration(
            this AuthenticationBuilder builder,
            string issuer,
            string audience,
            string jwtAccessKey)
        {
            return builder.AddJwtBearer(options =>
            {
                options.Authority = issuer;
                options.Audience = audience;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Строка, представляющая издателя
                    ValidIssuer = issuer,

                    // Установка потребителя токена
                    ValidAudience = audience,

                    // Установка ключа безопасности
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtAccessKey)),

                    // Указывает, будет ли валидироваться издатель при валидации токена
                    ValidateIssuer = true,

                    // Будет ли валидироваться потребитель токена
                    ValidateAudience = true,

                    // Будет ли валидироваться время существования
                    ValidateLifetime = true,

                    // Валидация ключа безопасности
                    ValidateIssuerSigningKey = true,
                };

                options.RequireHttpsMetadata = false;
                options.Configuration = new OpenIdConnectConfiguration();
            });
        }
    }
}
