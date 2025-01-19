﻿namespace Shopp.Services.OrderAPI.Models.DTO
{
    public class ResponseDTO
    {
        public object? Result { get; set; }
        public bool IsSuccessful { get; set; } = true;
        public string Message { get; set; } = String.Empty;
    }
}
