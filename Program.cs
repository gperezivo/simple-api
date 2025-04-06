using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/", (HttpContext ctx, IConfiguration config) =>
{
    var myVar = config.GetSection("MyVar")?.Value;
    var version = config.GetSection("Version")?.Value;

    return new
    {
        MyVar = myVar ?? "Hello World",
        MyVar2 = config.GetValue<string>("MyVar2") ?? "Hello World",
        version = version
    };
}
);
app.MapGet("/external-api", async (HttpContext ctx, IConfiguration config, IHttpClientFactory httpClientFactory) =>
{
    var apiUrl = config.GetSection("ApiUrl")?.Value;
    var version = config.GetSection("Version")?.Value;

    var httpClient = httpClientFactory.CreateClient();
    var response = await httpClient.GetAsync(apiUrl);
    var content = await response.Content.ReadAsStringAsync();

    return new
    {
        ApiResponse = content,
        version = version
    };
}
);
app.MapGet("/sql-connection", (HttpContext ctx, IConfiguration config) =>
{
    var sqlConnectionBuilder = new SqlConnectionStringBuilder()
    {
        DataSource = config.GetSection("Database").GetValue<string>("Server"),
        UserID = config.GetSection("Database").GetValue<string>("User"),
        Password = config.GetSection("Database").GetValue<string>("Password"),
        InitialCatalog = config.GetSection("Database").GetValue<string>("Name"),
        MultipleActiveResultSets = true,
        ConnectTimeout = 30
    };
    var version = config.GetSection("Version")?.Value;
    return new
    {
        SqlConnection = sqlConnectionBuilder.ConnectionString,
        version = version
    };
}
);
app.MapGet("/sql", async (HttpContext ctx, IConfiguration config) =>
{

    var sqlConnectionBuilder = new SqlConnectionStringBuilder()
    {
        DataSource = config.GetSection("Database").GetValue<string>("Server"),
        UserID = config.GetSection("Database").GetValue<string>("User"),
        Password = config.GetSection("Database").GetValue<string>("Password"),
        InitialCatalog = config.GetSection("Database").GetValue<string>("Name"),
        MultipleActiveResultSets = true,
        ConnectTimeout = 30
    };
    var version = config.GetSection("Version")?.Value;
    using var connection = new SqlConnection(sqlConnectionBuilder.ConnectionString);
    await connection.OpenAsync();
    using var command = new SqlCommand("SELECT * FROM sys.tables", connection);
    var reader = await command.ExecuteReaderAsync();
    var result = new List<dynamic>();
    while (await reader.ReadAsync())
    {
        result.Add(new
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1)
        });
    }
    return new
    {
        SqlResponse = result,
        version = version
    };
}
);
app.MapGet("/ip",
    (HttpContext ctx, IConfiguration config) =>
    {
        var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

        return new
        {
            Ip = ip
        };
    }
);
app.MapGet("/sort",
    (HttpContext ctx, IConfiguration config, int count) =>
    {
        if (count == 0) count = 1000;
        var randoms = new int[count];
        var random = new Random();
        for (int i = 0; i < randoms.Length; i++)
        {
            randoms[i] = random.Next(0, 1000);
        }
        for (int i = 0; i < randoms.Length; i++)
        {
            for (int j = 0; j < randoms.Length - 1; j++)
            {
                if (randoms[j] > randoms[j + 1])
                {
                    var temp = randoms[j];
                    randoms[j] = randoms[j + 1];
                    randoms[j + 1] = temp;
                }
            }
        }
        return Task.FromResult(new { value = randoms });
    });

await app.RunAsync();
