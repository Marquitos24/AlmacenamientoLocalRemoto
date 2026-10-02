namespace ServiceRemote.Config;

public class ConfigRedis
{
    /// <summary>Cadena de conexión a Redis.</summary>
    public string ConnectionString { get; set; } = "localhost:6379";

    /// <summary>
    /// Si es true, vacía la caché al arrancar.
    /// </summary>
    public bool DropData { get; set; } = true;
}