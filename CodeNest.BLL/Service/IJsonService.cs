﻿using CodeNest.DTO.Models;

namespace CodeNest.BLL.Service
{
    public interface IJsonService
    {

        Task<ValidationDto> Validate(string jsonObject);
    }
}