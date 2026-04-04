using System;

namespace Arpg.Client.Controls.Common;

/// <summary>
/// Provedor de tokens para cancelamento lógico de animações e transições.
/// </summary>
public class TransitionToken
{
    private Guid _id = Guid.NewGuid();

    /// <summary>
    /// Gera um novo token, invalidando qualquer comparação com o anterior.
    /// </summary>
    /// <returns>Uma nova instância de TransitionToken.</returns>
    public static TransitionToken Create() => new();

    /// <summary>
    /// Verifica se este token ainda é o mesmo que o fornecido.
    /// </summary>
    public bool IsSame(TransitionToken? other) => other != null && _id == other._id;

    public override bool Equals(object? obj) => obj is TransitionToken other && _id == other._id;
    public override int GetHashCode() => _id.GetHashCode();

    public static bool operator ==(TransitionToken? a, TransitionToken? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a._id == b._id;
    }

    public static bool operator !=(TransitionToken? a, TransitionToken? b) => !(a == b);
}
