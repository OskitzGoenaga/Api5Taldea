using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PistakController : BaseApiController
    {
        private readonly string _connectionString;

        public PistakController(string connectionString)
        {
            _connectionString = connectionString;
        }

        [HttpGet]
        public async Task<IActionResult> GetPistak()
        {
            try
            {
                using var con = new MySqlConnection(_connectionString);
                await con.OpenAsync();

                string sql = "SELECT id, izena, kirola FROM pista WHERE ezabatuta = 0";
                using var cmd = new MySqlCommand(sql, con);
                using var reader = await cmd.ExecuteReaderAsync();

                var pistas = new List<PistakModel>();
                while (await reader.ReadAsync())
                {
                    pistas.Add(new PistakModel
                    {
                        Id = reader.GetInt32("id"),
                        Izena = reader.GetString("izena"),
                        Kirola = reader.GetString("kirola")
                    });
                }

                return SuccessList(pistas, 200, "Pista aktiboak kargatu dira.");
            }
            catch (Exception ex)
            {
                return Error(500, "INTERNAL_SERVER_ERROR", "Errorea pista aktiboak lortzean", ex.Message);
            }
        }

        [HttpGet("ezabatuak")]
        public async Task<IActionResult> GetPistaEzabatuak()
        {
            try
            {
                using var con = new MySqlConnection(_connectionString);
                await con.OpenAsync();

                string sql = "SELECT id, izena, kirola FROM pista WHERE ezabatuta = 1";
                using var cmd = new MySqlCommand(sql, con);
                using var reader = await cmd.ExecuteReaderAsync();

                var pistak = new List<PistakModel>();
                while (await reader.ReadAsync())
                {
                    pistak.Add(new PistakModel
                    {
                        Id = reader.GetInt32("id"),
                        Izena = reader.GetString("izena"),
                        Kirola = reader.GetString("kirola")
                    });
                }

                return SuccessList(pistak, 200, "Ezabatutako pista guztiak kargatu dira.");
            }
            catch (Exception ex)
            {
                return Error(500, "INTERNAL_SERVER_ERROR", "Errorea ezabatutako pistak lortzean", ex.Message);
            }
        }

        [HttpPut("berreskuratu/{id}")]
        public async Task<IActionResult> PistaBerreskuratu(int id)
        {
            using var con = new MySqlConnection(_connectionString);
            await con.OpenAsync();

            using var transaction = await con.BeginTransactionAsync();

            try
            {
                string checkSql = "SELECT izena FROM pista WHERE id = @Id AND ezabatuta = true FOR UPDATE";
                using var checkCmd = new MySqlCommand(checkSql, con, transaction);
                checkCmd.Parameters.AddWithValue("@Id", id);

                var result = await checkCmd.ExecuteScalarAsync();

                if (result == null)
                {
                    await transaction.RollbackAsync();
                    return NotFoundError($"Ez da aurkitu {id} ID-a duen pistarik papeleran.");
                }

                string pistaIzena = result.ToString() ?? string.Empty;

                string updateSql = "UPDATE pista SET ezabatuta = false WHERE id = @Id";
                using var updateCmd = new MySqlCommand(updateSql, con, transaction);
                updateCmd.Parameters.AddWithValue("@Id", id);

                int rowsAffected = await updateCmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    await transaction.CommitAsync();
                    return Success(true, 200, $"'{pistaIzena}' pista berriro aktibatu da.");
                }

                await transaction.RollbackAsync();
                return Error(400, "BAD_REQUEST", $"Ezin izan da '{pistaIzena}' pista berreskuratu.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Error(500, "INTERNAL_SERVER_ERROR", "Errorea gertatu da pista berreskuratzean", ex.Message);
            }
        }

        [HttpPut("softDelete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            using var con = new MySqlConnection(_connectionString);
            await con.OpenAsync();

            using var transaction = await con.BeginTransactionAsync(); 

            try
            {
                string checkSql = "SELECT izena FROM pista WHERE id = @Id AND ezabatuta = false FOR UPDATE";
                using var checkCmd = new MySqlCommand(checkSql, con, transaction);
                checkCmd.Parameters.AddWithValue("@Id", id);

                var result = await checkCmd.ExecuteScalarAsync();

                if (result == null)
                {
                    await transaction.RollbackAsync();
                    return NotFoundError($"Ez da aurkitu {id} ID-a duen pista aktiborik.");
                }

                string pistaIzena = result.ToString()!;

                string updateSql = "UPDATE pista SET ezabatuta = true WHERE id = @Id";
                using var updateCmd = new MySqlCommand(updateSql, con, transaction);
                updateCmd.Parameters.AddWithValue("@Id", id);

                int rowsAffected = await updateCmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    await transaction.CommitAsync();
                    return Success(true, 200, $"'{pistaIzena}' pista desaktibatu egin da.");
                }

                await transaction.RollbackAsync();
                return Error(400, "BAD_REQUEST", $"Ezin izan da '{pistaIzena}' pista desaktibatu.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Error(500, "INTERNAL_SERVER_ERROR", "Errorea gertatu da soft delete egitean", ex.Message);
            }
        }

        [HttpDelete("hardDelete/{id}")]
        public async Task<IActionResult> DeleteHard(int id)
        {
            using var con = new MySqlConnection(_connectionString);
            await con.OpenAsync();

            using var transaction = await con.BeginTransactionAsync();

            try
            {
                string checkSql = "SELECT izena FROM pista WHERE id = @Id AND ezabatuta = true FOR UPDATE";
                using var checkCmd = new MySqlCommand(checkSql, con, transaction);
                checkCmd.Parameters.AddWithValue("@Id", id);

                var result = await checkCmd.ExecuteScalarAsync();

                if (result == null)
                {
                    await transaction.RollbackAsync();
                    return NotFoundError($"Ez da aurkitu {id} ID-a duen pistarik papeleran.");
                }

                string pistaIzena = result.ToString() ?? string.Empty;

                string deleteSql = "DELETE FROM pista WHERE id = @Id";
                using var deleteCmd = new MySqlCommand(deleteSql, con, transaction);
                deleteCmd.Parameters.AddWithValue("@Id", id);

                int rowsAffected = await deleteCmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    await transaction.CommitAsync();
                    return Success(true, 200, $"'{pistaIzena}' pista betiko ezabatu da.");
                }

                await transaction.RollbackAsync();
                return Error(400, "BAD_REQUEST", $"Ezin izan da '{pistaIzena}' pista ezabatu.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Error(500, "INTERNAL_SERVER_ERROR", "Errorea gertatu da pista ezabatzean", ex.Message);
            }
        }
    }
}
