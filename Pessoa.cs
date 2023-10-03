public class Pessoa
{
    public int Id {get; set;}
    public string Nome {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public int Idade {get; set;}
    public string UF {get; set;} = string.Empty;

    public override string ToString()
        => $"ID: {Id}, Nome: {Nome}, Email: {Email}, Idade: {Idade}, UF: {UF}";
}