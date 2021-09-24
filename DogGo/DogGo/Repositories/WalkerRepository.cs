using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class WalkerRepository : IWalkerRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public WalkerRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Walker> GetAllWalkers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], ImageUrl, NeighborhoodId
                        FROM Walker
                    ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walker> walkers = new List<Walker>();
                    while (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                        };

                        walkers.Add(walker);
                    }

                    reader.Close();

                    return walkers;
                }
            }
        }

        public Walker GetWalkerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        SELECT w.Id AS 'WalkerId', w.Name AS 'WalkerName', w.ImageUrl as 'Walker Avatar', 
                                        n.Id AS 'Neighborhood Id', n.Name AS 'Neighborhood Name', wa.Id AS 'Walks Id', Date, Duration, d.Id AS 'Dog Id', 
                                        d.Name AS 'Dog Name', o.Id AS 'Owner Id', o.Name AS 'Owner Name'
                                        FROM Walker w
                                        LEFT JOIN Neighborhood n ON w.NeighborhoodId = n.Id
                                        LEFT JOIN Walks wa ON wa.WalkerId = w.Id
                                        LEFT JOIN Dog d ON wa.DogId = d.Id
                                        LEFT JOIN Owner o ON o.Id = d.OwnerId                        
                                        WHERE w.Id = @id
                                        ORDER BY o.Name
                                    ";

                    cmd.Parameters.AddWithValue("@id", id);

                    Walker walker = null;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (walker == null)
                            {
                                walker = new Walker
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                                    Name = reader.GetString(reader.GetOrdinal("WalkerName")),
                                    ImageUrl = reader.GetString(reader.GetOrdinal("Walker Avatar")),
                                    Neighborhood = new Neighborhood
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("Neighborhood Id")),
                                        Name = reader.GetString(reader.GetOrdinal("Neighborhood Name"))
                                    },
                                    Walks = new List<Walk>()
                                };
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("Walks Id")))
                            {
                                walker.Walks.Add(new Walk
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Walks Id")),
                                    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                    Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                                    Dog = new Dog
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("Dog Id")),
                                        Name = reader.GetString(reader.GetOrdinal("Dog Name")),
                                        Owner = new Owner
                                        {
                                            Id = reader.GetInt32(reader.GetOrdinal("Owner Id")),
                                            Name = reader.GetString(reader.GetOrdinal("Owner Name"))
                                        }
                                    }
                                });
                            }
                        }
                        return walker;
                    }
                    
                }                                       
            }
        }
        

        public List<Walker> GetWalkersInNeighborhood(int neighborhoodId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT Id, [Name], ImageUrl, NeighborhoodId
                FROM Walker
                WHERE NeighborhoodId = @neighborhoodId
            ";

                    cmd.Parameters.AddWithValue("@neighborhoodId", neighborhoodId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walker> walkers = new List<Walker>();
                    while (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                        };

                        walkers.Add(walker);
                    }

                    reader.Close();

                    return walkers;
                }
            }
        }

    }
}
