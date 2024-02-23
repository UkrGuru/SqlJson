// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;

namespace UkrGuru.SqlJson.Extensions.Tests;

public class GlobalTests
{
    private static readonly Random Random = new(2511);

    private static byte[]? _testBytes1k, _testBytes5k, _testBytes55k;

    public static byte[] TestBytes1k
    {
        get
        {
            if (_testBytes1k == null)
            {
                _testBytes1k = new byte[1024];
                Random.NextBytes(_testBytes1k);
            }

            return _testBytes1k;
        }
    }
    public static string TestString1k => Convert.ToBase64String(TestBytes1k);
    public static char[] TestChars1k => TestString1k.ToCharArray();

    public static byte[] TestBytes5k
    {
        get
        {
            if (_testBytes5k == null)
            {
                _testBytes5k = new byte[1024 * 5];
                Random.NextBytes(_testBytes5k);
            }

            return _testBytes5k;
        }
    }
    public static string TestString5k => Convert.ToBase64String(TestBytes5k);
    public static char[] TestChars5k => TestString5k.ToCharArray();

    public static byte[] TestBytes55k
    {
        get
        {
            if (_testBytes55k == null)
            {
                _testBytes55k = new byte[1024 * 55];
                Random.NextBytes(_testBytes55k);
            }

            return _testBytes55k;
        }
    }
    public static string TestString55k => Convert.ToBase64String(TestBytes55k);
    public static char[] TestChars55k => TestString55k.ToCharArray();

    public class Region
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public enum UserType
    {
        Guest,
        User,
        Manager,
        Admin,
        SysAdmin
    }
}