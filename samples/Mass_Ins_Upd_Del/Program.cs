using System.Reflection;
using UkrGuru.Extensions;
using UkrGuru.SqlJson;

DbHelper.ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=SqlJsonMass;Trusted_Connection=True;";
Assembly.GetExecutingAssembly().ExecResource($"{Assembly.GetExecutingAssembly().GetName().Name}.Resources.InitDb.sql");

int N = 100 * 1024; 
List<Student>? students = new();
DateTime started = DateTime.Now;

// MASS INSERT
for (int i = 0; i < N; i++)
{
    students.Add(new Student() { ID = i, Name = $"Name_{i}" });
}

var sql_insert = @"INSERT Students (ID, Name)
SELECT * FROM OPENJSON(@Data) 
WITH (ID int, Name varchar(50))";

started = DateTime.Now;

await UkrGuru.SqlJson.DbHelper.ExecAsync(sql_insert, students);

Console.WriteLine($"Inserted {N / 1024}K - {DateTime.Now.Subtract(started)}");

// MASS UPDATE
for (int i = 0; i < N; i++)
{
    students[i].Class = (char)((byte)'A' + i % 25);
    students[i].Grade = (byte)(i % 5);
}

var sql_update = @"UPDATE S
SET Class = D.Class, Grade = D.Grade
FROM Students S
INNER JOIN (SELECT * FROM OPENJSON(@Data) 
	WITH (ID int, Class char(1), Grade tinyint)) D ON D.ID = S.ID";

started = DateTime.Now;

await UkrGuru.SqlJson.DbHelper.ExecAsync(sql_update, 
    students.Select(c => new { c.ID, c.Class, c.Grade }));

Console.WriteLine($"Updated {N / 1024}K - {DateTime.Now.Subtract(started)}");

// MASS DELETE
var sql_delete = @"DELETE Students 
WHERE ID IN (SELECT value FROM OPENJSON(@Data))";

started = DateTime.Now;

await UkrGuru.SqlJson.DbHelper.ExecAsync(sql_delete, 
    students.Where(x => x.Grade < 1).Select(c => c.ID ));

Console.WriteLine($"Deleted {(N / 5) / 1024}K - {DateTime.Now.Subtract(started)}");

class Student
{
    public int ID { get; set; }
    public string? Name { get; set; }
    public char? Class { get; set; }
    public byte? Grade { get; set; }
}