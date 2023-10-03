using BenchmarkDotNet.Attributes;
using LiteDB;

public class LiteDBBenchmark
{
    private LiteDatabase _db;
    private ILiteCollection<Pessoa> _pessoasComIndice;
    private ILiteCollection<Pessoa> _pessoasSemIndice;

    [GlobalSetup]
    public void Setup()
    {
        var caminho = "BenchmarkIndice.db";
        if (File.Exists(caminho))File.Delete(caminho);
        _db = new LiteDatabase(caminho);

        // Coleção com índice no campo de e-mail
        _pessoasComIndice = _db.GetCollection<Pessoa>("pessoasComIndice");
        _pessoasComIndice.EnsureIndex(x => x.Email);

        // Coleção sem índice no campo de e-mail
        _pessoasSemIndice = _db.GetCollection<Pessoa>("pessoasSemIndice");

        for (var i = 0; i < 1_000; i++)
        {
            var pessoa = new Pessoa { Nome = $"Pessoa {i}", Email = $"email_{i}@email.com", Idade = i, UF = "SP" };

            _pessoasComIndice.Insert(pessoa);
            _pessoasSemIndice.Insert(pessoa);
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _db.Dispose();
    }

    [Benchmark]
    public void BuscarComIndice()
        => _pessoasComIndice.FindOne(p => p.Email == "email_1@exemplo.com");

    [Benchmark]
    public void BuscarSemIndice()
        => _pessoasSemIndice.FindOne(p => p.Email == "email_1@exemplo.com");
}