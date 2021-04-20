using System;

namespace Tracker {
    public static class TokenManagement {
        public static Random rand = new();
        
        public static string GenerateToken() {
            string token = string.Empty;
            
            for (int i = 0; i < 60; i++) {
                token += (char)rand.Next(33, 127);
            }

            return token;
        }
    }
}