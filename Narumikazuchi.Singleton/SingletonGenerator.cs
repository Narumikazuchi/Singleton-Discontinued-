namespace Narumikazuchi.Singletons;

[Generator]
public sealed partial class SingletonGenerator
{
    public SingletonGenerator()
    {
        m_DefaultAttributeSourceText = SourceText.From(text: DEFAULT_ATTRIBUTE_SOURCE,
                                                       encoding: Encoding.UTF8);
        m_ParametersAttributeSourceText = SourceText.From(text: PARAMETERS_ATTRIBUTE_SOURCE,
                                                          encoding: Encoding.UTF8);
        m_InstantiationAttributeSourceText = SourceText.From(text: INSTANTIATION_ATTRIBUTE_SOURCE,
                                                             encoding: Encoding.UTF8);
        m_AbstractClassSourceText = SourceText.From(text: ABSTRACT_CLASS_SOURCE,
                                                    encoding: Encoding.UTF8);
    }
}

// Non-Public
partial class SingletonGenerator
{
    private void InjectSources(GeneratorExecutionContext context)
    {
        context.AddSource(hintName: "SingletonAttribute.g.cs",
                          sourceText: m_DefaultAttributeSourceText);
        context.AddSource(hintName: "SingletonWithParametersAttribute.g.cs",
                          sourceText: m_ParametersAttributeSourceText);
        context.AddSource(hintName: "SingletonWithInstantiationAttribute.g.cs",
                          sourceText: m_InstantiationAttributeSourceText);
        context.AddSource(hintName: "Singleton.g.cs",
                          sourceText: m_AbstractClassSourceText);
    }

    private Compilation GetCompilation(GeneratorExecutionContext context)
    {
        CSharpParseOptions options = (CSharpParseOptions)context.Compilation
                                                                .SyntaxTrees
                                                                .First()
                                                                .Options;

        Compilation compilation = context.Compilation
                                         .AddSyntaxTrees(CSharpSyntaxTree.ParseText(text: m_AbstractClassSourceText,
                                                                                    options: options),
                                                         CSharpSyntaxTree.ParseText(text: m_DefaultAttributeSourceText,
                                                                                    options: options),
                                                         CSharpSyntaxTree.ParseText(text: m_ParametersAttributeSourceText,
                                                                                    options: options),
                                                         CSharpSyntaxTree.ParseText(text: m_InstantiationAttributeSourceText,
                                                                                    options: options));
        return compilation;
    }

    private static INamedTypeSymbol GetSymbol(Compilation compilation,
                                              ClassDeclarationSyntax syntax)
    {
        SemanticModel model = compilation.GetSemanticModel(syntax.SyntaxTree);
        return model.GetDeclaredSymbol(syntax)!;
    }

    private String GenerateDefault(INamedTypeSymbol symbol)
    {
        String @namespace = symbol.ContainingNamespace
                                  .ToDisplayString();

        String result = $@"using System;

namespace {@namespace};

partial class {symbol.Name} : Narumikazuchi.Singletons.Singleton
{{
    private {symbol.Name}() : base()
    {{ }}

    public static {symbol.Name} Instance
    {{ 
        get
        {{
            if (s_Instance == null)
            {{
                s_Instance = new {symbol.Name}();
            }}
            return s_Instance;
        }}
    }}

    public static Boolean IsInstanceCreated {{ get; }} = s_Instance != null;

    private static {symbol.Name} s_Instance;
}}";

        return result;
    }

    private String GenerateWithInstantiation(INamedTypeSymbol symbol)
    {
        String @namespace = symbol.ContainingNamespace
                                  .ToDisplayString();

        String result = $@"using System;

namespace {@namespace};

partial class {symbol.Name} : Narumikazuchi.Singletons.Singleton
{{
    private {symbol.Name}() : base()
    {{ }}

    public static void CreateInstance()
    {{
        if (s_Instance == null)
        {{
            s_Instance = CreateInstanceWithParameters();
        }}
    }}

    private static partial {symbol.Name} CreateInstanceWithParameters();

    public static {symbol.Name} Instance
    {{ 
        get
        {{
            if (s_Instance == null)
            {{
                throw new NullReferenceException(""Instance has not yet been created. Please call \""CreateInstance\"" before any attempt to retrieve the instance value."");
            }}
            return s_Instance;
        }}
    }}

    public static Boolean IsInstanceCreated {{ get; }} = s_Instance != null;

    private static {symbol.Name} s_Instance;
}}";

        return result;
    }

    private String GenerateParameterInterface(INamedTypeSymbol symbol,
                                              String interfaceName)
    {
        String @namespace = symbol.ContainingNamespace
                                  .ToDisplayString();

        String result = $@"namespace {@namespace};

partial interface {interfaceName}
{{ }}";

        return result;
    }

    private String GenerateParameterized(INamedTypeSymbol symbol,
                                         String interfaceName)
    {
        String @namespace = symbol.ContainingNamespace
                                  .ToDisplayString();

        String result = $@"using System;

namespace {@namespace};

partial class {symbol.Name} : Narumikazuchi.Singletons.Singleton
{{
    private {symbol.Name}() : base()
    {{ }}

    public static void CreateInstance({interfaceName} parameters)
    {{
        if (s_Instance == null)
        {{
            s_Instance = CreateInstanceWithParameters(parameters);
        }}
    }}

    private static partial {symbol.Name} CreateInstanceWithParameters({interfaceName} parameters);

    public static {symbol.Name} Instance
    {{ 
        get
        {{
            if (s_Instance == null)
            {{
                throw new NullReferenceException(""Instance has not yet been created. Please call \""CreateInstance\"" before any attempt to retrieve the instance value."");
            }}
            return s_Instance;
        }}
    }}

    public static Boolean IsInstanceCreated {{ get; }} = s_Instance != null;

    private static {symbol.Name} s_Instance;
}}";

        return result;
    }

    private readonly SourceText m_DefaultAttributeSourceText;
    private readonly SourceText m_ParametersAttributeSourceText;
    private readonly SourceText m_InstantiationAttributeSourceText;
    private readonly SourceText m_AbstractClassSourceText;
    private INamedTypeSymbol? m_DefaultAttribute;
    private INamedTypeSymbol? m_ParametersAttribute;
    private INamedTypeSymbol? m_InstantiationAttribute;

    private const String DEFAULT_ATTRIBUTE_SOURCE = @"using System;

namespace Narumikazuchi.Singletons;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class SingletonAttribute : Attribute
{
    public SingletonAttribute()
    { }
}";
    private const String PARAMETERS_ATTRIBUTE_SOURCE = @"using System;

namespace Narumikazuchi.Singletons;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class SingletonWithParametersAttribute : Attribute
{
    public SingletonWithParametersAttribute()
    { }
    public SingletonWithParametersAttribute(String parameterInterface)
    { }
}";
    private const String INSTANTIATION_ATTRIBUTE_SOURCE = @"using System;

namespace Narumikazuchi.Singletons;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class SingletonWithInstantiationAttribute : Attribute
{
    public SingletonWithInstantiationAttribute()
    { }
}";
    private const String ABSTRACT_CLASS_SOURCE = @"using System;
using System.Collections.Generic;

namespace Narumikazuchi.Singletons;

public abstract class Singleton
{
    protected Singleton()
    {
        String name = this.GetType()
                          .AssemblyQualifiedName!;
        if (s_Initialized.Contains(item: name))
        {
            InvalidOperationException exception = new(message: MULTIPLE_INSTANCES_ARE_INVALID);
            exception.Data
                     .Add(key: ""Typename"",
                          value: this.GetType()
                                     .FullName);
            exception.Data
                     .Add(key: ""AssemblyQualifiedName"",
                          value: this.GetType()
                                     .AssemblyQualifiedName);
        }
        s_Initialized.Add(name);
    }

    private static readonly List<String> s_Initialized = new();

    private const String MULTIPLE_INSTANCES_ARE_INVALID = ""Can't create multiple instances of the same singleton."";
}";
}

// ISourceGenerator
partial class SingletonGenerator : ISourceGenerator
{
    void ISourceGenerator.Initialize(GeneratorInitializationContext context) =>
        context.RegisterForSyntaxNotifications(() => new __SyntaxReceiver());

    void ISourceGenerator.Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not __SyntaxReceiver receiver)
        {
            return;
        }

        this.InjectSources(context);
        Compilation compilation = this.GetCompilation(context);

        m_DefaultAttribute = compilation.GetTypeByMetadataName("Narumikazuchi.Singletons.SingletonAttribute");
        m_ParametersAttribute = compilation.GetTypeByMetadataName("Narumikazuchi.Singletons.SingletonWithParametersAttribute");
        m_InstantiationAttribute = compilation.GetTypeByMetadataName("Narumikazuchi.Singletons.SingletonWithInstantiationAttribute");

        IEnumerable<INamedTypeSymbol> symbols = receiver.Candidates
                                                        .Select(x => GetSymbol(compilation, x));
        foreach (INamedTypeSymbol symbol in symbols)
        {
            if (symbol.TryGetAttribute(attributeType: m_DefaultAttribute!,
                                       attributes: out IEnumerable<AttributeData> attributes))
            {
                context.AddSource(hintName: $"{symbol.Name}.g.cs",
                                  sourceText: SourceText.From(text: this.GenerateDefault(symbol),
                                                              encoding: Encoding.UTF8));
                continue;
            }

            if (symbol.TryGetAttribute(attributeType: m_InstantiationAttribute!,
                                       attributes: out attributes))
            {
                context.AddSource(hintName: $"{symbol.Name}.g.cs",
                                  sourceText: SourceText.From(text: this.GenerateWithInstantiation(symbol),
                                                              encoding: Encoding.UTF8));
                continue;
            }

            if (symbol.TryGetAttribute(attributeType: m_ParametersAttribute!,
                                       attributes: out attributes))
            {
                AttributeData attribute = attributes.Single();

                String interfaceName = "I" + symbol.Name + "ConstructorParameters";
                if (attribute.ConstructorArguments
                             .Any())
                {
                    TypedConstant constant = attribute.ConstructorArguments
                                                      .First();
                    if (constant.Value is not null)
                    {
                        String temp = constant.Value
                                              .ToString();
                        if (!String.IsNullOrWhiteSpace(temp))
                        {
                            interfaceName = temp;
                        }
                    }
                }

                context.AddSource(hintName: $"{interfaceName}.g.cs",
                                  sourceText: SourceText.From(text: this.GenerateParameterInterface(symbol: symbol,
                                                                                                    interfaceName: interfaceName),
                                                              encoding: Encoding.UTF8));

                context.AddSource(hintName: $"{symbol.Name}.g.cs",
                                  sourceText: SourceText.From(text: this.GenerateParameterized(symbol: symbol,
                                                                                               interfaceName: interfaceName),
                                                              encoding: Encoding.UTF8));
                continue;
            }
        }
    }
}