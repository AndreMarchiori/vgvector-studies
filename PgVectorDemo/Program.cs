using Npgsql;
using Pgvector;

var connectionString = "Host=localhost;Database=pgvector_demo;Username=postgres;Password=senha123";

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.UseVector();

await using var dataSource = dataSourceBuilder.Build();
await using var conn = await dataSource.OpenConnectionAsync();

Console.WriteLine("Conectado com o PostgreSQL com pgvector sem problemas.");

Console.WriteLine("Inserindo docs com embeddings...");

var documentos = new[]
{
    ("Gosto muito de programar em C#", new float[] {0.9f, 0.1f, 0.2f}),
    ("Adoro jogar futebol no final de semana", new float[] {0.1f, 0.9f, 0.1f}),
    ("C# e .NET são ótimas tecnologias", new float[] {0.85f, 0.15f, 0.3f}),
    ("Prefiro assistir filmes em casa", new float[]{0.2f, 0.7f, 0.5f}),
};

foreach (var (texto, valores) in documentos)
{
    var vetor = new Vector(valores);

    await using var cmd = new NpgsqlCommand(
        "INSERT INTO documentos (conteudo, embedding) VALUES (@c, @e)", conn);

    cmd.Parameters.AddWithValue("c", texto);
    cmd.Parameters.AddWithValue("e", vetor);

    
    await cmd.ExecuteNonQueryAsync();
    Console.WriteLine($"Inserido: \"{texto}\"");
}

Console.WriteLine("\n Buscando docs similares a 'programação'...\n");

var vertorConsulta = new Vector(new float[] { 0.88f, 0.12f, 0.25f });

await using var cmdBusca = new NpgsqlCommand(@"
SELECT conteudo,
        1 - (embedding <=> @q) AS similaridade
FROM    documentos
ORDER   BY embedding <=> @q
LIMIT   3;
", conn);

cmdBusca.Parameters.AddWithValue("q", vertorConsulta);

await using var reader = await cmdBusca.ExecuteReaderAsync();


Console.WriteLine($"{"Documento",-45} {"Similaridade",12}");
Console.WriteLine(new string('-', 60));

while (await reader.ReadAsync())
{
    var texto        = reader.GetString(0);
    var similaridade = reader.GetDouble(1);
    Console.WriteLine($"{texto,-45} {similaridade,12:P1}");
}

Console.WriteLine("\n Demo concluída!");