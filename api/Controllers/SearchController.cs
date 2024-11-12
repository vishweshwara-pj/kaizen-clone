using Microsoft.AspNetCore.Mvc;
using MyWebApi.Entities;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace MyWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SearchController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("search")]
        public IActionResult SearchKeyword([FromBody] KeywordRequest request)
        {
            if (string.IsNullOrEmpty(request.Keyword))
            {
                return BadRequest("Keyword cannot be empty");
            }

            try
            {
                // Fetch results from SQL Server
                var results = GetKeywordSearchResults(request.Keyword);
                return Ok(results);
            }
            catch (SqlException sqlEx)
            {
                // Return a specific database-related error message
                return StatusCode(500, $"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                // General fallback for other types of errors
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Method to fetch keyword search results from SQL Server
        private List<SearchResult> GetKeywordSearchResults(string keyword)
        {
            var results = new List<SearchResult>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"SELECT Theme_title, kaizen_workarea, type_of_kaizen, before_kaizen, ideal_situation, current_situation
                        FROM KaizenEntries
                        WHERE Theme_title LIKE '%keyword%'
                        OR kaizen_workarea LIKE '%keyword%'
                        OR type_of_kaizen LIKE '%keyword%'
                        OR before_kaizen LIKE '%keyword%'
                        OR ideal_situation LIKE '%keyword%'
                        OR current_situation LIKE '%keyword%';
                        ";

                using (var command = new SqlCommand(query, connection))
                {
                    // Safely add the parameter to avoid SQL Injection
                    command.Parameters.AddWithValue("@keyword", $"%{keyword}%");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Map the results to the SearchResult object
                            results.Add(new SearchResult
                            {
                                ThemeTitle = reader["Theme_title"].ToString(),
                                KaizenWorkArea = reader["kaizen_workarea"].ToString(),
                                TypeOfKaizen = reader["type_of_kaizen"].ToString(),
                                BeforeKaizen = reader["before_kaizen"].ToString(),
                                IdealSituation = reader["ideal_situation"].ToString(),
                                CurrentSituation = reader["current_situation"].ToString()
                            });
                        }
                    }
                }
            }

            return results;
        }
    }
}
