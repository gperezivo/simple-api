using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/", (HttpContext ctx, IConfiguration config)  => {
        var myVar = config.GetSection("MyVar")?.Value;
        var version = config.GetSection("Version")?.Value;
        
        return new { 
            MyVar= myVar ?? "Hello World" , 
            MyVar2= config.GetValue<string>("MyVar2") ?? "Hello World",
            version = version
        };
    }
);
app.MapGet("/external-api", (HttpContext ctx, IConfiguration config)  => {
        var apiUrl = config.GetSection("ApiUrl")?.Value;
        var version = config.GetSection("Version")?.Value;
        //call to GET apiUrl 
        var httpClient = new HttpClient();
        var response = httpClient.GetAsync(apiUrl).Result;
        var content = response.Content.ReadAsStringAsync().Result;
        return new {
            ApiResponse= content,
            version= version
        };
    }
);
app.MapGet("/sql-connection", (HttpContext ctx, IConfiguration config)  => {
       var sqlConnectionBuilder = new SqlConnectionStringBuilder(){
            DataSource=config.GetSection("Database").GetValue<string>("Server"),
            UserID=config.GetSection("Database").GetValue<string>("User"),
            Password=config.GetSection("Database").GetValue<string>("Password"),
            InitialCatalog=config.GetSection("Database").GetValue<string>("Name"),
            MultipleActiveResultSets=true,
            ConnectTimeout=30
        };
        var version = config.GetSection("Version")?.Value;
        return new {
            SqlConnection = sqlConnectionBuilder.ConnectionString,
            version= version
        };
    }
);
app.MapGet("/sql", async (HttpContext ctx, IConfiguration config)  => {
        
        var sqlConnectionBuilder = new SqlConnectionStringBuilder(){
            DataSource=config.GetSection("Database").GetValue<string>("Server"),
            UserID=config.GetSection("Database").GetValue<string>("User"),
            Password=config.GetSection("Database").GetValue<string>("Password"),
            InitialCatalog=config.GetSection("Database").GetValue<string>("Name"),
            MultipleActiveResultSets=true,
            ConnectTimeout=30
        };
        var version = config.GetSection("Version")?.Value;
        var connection = new SqlConnection(sqlConnectionBuilder.ConnectionString);
        await connection.OpenAsync();
        var command = new SqlCommand("SELECT * FROM sys.tables", connection);
        var reader = await command.ExecuteReaderAsync();
        var result = new List<dynamic>();
        while (reader.Read())
        {
            result.Add(new {
                Id= reader.GetInt32(0),
                Name= reader.GetString(1)
            });
        }
        return new {
            SqlResponse= result,
            version=version
        };
    }
);
app.MapGet("/sort",
    (HttpContext ctx, IConfiguration config) => {
        // Create an array of 1000 randoms ints
        var randoms = new int[100000];
        for (int i = 0; i < randoms.Length; i++)
        {
            randoms[i] = new Random().Next(0, 1000);
        }
        // Sort the array with bubble sort
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

app.Run();
