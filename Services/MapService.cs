using Npgsql;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class MapService
{
    private readonly string _connectionString;

    public MapService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<List<MapData>> GetMapDataAsync()
    {
        var mapDataList = new List<MapData>();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand("SELECT id, name, ST_AsText(geom) FROM map_data", conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var id = reader.GetInt32(0);
            var name = reader.GetString(1);
            var coordinates = reader.GetString(2);

            mapDataList.Add(new MapData { Id = id, Name = name, Coordinates = coordinates });
        }

        return mapDataList;
    }

    public async Task<string> GetGeoJsonDataAsync()
    {
        var geoJsonData = new List<GeoJsonFeature>();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand("SELECT ST_AsGeoJSON(geom) FROM countries", conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var geoJson = reader.GetString(0);
            geoJsonData.Add(JsonSerializer.Deserialize<GeoJsonFeature>(geoJson));
        }

        var featureCollection = new GeoJsonFeatureCollection { Features = geoJsonData };
        return JsonSerializer.Serialize(featureCollection);
    }
}

public class MapData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Coordinates { get; set; }
}

public class GeoJsonFeature
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("geometry")]
    public JsonElement Geometry { get; set; }

    [JsonPropertyName("properties")]
    public JsonElement Properties { get; set; }
}

public class GeoJsonFeatureCollection
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "FeatureCollection";

    [JsonPropertyName("features")]
    public List<GeoJsonFeature> Features { get; set; }
}
