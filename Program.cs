using BenchmarkDotNet.Running;
using LiteDB;

// Defina o nome do arquivo de banco de dados
const string databaseName = "LiteDBSample.db";

if(File.Exists(databaseName)) File.Delete(databaseName);

// Crie ou abra um banco de dados
using var db = new LiteDatabase(databaseName);

// Obtenha uma coleção chamada "pessoas". Caso não exista, na primeira inserção ela será criada
var pessoas = db.GetCollection<Pessoa>("pessoas");

// ChatGPT ajudou na criação dessa massa de dados ;)
var pessoa1 = new Pessoa { Id = 1, Nome = "José", Email = "jose@email.com", Idade = 30, UF = "SP" };
var pessoa2 = new Pessoa { Id = 2, Nome = "Maria", Email = "maria@email.com", Idade = 25, UF = "BH" };
var pessoa3 = new Pessoa { Id = 3, Nome = "Antônio", Email = "antonio@email.com", Idade = 28, UF = "SP" };
var pessoa4 = new Pessoa { Id = 4, Nome = "Sofia", Email = "sofia@email.com", Idade = 35, UF = "AC" };
var pessoa5 = new Pessoa { Id = 5, Nome = "Carlos", Email = "carlos@email.com", Idade = 22, UF = "CE" };

// Inserir as pessoas na coleção
// Aqui podemos notar que é possível efetuar um insert em lote \o/
pessoas.InsertBulk(new List<Pessoa> { pessoa1, pessoa2, pessoa3, pessoa4, pessoa5 });

try
{
    // Será que essa pessoa vai ser inserida na collection?
    var pessoa7 = new Pessoa { Id = 1, Nome = "Mario", Email = "mario@email.com", Idade = 36, UF = "PE" };
    pessoas.Insert(pessoa7);
}
catch (Exception e)
{
    // LiteDB.LiteException: Cannot insert duplicate key in unique index '_id'. The duplicate value is '1'.
}

// Encontre uma pessoa pelo email
var maria = pessoas.FindOne(p => p.Email == "maria@email.com");
Console.WriteLine(maria);

// Consulta via LINQ: Encontre todas as pessoas com idade maior que 25 anos
var pessoasComMaisDe25Anos = from pessoa in pessoas.Query() where pessoa.Idade > 25 select pessoa;
Console.WriteLine("\nPessoas com mais de 25 anos:");
foreach (var pessoa in pessoasComMaisDe25Anos.ToList())
{
    Console.WriteLine(pessoa);
}

// Consulta com agrupamento: Encontre a quantidade de pessoas por UF
var pessoasPorUF = pessoas
    .FindAll()
    .GroupBy(p => p.UF)
    .Select(g => new
    {
        UF = g.Key,
        Quantidade = g.Count()
    });
Console.WriteLine("\nPessoas por UF:");
foreach (var pessoa in pessoasPorUF)
{
    Console.WriteLine(pessoa);
}

// Atualizar a idade do Antônio
var antonio = pessoas.FindOne(x => x.Id == 3);
if (antonio != null)
{
    antonio.Idade = 26;
    pessoas.Update(antonio);
    Console.WriteLine("\nAntônio após a atualização de idade:");
    Console.WriteLine(antonio);
}

// Excluir uma pessoa
var jose = pessoas.FindOne(x => x.Id == 3);
if (jose != null)
{
    pessoas.Delete(jose.Id);
    Console.WriteLine("\nJosé foi excluído...");
}

#if  !DEBUG
// Teste de performance
// pra executar: sudo dotnet run --configuration Release
var summary = BenchmarkRunner.Run<LiteDBBenchmark>();
#endif
