using System;
using BaseLibrary.Responses;

namespace ClientLibrary.Services.Contracts;

public interface IUserService
{
    Task<List<UserSearchResponse>> SearchAsync(string query);
}  
