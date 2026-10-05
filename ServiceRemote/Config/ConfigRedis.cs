namespace ServiceRemote.Config;

public class ConfigRedis
{
    /// <summary>Cadena de conexión a Redis.</summary>
    public static string ConnectionString { get; set; } = "localhost:6379";
    
    public static string multiplexConectionString { get; set; } = "localhost";

    /// <summary>
    /// Si es true, vacía la caché al arrancar.
    /// </summary>
    public static bool DropData { get; set; } = true;
}