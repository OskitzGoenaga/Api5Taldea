using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using WebApi.DTOs;
using WebApi.Responses;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErreserbakController : BaseApiController
    {
        private readonly string _connectionString;

        public ErreserbakController(string connectionString)
        {
            _connectionString = connectionString;
        }

        // POST: api/Erreserbak
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ErreserbaCreateDto dto)
        {
            bool arrakasta = false;
            int saiakera = 0;
            int maxSaiakera = 3;

            while (!arrakasta && saiakera < maxSaiakera)
            {
                saiakera++;

                using var con = new MySqlConnection(_connectionString);
                await con.OpenAsync();

                using var transaction = await con.BeginTransactionAsync();

                try
                {
                    string selectSql = "SELECT COUNT(1) FROM erreserba WHERE pista_id = @PistaId AND data = @Data AND ordua = @Ordua FOR UPDATE";
                    using var selectCmd = new MySqlCommand(selectSql, con, transaction);
                    selectCmd.Parameters.AddWithValue("@PistaId", dto.PistaId);
                    selectCmd.Parameters.AddWithValue("@Data", dto.Data);
                    selectCmd.Parameters.AddWithValue("@Ordua", dto.Ordua);

                    long count = Convert.ToInt64(await selectCmd.ExecuteScalarAsync());

                    if (count > 0)
                    {
                        await transaction.RollbackAsync();
                        return Error(400, "BAD_REQUEST", "Pista hori jada erreserbatuta dago ordu eta data horretan.");
                    }

                    string insertSql = "INSERT INTO erreserba (pista_id, bazkidea_id, data, ordua) VALUES (@PistaId, @BazkideId, @Data, @Ordua)";
                    using var insertCmd = new MySqlCommand(insertSql, con, transaction);
                    insertCmd.Parameters.AddWithValue("@PistaId", dto.PistaId);
                    insertCmd.Parameters.AddWithValue("@BazkideId", dto.BazkideaId);
                    insertCmd.Parameters.AddWithValue("@Data", dto.Data);
                    insertCmd.Parameters.AddWithValue("@Ordua", dto.Ordua);

                    await insertCmd.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                    arrakasta = true;

                    return Created(dto, 201, "Erreserba ongi exekutatu da.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    if (saiakera == maxSaiakera)
                    {
                        return Error(500, "INTERNAL_SERVER_ERROR", "Ezin izan da erreserba egin saiakera guztien ondoren.", ex.Message);
                    }

                    await Task.Delay(1000);
                }
            }

            return Error(500, "INTERNAL_SERVER_ERROR", "Errorea gertatu da erreserba prozesatzean.");
        }
    }
}