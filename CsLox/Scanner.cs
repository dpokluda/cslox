namespace CsLox;

public class Scanner
{
  private string _source;

  public Scanner(string source)
  {
    _source = source;
  }

  public Task<IEnumerable<Token>> ScanTokensAsync()
  {
    return Task.FromResult(new List<Token>().AsEnumerable());
  }
}
