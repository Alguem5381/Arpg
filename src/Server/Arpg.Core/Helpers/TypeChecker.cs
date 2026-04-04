using Arpg.Primitives.Enums.Template;

namespace Arpg.Core.Helpers;

public static class TypeChecker
{
    public static bool IsValueValidForType(object? value, FieldType type)
    {
        if (value == null) return true;
        
        return type switch
        {
            FieldType.Number => value is int or double or float or long,
            FieldType.Text or FieldType.TextArea => value is string,
            _ => true
        };
    }
}