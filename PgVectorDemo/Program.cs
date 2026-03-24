using Npgsql;
using Pgvector;

var connectionString = "Host=localhost;Database=pgvector_demo;Username=postgres;Password=senha123";

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.UseVector();

await using var dataSource = dataSourceBuilder.Build();
await using var conn = await dataSource.OpenConnectionAsync();

Console.WriteLine("Conectado com o PostgreSQL com pgvector sem problemas.");

Console.WriteLine("Inserindo docs com embeddings...");

var docs = new[]
{
    ("Gosto muito de programar em C#", new float[] {0.9f, 0.1f, 0.2f}),
    ("Adoro jogar futebol no final de semana", new float[] {0.1f, 0.9f, 0.1f}),
    ("C# e .NET são ótimas tecnologias", new float[] {0.85f, 0.15f, 0.3f}),
    ("Prefiro assistir filmes em casa", new float[]{0.2f, 0.7f, 0.5f}),
};