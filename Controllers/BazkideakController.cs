using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BazkideakController : BaseApiController
    {
        private readonly string _connectionString;

        public BazkideakController(string connectionString)
        {
            _connectionString = connectionString;
        }

        [HttpGet]
        public async Task<IActionResult> GetBazkideak()
        {
            try
            {
                using var con = new MySqlConnection(_connectionString);
                await con.OpenAsync();

                string sql = "SELECT id, CONCAT(izena, ' ', abizena) AS izena_osoa FROM bazkidea";
                using var cmd = new MySqlCommand(sql, con);
                using var reader = await cmd.ExecuteReaderAsync();

                var bazkideak = new List<BazkideakModel>();
                while (await reader.ReadAsync())
                {
                    bazkideak.Add(new BazkideakModel
                    {
                        Id = reader.GetInt32("id"),
                        IzenaOsoa = reader.GetString("izena_osoa")
                    });
                }

                return SuccessList(bazkideak, 200, "Bazkide guztiak kargatu dira.");
            }
            catch (Exception ex)
            {
                return Error(500, "INTERNAL_SERVER_ERROR", "Errorea bazkideak lortzean", ex.Message);
            }
        }
    }
}