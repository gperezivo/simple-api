var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/", (HttpContext ctx, IConfiguration config)  => {
        var myVar = config.GetSection("MyVar")?.Value;
        
        return new { 
            MyVar= myVar ?? "Hello World" , 
            MyVar2= config.GetValue<string>("MyVar2") ?? "Hello World",
            version= "v2"
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
