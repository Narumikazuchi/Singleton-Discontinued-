namespace Narumikazuchi.Singletons;

internal class __SyntaxReceiver : ISyntaxReceiver
{
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax @class &&
            @class.AttributeLists
                  .Count > 0)
        {
            this.Candidates
                .Add(@class);
        }
    }

    public IList<ClassDeclarationSyntax> Candidates { get; } = new List<ClassDeclarationSyntax>();

    public List<String> Log { get; } = new();
}