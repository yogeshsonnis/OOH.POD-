using Microsoft.Data.Sqlite;
using OOH.POD.DataModels;
using System.Text.Json;

namespace OOH.POD.DAL
{
    public class CompartmentRepository
    {
        private readonly string _connectionString = "Data Source=newcompartments.db";
        public void EnsureTablesCreated()
        {
            var jsonList = new[]
            {
                "{\"compartmentName\":\"1\",\"compartmentSize\":\"L\",\"compartmentType\":null,\"isClosed\":true,\"isLocked\":true,\"isOutOfService\":false,\"shipments\":[{\"trackingNumber\":\"JEREMYTT222\",\"timeStored\":\"2025-08-14T13:00:49\",\"timeOverdue\":\"2025-08-16T13:00:49\",\"timeStayed\":\"1h 7min\"}]}",
                "{\"compartmentName\":\"2\",\"compartmentSize\":\"S\",\"compartmentType\":null,\"isClosed\":true,\"isLocked\":true,\"isOutOfService\":false,\"shipments\":[{\"trackingNumber\":\"JBB2\",\"timeStored\":\"2025-08-13T14:42:17\",\"timeOverdue\":\"2025-08-15T14:42:17\",\"timeStayed\":\"23h 25min\"}]}",
                "{\"compartmentName\":\"5\",\"compartmentSize\":\"XXS\",\"compartmentType\":null,\"isClosed\":true,\"isLocked\":true,\"isOutOfService\":false,\"shipments\":[{\"trackingNumber\":\"H041JA0000027134\",\"timeStored\":\"2025-08-14T10:35:45\",\"timeOverdue\":\"2025-08-16T10:35:45\",\"timeStayed\":\"3h 32min\"}]}"
            };

            foreach (var json in jsonList)
            {
                var compartment = JsonSerializer.Deserialize<Compartment>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                SaveCompartmentToDatabase(compartment);
            }

        }

        public void DeleteCompartment(long compartmentId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Delete shipments for the compartment
            using (var deleteShipmentsCmd = connection.CreateCommand())
            {
                deleteShipmentsCmd.CommandText = "DELETE FROM Shipment WHERE CompartmentId = $compartmentId";
                deleteShipmentsCmd.Parameters.AddWithValue("$compartmentId", compartmentId);
                deleteShipmentsCmd.ExecuteNonQuery();
            }

            // Delete the compartment itself
            using (var deleteCompartmentCmd = connection.CreateCommand())
            {
                deleteCompartmentCmd.CommandText = "DELETE FROM Compartment WHERE Id = $id";
                deleteCompartmentCmd.Parameters.AddWithValue("$id", compartmentId);
                deleteCompartmentCmd.ExecuteNonQuery();
            }
        }

        public List<Compartment> GetCompartmentsWithShipments()
        {
            try
            {
                var compartments = new List<Compartment>();
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();
                // Get all compartments
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT * FROM Compartment";
                using var reader = cmd.ExecuteReader();
                var compartmentIds = new List<long>();

                while (reader.Read())
                {
                    var compartment = new Compartment
                    {
                        Id = reader.GetInt64(reader.GetOrdinal("Id")),
                        CompartmentName = reader["CompartmentName"]?.ToString(),
                        CompartmentSize = reader["CompartmentSize"]?.ToString(),
                        CompartmentType = reader["CompartmentType"] as string,
                        IsClosed = reader.GetInt64(reader.GetOrdinal("IsClosed")) == 1,
                        IsLocked = reader.GetInt64(reader.GetOrdinal("IsLocked")) == 1,
                        IsOutOfService = reader.GetInt64(reader.GetOrdinal("IsOutOfService")) == 1,
                        Shipments = new System.Collections.ObjectModel.ObservableCollection<Shipment>()
                    };
                    compartmentIds.Add(compartment.Id);
                    compartments.Add(compartment);
                }
                reader.Close();

                // Get all shipments for all compartments
                if (compartmentIds.Count > 0)
                {
                    var shipmentCmd = connection.CreateCommand();
                    shipmentCmd.CommandText = $"SELECT * FROM Shipment WHERE CompartmentId IN ({string.Join(",", compartmentIds)})";
                    using var shipmentReader = shipmentCmd.ExecuteReader();
                    while (shipmentReader.Read())
                    {
                        var shipment = new Shipment
                        {
                            Id = shipmentReader.GetInt64(shipmentReader.GetOrdinal("Id")),
                            TrackingNumber = shipmentReader["TrackingNumber"]?.ToString(),
                            TimeStored = System.DateTime.Parse(shipmentReader["TimeStored"]?.ToString() ?? System.DateTime.MinValue.ToString()),
                            TimeOverdue = System.DateTime.Parse(shipmentReader["TimeOverdue"]?.ToString() ?? System.DateTime.MinValue.ToString()),
                            TimeStayed = shipmentReader["TimeStayed"]?.ToString()
                        };
                        long compartmentId = shipmentReader.GetInt64(shipmentReader.GetOrdinal("CompartmentId"));
                        var compartment = compartments.Find(c => c.Id == compartmentId);
                        compartment?.Shipments.Add(shipment);
                    }
                }




                return compartments;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public void UpdateCompartment(Compartment compartment)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
        UPDATE Compartment SET
            CompartmentName = $name,
            CompartmentSize = $size,
            CompartmentType = $type,
            IsClosed = $closed,
            IsLocked = $locked,
            IsOutOfService = $outOfService
        WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$name", compartment.CompartmentName);
            cmd.Parameters.AddWithValue("$size", compartment.CompartmentSize);
            cmd.Parameters.AddWithValue("$type", (object)compartment.CompartmentType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$closed", compartment.IsClosed ? 1 : 0);
            cmd.Parameters.AddWithValue("$locked", compartment.IsLocked ? 1 : 0);
            cmd.Parameters.AddWithValue("$outOfService", compartment.IsOutOfService ? 1 : 0);
            cmd.Parameters.AddWithValue("$id", compartment.Id);
            cmd.ExecuteNonQuery();
        }

        private void SaveCompartmentToDatabase(Compartment compartment)
        {
            string connectionString = "Data Source=newcompartments.db";
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            // Create tables if not exist
            var createCompartmentTable = @"
                CREATE TABLE IF NOT EXISTS Compartment (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CompartmentName TEXT,
                    CompartmentSize TEXT,
                    CompartmentType TEXT,
                    IsClosed INTEGER,
                    IsLocked INTEGER,
                    IsOutOfService INTEGER
                );";
            var createShipmentTable = @"
                CREATE TABLE IF NOT EXISTS Shipment (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CompartmentId INTEGER,
                    TrackingNumber TEXT,
                    TimeStored TEXT,
                    TimeOverdue TEXT,
                    TimeStayed TEXT,
                    FOREIGN KEY (CompartmentId) REFERENCES Compartment(Id)
                );";
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = createCompartmentTable;
                cmd.ExecuteNonQuery();
                cmd.CommandText = createShipmentTable;
                cmd.ExecuteNonQuery();
            }

            // Insert Compartment
            long compartmentId;
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO Compartment (CompartmentName, CompartmentSize, CompartmentType, IsClosed, IsLocked, IsOutOfService)
                    VALUES ($name, $size, $type, $closed, $locked, $outOfService);
                    SELECT last_insert_rowid();";
                cmd.Parameters.AddWithValue("$name", compartment.CompartmentName);
                cmd.Parameters.AddWithValue("$size", compartment.CompartmentSize);
                cmd.Parameters.AddWithValue("$type", (object)compartment.CompartmentType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$closed", compartment.IsClosed ? 1 : 0);
                cmd.Parameters.AddWithValue("$locked", compartment.IsLocked ? 1 : 0);
                cmd.Parameters.AddWithValue("$outOfService", compartment.IsOutOfService ? 1 : 0);
                compartmentId = (long)cmd.ExecuteScalar();
            }

            // Insert Shipments
            if (compartment.Shipments != null)
            {
                foreach (var shipment in compartment.Shipments)
                {
                    using var cmd = connection.CreateCommand();
                    cmd.CommandText = @"
                        INSERT INTO Shipment (CompartmentId, TrackingNumber, TimeStored, TimeOverdue, TimeStayed)
                        VALUES ($compartmentId, $tracking, $stored, $overdue, $stayed);";
                    cmd.Parameters.AddWithValue("$compartmentId", compartmentId);
                    cmd.Parameters.AddWithValue("$tracking", shipment.TrackingNumber);
                    cmd.Parameters.AddWithValue("$stored", shipment.TimeStored.ToString("s"));
                    cmd.Parameters.AddWithValue("$overdue", shipment.TimeOverdue.ToString("s"));
                    cmd.Parameters.AddWithValue("$stayed", shipment.TimeStayed);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateShipments(long compartmentId, List<Shipment> shipments)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Get existing shipment IDs for this compartment
            var existingIds = new HashSet<long>();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT Id FROM Shipment WHERE CompartmentId = $compartmentId";
                cmd.Parameters.AddWithValue("$compartmentId", compartmentId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    existingIds.Add(reader.GetInt64(0));
                }
            }

            // Track updated/added shipment IDs
            var processedIds = new HashSet<long>();

            foreach (var shipment in shipments)
            {
                if (shipment.Id == 0)
                {
                    // Insert new shipment
                    using var insertCmd = connection.CreateCommand();
                    insertCmd.CommandText = @"
                        INSERT INTO Shipment (CompartmentId, TrackingNumber, TimeStored, TimeOverdue, TimeStayed)
                        VALUES ($compartmentId, $tracking, $stored, $overdue, $stayed);
                        SELECT last_insert_rowid();";
                    insertCmd.Parameters.AddWithValue("$compartmentId", compartmentId);
                    insertCmd.Parameters.AddWithValue("$tracking", shipment.TrackingNumber);
                    insertCmd.Parameters.AddWithValue("$stored", shipment.TimeStored.ToString("s"));
                    insertCmd.Parameters.AddWithValue("$overdue", shipment.TimeOverdue.ToString("s"));
                    insertCmd.Parameters.AddWithValue("$stayed", shipment.TimeStayed);
                    var newId = (long)insertCmd.ExecuteScalar();
                    shipment.Id = newId; // Update model with new DB Id
                    processedIds.Add(newId);
                }
                else
                {
                    // Update existing shipment
                    using var updateCmd = connection.CreateCommand();
                    updateCmd.CommandText = @"
                        UPDATE Shipment SET
                            TrackingNumber = $tracking,
                            TimeStored = $stored,
                            TimeOverdue = $overdue,
                            TimeStayed = $stayed
                        WHERE Id = $id;";
                    updateCmd.Parameters.AddWithValue("$tracking", shipment.TrackingNumber);
                    updateCmd.Parameters.AddWithValue("$stored", shipment.TimeStored.ToString("s"));
                    updateCmd.Parameters.AddWithValue("$overdue", shipment.TimeOverdue.ToString("s"));
                    updateCmd.Parameters.AddWithValue("$stayed", shipment.TimeStayed);
                    updateCmd.Parameters.AddWithValue("$id", shipment.Id);
                    updateCmd.ExecuteNonQuery();
                    processedIds.Add(shipment.Id);
                }
            }

            // Delete shipments that were removed in the UI
            foreach (var id in existingIds)
            {
                if (!processedIds.Contains(id))
                {
                    using var deleteCmd = connection.CreateCommand();
                    deleteCmd.CommandText = "DELETE FROM Shipment WHERE Id = $id";
                    deleteCmd.Parameters.AddWithValue("$id", id);
                    deleteCmd.ExecuteNonQuery();
                }
            }
        }
    }
}


