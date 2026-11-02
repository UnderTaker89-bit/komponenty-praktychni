using Microsoft.Data.Sqlite;
using System.Globalization;
using PostService.Models;
namespace PostService.DataAccess;
public interface IPostingRepository
{
 List<Posting> GetList(); Posting? GetById(int id); int Create(Posting model); void Update(Posting model); int Delete(int id);
}
public class PostingRepository(string connectionString):IPostingRepository
{
 private static readonly string[] Fields=["From","To","Content","DeliveryType","Weight","Width","Height","Depth","Value","Price","CreatedAt"];
 private SqliteConnection Open(){var c=new SqliteConnection(connectionString);c.Open();return c;}
 public void CreateDb(){using var c=Open();using var cmd=c.CreateCommand();cmd.CommandText="CREATE TABLE IF NOT EXISTS Postings(Id INTEGER PRIMARY KEY AUTOINCREMENT,\"From\" TEXT NOT NULL,\"To\" TEXT NOT NULL,Content TEXT NOT NULL,DeliveryType TEXT NOT NULL,Weight TEXT NOT NULL,Width TEXT NOT NULL,Height TEXT NOT NULL,Depth TEXT NOT NULL,Value TEXT NOT NULL,Price TEXT NOT NULL,CreatedAt TEXT NOT NULL)";cmd.ExecuteNonQuery();}
 private static Posting Read(SqliteDataReader r)=>new(){Id=r.GetInt32(0),From=r.GetString(1),To=r.GetString(2),Content=r.GetString(3),DeliveryType=r.GetString(4),Weight=decimal.Parse(r.GetString(5),CultureInfo.InvariantCulture),Width=decimal.Parse(r.GetString(6),CultureInfo.InvariantCulture),Height=decimal.Parse(r.GetString(7),CultureInfo.InvariantCulture),Depth=decimal.Parse(r.GetString(8),CultureInfo.InvariantCulture),Value=decimal.Parse(r.GetString(9),CultureInfo.InvariantCulture),Price=decimal.Parse(r.GetString(10),CultureInfo.InvariantCulture),CreatedAt=DateTime.Parse(r.GetString(11),CultureInfo.InvariantCulture,DateTimeStyles.RoundtripKind)};
 public List<Posting> GetList(){using var c=Open();using var cmd=c.CreateCommand();cmd.CommandText="SELECT * FROM Postings ORDER BY Id";using var r=cmd.ExecuteReader();var result=new List<Posting>();while(r.Read())result.Add(Read(r));return result;}
 public Posting? GetById(int id){using var c=Open();using var cmd=c.CreateCommand();cmd.CommandText="SELECT * FROM Postings WHERE Id=$id";cmd.Parameters.AddWithValue("$id",id);using var r=cmd.ExecuteReader();return r.Read()?Read(r):null;}
 private static void Bind(SqliteCommand cmd,Posting p){foreach(var name in Fields){var value=typeof(Posting).GetProperty(name)!.GetValue(p)!;cmd.Parameters.AddWithValue("$"+name,value is DateTime date?date.ToString("O",CultureInfo.InvariantCulture):Convert.ToString(value,CultureInfo.InvariantCulture));}}
 public int Create(Posting p){using var c=Open();using var cmd=c.CreateCommand();cmd.CommandText="INSERT INTO Postings("+string.Join(",",Fields.Select(f=>"\""+f+"\""))+") VALUES("+string.Join(",",Fields.Select(f=>"$"+f))+") RETURNING Id";Bind(cmd,p);p.Id=Convert.ToInt32(cmd.ExecuteScalar());return p.Id;}
 public void Update(Posting p){using var c=Open();using var cmd=c.CreateCommand();cmd.CommandText="UPDATE Postings SET "+string.Join(",",Fields.Select(f=>"\""+f+"\"=$"+f))+" WHERE Id=$id";Bind(cmd,p);cmd.Parameters.AddWithValue("$id",p.Id);cmd.ExecuteNonQuery();}
 public int Delete(int id){using var c=Open();using var cmd=c.CreateCommand();cmd.CommandText="DELETE FROM Postings WHERE Id=$id";cmd.Parameters.AddWithValue("$id",id);return cmd.ExecuteNonQuery();}
}
