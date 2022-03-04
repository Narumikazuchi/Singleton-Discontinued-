namespace Narumikazuchi.Singletons;

internal static class __Extensions
{
    public static Boolean TryGetAttribute(this ISymbol symbol,
                                          INamedTypeSymbol attributeType,
                                          out IEnumerable<AttributeData> attributes)
    {
        attributes = symbol.GetAttributes()
                           .Where(x => SymbolEqualityComparer.Default
                                                             .Equals(x: x.AttributeClass,
                                                                     y: attributeType));
        return attributes.Any();
    }
}