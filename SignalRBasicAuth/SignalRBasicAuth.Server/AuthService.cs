﻿using System.Threading.Tasks;

namespace SignalRBasicAuth.Server
{
    public class AuthService
    {
        public  Task<(bool isValid, string userId, string displayName)> ValidateCredentialsAsync(string userName, string password) =>
            Task.FromResult(password switch
            {
                "password" => (true, userName + "@demo.com", userName),
                _ => (false, default, default)
            });
    }
}
