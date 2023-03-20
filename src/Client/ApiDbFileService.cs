// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using UkrGuru.Extensions.Data;

namespace UkrGuru.SqlJson.Client;

/// <summary>
/// 
/// </summary>
public class ApiDbFileService : ApiDbService, IDbFileService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="http"></param>
    public ApiDbFileService(HttpClient http) : base(http) { }

}