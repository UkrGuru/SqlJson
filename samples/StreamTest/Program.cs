﻿using System.Security.Cryptography;
using UkrGuru.SqlJson;

DbHelper.ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=SqlJsonTest;Trusted_Connection=True";

// https://stuff.mit.edu/afs/sipb/contrib/pi/pi-billion.txt
string srcPath = "pi-billion.txt", sqlPath = "pi-billion-sql.txt";

WriteInfo(srcPath);

using var srcStream = new FileStream(srcPath, FileMode.Open);

Console.WriteLine($"Start Time: {DateTime.Now}");

using (var sqlStream = new FileStream(sqlPath, FileMode.OpenOrCreate))
{
    DbHelper.Exec<Stream>("SELECT @Data", srcStream)?.CopyTo(sqlStream);
}

Console.WriteLine($"Finish Time: {DateTime.Now}");

WriteInfo(sqlPath);

void WriteInfo(string path)
{
    using var stream = new FileStream(path, FileMode.Open);
    using var sha256 = SHA256.Create();
    Console.WriteLine($"\r\n{path} lenght = {stream.Length}.");
    Console.WriteLine($"sha256 = {BitConverter.ToString(sha256.ComputeHash(stream))}.\r\n");
}
