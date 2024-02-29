// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using System.Text.Json;
using static UkrGuru.SqlJson.Client.Tests.GlobalTests;

namespace UkrGuru.SqlJson.Extensions.Tests;

public class ObjExtensionsTests
{
    [Fact]
    public void CanToObj_Null()
    {
        Assert.Null((null as object).ToObj<int?>());
        Assert.Null(DBNull.Value.ToObj<int?>());
        Assert.Null(string.Empty.ToObj<int?>());
        Assert.Null(new StringBuilder().ToObj<bool?>());
        Assert.Null(Array.Empty<byte[]>().ToObj<byte[]?>());
        Assert.Null(Array.Empty<char[]>().ToObj<char[]?>());
    }

    [Fact]
    public void CanToObj_Default()
    {
        var bytes_default = new byte[1] { 0x01 };
        var chars_default = new char[1] { 'A' };

        Assert.True((null as string).ToObj(true));
        Assert.True(DBNull.Value.ToObj(true));
        Assert.True(string.Empty.ToObj(true));
        Assert.True(new StringBuilder().ToObj(true));
        Assert.Equal(bytes_default, Array.Empty<byte[]>().ToObj(bytes_default));
        Assert.Equal(chars_default, Array.Empty<char[]>().ToObj(chars_default));
        Assert.True((JsonDocument.Parse("null").RootElement).ToObj(true));
    }

    [Fact]
    public void CanToObj_Boolean()
    {
        bool value = false;

        Assert.Equal(value, value.ToObj<bool>());
        Assert.Equal(value, value.ToString().ToObj<bool>());
        Assert.Equal(value, 0.ToObj<bool>());

        value = true;
        Assert.Equal(value, value.ToObj<bool>());
        Assert.Equal(value, value.ToString().ToObj<bool>());
        Assert.Equal(value, 1.ToObj<bool>());
    }

    [Fact]
    public void CanToObj_Byte()
    {
        byte value = 0x0a;

        Assert.Equal(value, value.ToObj<byte>());
        Assert.Equal(value, value.ToString().ToObj<byte>());
    }

    [Fact]
    public void CanToObj_ByteArray()
    {
        byte[] value = Encoding.UTF8.GetBytes("\n\r");

        Assert.Equal(value, value.ToObj<byte[]>());
        Assert.Equal(value, "\n\r".ToObj<byte[]>());
    }

    [Fact]
    public void CanToObj_Char()
    {
        char value = 'X';
        string string_value = "X";

        Assert.Equal(value, value.ToObj<char>());
        Assert.Equal(string_value, string_value.ToObj<string>());
    }

    [Fact]
    public void CanToObj_CharArray()
    {
        char[] value = new char[] { 'A', 'X' };
        string string_value = "AX";

        Assert.Equal(value, value.ToObj<char[]?>());
        Assert.Equal(string_value, string_value.ToObj<string?>());
    }

    [Fact]
    public void CanToObj_DateOnly()
    {
        DateOnly value = new (2000, 11, 25);
        DateTime dt = new(2000, 11, 25);

        Assert.Equal(value, value.ToObj<DateOnly>());
        Assert.Equal(value, dt.ToLongDateString().ToObj<DateOnly>());
        Assert.Equal(value, dt.ToString("yyyy-MM-dd").ToObj<DateOnly>());
        Assert.Equal(value, dt.ToString("yyyy-MM-ddTHH:mm:ss").ToObj<DateOnly>());
        Assert.Equal(value, dt.ToString("yyyy-MM-ddTHH:mm:ssZ").ToObj<DateOnly>());
        Assert.Equal(value, dt.ToString("yyyy-MM-dd HH:mm:ss.fffffff").ToObj<DateOnly>());
        Assert.Equal(value, dt.ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz").ToObj<DateOnly>());
    }

    [Fact]
    public void CanToObj_TimeOnly()
    {
        TimeOnly value = new(23, 59, 59);
        DateTime dt = new(2000, 1, 1, 23, 59, 59);

        Assert.Equal(value, value.ToObj<TimeOnly>());
        Assert.Equal(value, dt.ToLongTimeString().ToObj<TimeOnly>());
    }

    [Fact]
    public void CanToObj_DateTime()
    {
        DateTime value = new(2000, 11, 25, 23, 59, 59);

        Assert.Equal(value, value.ToObj<DateTime>());
        Assert.Equal(value, value.ToString().ToObj<DateTime>());
        Assert.Equal(value, value.ToString("yyyy-MM-ddTHH:mm:ss").ToObj<DateTime>());

        //value = TimeZoneInfo.ConvertTimeFromUtc(value, TimeZoneInfo.Local);
        //Assert.Equal(value, value.ToString("yyyy-MM-ddTHH:mm:ssZ").ToObj<DateTime>());
    }

    [Fact]
    public void CanToObj_Decimal()
    {
        decimal value = decimal.MinValue;
        Assert.Equal(value, value.ToObj<decimal>());
        Assert.Equal(value, value.ToString().ToObj<decimal>());

        value = decimal.Zero;
        Assert.Equal(value, value.ToObj<decimal>());
        Assert.Equal(value, value.ToString().ToObj<decimal>());

        value = decimal.One;
        Assert.Equal(value, value.ToObj<decimal>());
        Assert.Equal(value, value.ToString().ToObj<decimal>());

        value = decimal.MinusOne;
        Assert.Equal(value, value.ToObj<decimal>());
        Assert.Equal(value, value.ToString().ToObj<decimal>());

        value = decimal.MaxValue;
        Assert.Equal(value, value.ToObj<decimal>());
        Assert.Equal(value, value.ToString().ToObj<decimal>());

        value = 123456.789m;
        Assert.Equal(value, value.ToObj<decimal>());
        Assert.Equal(value, value.ToString().ToObj<decimal>());
    }

    [Fact]
    public void CanToObj_Double()
    {
        double value = double.MinValue;
        Assert.Equal(value, value.ToObj<double>());
        Assert.Equal(value, value.ToString().ToObj<double>());

        value = double.MaxValue;
        Assert.Equal(value, value.ToObj<double>());
        Assert.Equal(value, value.ToString().ToObj<double>());

        value = 123456.789d;
        Assert.Equal(value, value.ToObj<double>());
        Assert.Equal(value, value.ToString().ToObj<double>());
    }

    [Fact]
    public void CanToObj_Enum()
    {
        UserType value = UserType.Guest;

        Assert.Equal(value, value.ToObj<UserType>());
        Assert.Equal(value, ((int)value).ToObj<UserType>());
        Assert.Equal(value, value.ToString().ToObj<UserType>());
    }

    [Fact]
    public void CanToObj_Guid()
    {
        Guid value = Guid.NewGuid();

        Assert.Equal(value, value.ToObj<Guid>());
        Assert.Equal(value, value.ToString().ToObj<Guid>());
    }

    [Fact]
    public void CanToObj_Int16()
    {
        short value = short.MinValue;

        Assert.Equal(value, value.ToObj<short>());
        Assert.Equal(value, value.ToString().ToObj<short>());

        value = short.MaxValue;
        Assert.Equal(value, value.ToObj<short>());
        Assert.Equal(value, value.ToString().ToObj<short>());

        value = 0;
        Assert.Equal(value, value.ToObj<short>());
        Assert.Equal(value, value.ToString().ToObj<short>());
    }

    [Fact]
    public void CanToObj_Int32()
    {
        int value = int.MinValue;

        Assert.Equal(value, value.ToObj<int>());
        Assert.Equal(value, value.ToString().ToObj<int>());

        value = int.MaxValue;
        Assert.Equal(value, value.ToObj<int>());
        Assert.Equal(value, value.ToString().ToObj<int>());

        value = 0;
        Assert.Equal(value, value.ToObj<int>());
        Assert.Equal(value, value.ToString().ToObj<int>());
    }

    [Fact]
    public void CanToObj_Int64()
    {
        long value = long.MinValue;

        Assert.Equal(value, value.ToObj<long>());
        Assert.Equal(value, value.ToString().ToObj<long>());

        value = long.MaxValue;
        Assert.Equal(value, value.ToObj<long>());
        Assert.Equal(value, value.ToString().ToObj<long>());

        value = 0;
        Assert.Equal(value, value.ToObj<long>());
        Assert.Equal(value, value.ToString().ToObj<long>());
    }

    [Fact]
    public void CanToObj_StringBuilder()
    {
        var value = new StringBuilder();
        value.Append('t');
        value.Append('r');
        value.Append('u');
        value.Append('e');

        Assert.True(value.ToObj(false));
    }

    [Fact]
    public void CanToObj_JsonElement()
    {
        var json = "null";  var value = JsonDocument.Parse(json).RootElement;
        Assert.Null(value.ToObj<bool?>());

        bool bool_value = false;
        json = JsonSerializer.Serialize(bool_value); value = JsonDocument.Parse(json).RootElement;
        Assert.False(value.ToObj<bool>());
        Assert.Equal(bool.FalseString.ToLower(), value.ToObj<string>());

        bool_value = true;
        json = JsonSerializer.Serialize(bool_value); value = JsonDocument.Parse(json).RootElement;
        Assert.True(value.ToObj<bool>());
        Assert.Equal(bool.TrueString.ToLower(), value.ToObj<string>());

        byte byte_value = 0x0a;
        json = JsonSerializer.Serialize(byte_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(byte_value, value.ToObj<byte>());
        Assert.Equal(byte_value.ToString(), value.ToObj<string>());

        byte[] bytearr_value = Array.Empty<byte>();
        json = JsonSerializer.Serialize(bytearr_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(bytearr_value, value.ToObj<byte[]>());
        Assert.Equal(bytearr_value, Convert.FromBase64String(value.ToObj<string>()!));

        bytearr_value = Encoding.UTF8.GetBytes("\n\r");
        json = JsonSerializer.Serialize(bytearr_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(bytearr_value, value.ToObj<byte[]>());
        Assert.Equal(bytearr_value, Convert.FromBase64String(value.ToObj<string>()!));

        char char_value = 'A';
        json = JsonSerializer.Serialize(char_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(char_value, value.ToObj<char>());
        Assert.Equal(char_value.ToString(), value.ToObj<string>());
        Assert.Equal(char_value.ToString(), value.ToObj<string>());

        char[] chararr_value = Array.Empty<char>();
        json = JsonSerializer.Serialize(chararr_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(chararr_value, value.ToObj<char[]>());
        Assert.Equal("[]", value.ToObj<string>());

        chararr_value = [ 'A', 'V' ];
        json = JsonSerializer.Serialize(chararr_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(chararr_value, value.ToObj<char[]>());
        Assert.Equal(@"[""A"",""V""]", value.ToObj<string>());

        DateOnly date_value = new(2000, 11, 25);
        json = JsonSerializer.Serialize(date_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(date_value, value.ToObj<DateOnly>());
        Assert.Equal(date_value.ToString("yyyy-MM-dd"), value.ToObj<string>());

        TimeOnly time_value = new(23, 59, 59);
        json = JsonSerializer.Serialize(time_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(time_value, value.ToObj<TimeOnly>());
        Assert.Equal(time_value.ToString("HH:mm:ss"), value.ToObj<string>());

        DateTime datetime_value = new(2000, 11, 25, 23, 59, 59);
        json = JsonSerializer.Serialize(datetime_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(datetime_value, value.ToObj<DateTime>());
        Assert.Equal(datetime_value.ToString("yyyy-MM-ddTHH:mm:ss"), value.ToObj<string>());

        decimal decimal_value = decimal.MinValue;
        json = JsonSerializer.Serialize(decimal_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(decimal_value, value.ToObj<decimal>());
        Assert.Equal(decimal_value.ToString(), value.ToObj<string>());

        decimal_value = decimal.Zero;
        json = JsonSerializer.Serialize(decimal_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(decimal_value, value.ToObj<decimal>());
        Assert.Equal(decimal_value.ToString(), value.ToObj<string>());

        decimal_value = decimal.One;
        json = JsonSerializer.Serialize(decimal_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(decimal_value, value.ToObj<decimal>());
        Assert.Equal(decimal_value.ToString(), value.ToObj<string>());

        decimal_value = decimal.MinusOne;
        json = JsonSerializer.Serialize(decimal_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(decimal_value, value.ToObj<decimal>());
        Assert.Equal(decimal_value.ToString(), value.ToObj<string>());

        decimal_value = decimal.MaxValue;
        json = JsonSerializer.Serialize(decimal_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(decimal_value, value.ToObj<decimal>());
        Assert.Equal(decimal_value.ToString(), value.ToObj<string>());

        decimal_value = 123456.789m;
        json = JsonSerializer.Serialize(decimal_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(decimal_value, value.ToObj<decimal>());
        Assert.Equal(decimal_value.ToString(), value.ToObj<string>());

        double double_value = double.MinValue;
        json = JsonSerializer.Serialize(double_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(double_value, value.ToObj<double>());
        Assert.Equal(double_value.ToString(), value.ToObj<string>());

        double_value = double.MaxValue;
        json = JsonSerializer.Serialize(double_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(double_value, value.ToObj<double>());
        Assert.Equal(double_value.ToString(), value.ToObj<string>());

        double_value = 123456.789d;
        json = JsonSerializer.Serialize(double_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(double_value, value.ToObj<double>());
        Assert.Equal(double_value.ToString(), value.ToObj<string>());

        UserType enum_value = UserType.Guest;
        json = JsonSerializer.Serialize(enum_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(enum_value, value.ToObj<UserType>());
        Assert.Equal(((int)enum_value).ToString(), value.ToObj<string>());

        Guid guid_value = Guid.NewGuid();
        json = JsonSerializer.Serialize(guid_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(guid_value, value.ToObj<Guid>());
        Assert.Equal(guid_value.ToString(), value.ToObj<string>());

        short short_value = short.MinValue;
        json = JsonSerializer.Serialize(short_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(short_value, value.ToObj<short>());
        Assert.Equal(short_value.ToString(), value.ToObj<string>());

        short_value = short.MaxValue;
        json = JsonSerializer.Serialize(short_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(short_value, value.ToObj<short>());
        Assert.Equal(short_value.ToString(), value.ToObj<string>());

        short_value = 0;
        json = JsonSerializer.Serialize(short_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(short_value, value.ToObj<short>());
        Assert.Equal(short_value.ToString(), value.ToObj<string>());

        int int_value = int.MinValue;
        json = JsonSerializer.Serialize(int_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(int_value, value.ToObj<int>());
        Assert.Equal(int_value.ToString(), value.ToObj<string>());

        int_value = int.MaxValue;
        json = JsonSerializer.Serialize(int_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(int_value, value.ToObj<int>());
        Assert.Equal(int_value.ToString(), value.ToObj<string>());

        int_value = 0;
        json = JsonSerializer.Serialize(int_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(int_value, value.ToObj<int>());
        Assert.Equal(int_value.ToString(), value.ToObj<string>());

        long long_value = long.MinValue;
        json = JsonSerializer.Serialize(long_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(long_value, value.ToObj<long>());
        Assert.Equal(long_value.ToString(), value.ToObj<string>());

        long_value = long.MaxValue;
        json = JsonSerializer.Serialize(long_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(long_value, value.ToObj<long>());
        Assert.Equal(long_value.ToString(), value.ToObj<string>());

        long_value = 0;
        json = JsonSerializer.Serialize(long_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(long_value, value.ToObj<long>());
        Assert.Equal(long_value.ToString(), value.ToObj<string>());

        string string_value = string.Empty;
        json = JsonSerializer.Serialize(string_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(string_value, value.ToObj<string>());
        Assert.Equal(string_value, value.ToObj<string>());

        string_value = "ASD";
        json = JsonSerializer.Serialize(string_value); value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(string_value, value.ToObj<string>());
        Assert.Equal(string_value, value.ToObj<string>());
    }

    [Fact]
    public static void CanToObj_Class()
    {
        var value = new Region() { Id = 1, Name = "West" };

        var actual = JsonSerializer.Serialize(value).ToObj<Region?>();

        Assert.Equal(value.Id, actual?.Id);
        Assert.Equal(value.Name, actual?.Name);

        //var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value)));
        //actual = stream.ToObj<Region?>();

        //Assert.Equal(value.Id, actual?.Id);
        //Assert.Equal(value.Name, actual?.Name);
    }
}