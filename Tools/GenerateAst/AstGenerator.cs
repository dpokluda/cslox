namespace GenerateAst;

public class AstGenerator
{
    private static HashSet<string> Keywords = new HashSet<string>
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while"
    };
    
    public void DefineAst(string[] imports, string baseName, string outputDir, string[] types)
    {
        ConsoleEx.WriteLine(ConsoleColor.Yellow, $"Generating AST for {baseName}...");
        string path = Path.Combine(outputDir, baseName + ".cs");
        using StreamWriter writer = new StreamWriter(path, false, System.Text.Encoding.UTF8);

        // header
        writer.WriteLine($"//[ Appendix II {baseName}");
        foreach (var import in imports)
        {
            writer.WriteLine(import);
        }
        writer.WriteLine();

        // define the base abstract class with Accept method and visitor interface
        List<(string ClassName, string Fields)> typeFields = new List<(string, string)>();
        foreach (var type in types)
        {
            var parts = type.Split(':', 2);
            var className = parts[0].Trim();
            var fields = parts[1].Trim();
            
            typeFields.Add((className, fields));
        }
        
        writer.WriteLine($"public abstract class {baseName}");
        writer.WriteLine("{");
        DefineVisitor(writer, typeFields);

        writer.WriteLine("    public abstract T Accept<T>(IVisitor<T> visitor);");
        writer.WriteLine("}");
        writer.WriteLine();

        // define the concrete AST classes
        foreach (var (className, fields) in typeFields)
        {
            DefineType(writer, baseName, className, fields);
        }
        
        // footer
        writer.WriteLine($"//] Appendix II {baseName}");
        
        writer.Close();
        ConsoleEx.WriteLine(ConsoleColor.Green, $"Generated {baseName} in {path}.");
    }

    private static void DefineVisitor(StreamWriter writer, List<(string ClassName, string Fields)> typeFields)
    {
        writer.WriteLine("     public interface IVisitor<T>");
        writer.WriteLine("     {");
        
        // finish the IVisitor interface definition
        foreach (var (className, fields) in typeFields)
        {
            writer.WriteLine($"         T Visit{className}({className} expr);");
        }
        writer.WriteLine("     }");
        writer.WriteLine();
    }

    private void DefineType(StreamWriter writer, string baseName, string className, string fields)
    {
        writer.WriteLine($"public class {className} : {baseName}");
        writer.WriteLine("{");

        List<(string FieldType, string FieldName, string FieldIdent)> fieldsList = new List<(string, string, string)>();
        foreach (var field in fields.Split(',', StringSplitOptions.TrimEntries))
        {
            var fieldParts = field.Trim().Split(' ', StringSplitOptions.TrimEntries);
            var fieldType = fieldParts[0];
            var fieldName = SafeProp(fieldParts[1]);
            var fieldIdent = SafeIdent(fieldParts[1]);
            fieldsList.Add((fieldType, fieldName, fieldIdent));
        }
        
        // constructor
        writer.Write($"    public {className}(");
        for (int i = 0; i < fieldsList.Count; i++)
        {
            var (fieldType, fieldName, fieldIdent) = fieldsList[i];
            var comma = i < fieldsList.Count - 1 ? ", " : ")";
            writer.Write($"{fieldType} {fieldIdent}{comma}");
        }
        writer.WriteLine("\n    {");
        foreach (var (fieldType, fieldName, fieldIdent) in fieldsList)
        {
            writer.WriteLine($"        {SafeProp(fieldName)} = {fieldIdent};");
        }
        writer.WriteLine("    }");
        writer.WriteLine();

        foreach (var (fieldType, fieldName, fieldIdent) in fieldsList)
        {
            writer.WriteLine($"    public {fieldType} {fieldName} {{ get; init; }}");
        }
        
        writer.WriteLine();
        writer.WriteLine("    public override T Accept<T>(IVisitor<T> visitor)");
        writer.WriteLine("    {");
        writer.WriteLine($"        return visitor.Visit{className}(this);");
        writer.WriteLine("    }");
        
        writer.WriteLine("}");
        writer.WriteLine();
    }

    private string SafeIdent(string name)
    {
        if (Keywords.Contains(name))
        {
            return $"{name}_";
        }
        
        return name;
    }

    private string SafeProp(string name)
    {
        name = char.ToUpper(name[0]) + name.Substring(1);
        
        return name;
    }
}