using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PistakController : ControllerBase
    {
        private readonly string _connectionString;

        public PistakController(string connectionString)
        {
            _connectionString = connectionString;
        }

        // GET: api/<PistakController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // DELETE api/<PistakController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpDelete("/hardDelete/{id}")]
        public bool DeleteHard(int id)
        {
            try
            {
                using var con = new MySqlConnection(_connectionString);
                con.Open();

                string checkSql = "SELECT COUNT(1) FROM pista WHERE id = @id and ezabatuta = true";
                using var checkCmd = new MySqlCommand(checkSql, con);
                checkCmd.Parameters.AddWithValue("@Id", id);
                
                long count = Convert.ToInt64(checkCmd.ExecuteScalar());

                if (count == 0)
                {
                    Console.WriteLine($"Ez da aurkitu erreserbarik");
                    return false;
                }

                string deleteSql = "DELETE FROM pistak WHERE id = @Id";
                using var deleteCmd = new MySqlCommand(deleteSql, con);
                deleteCmd.Parameters.AddWithValue("@Id", id);

                int rowsAffected = deleteCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"\nPista ondo ezabatu da");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nErrorea erreserba ezabatzean: {ex.Message}");
                return false;
            }
        }
    }
}
