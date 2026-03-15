using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using NCalc;
using NCalc.Handlers;

namespace Arpg.Application.Services;

public partial class ExpressionResolver
{
    [AttributeUsage(AttributeTargets.Method)]
    private class ExpressionFunctionAttribute : Attribute { }

    private static EvaluateFunctionHandler? _functions;

    public ExpressionResolver()
    {

        var methods = typeof(ExpressionResolver)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(o => o.GetCustomAttribute<ExpressionFunctionAttribute>() != null);

        foreach (var method in methods)
        {
            var handler = (EvaluateFunctionHandler)Delegate.CreateDelegate(typeof(EvaluateFunctionHandler), method);
            _functions = (EvaluateFunctionHandler?)Delegate.Combine(_functions, handler);
        }
    }

    [ExpressionFunction]
    public static void Round(string name, FunctionArgs args)
    {
        if (args.HasResult) return;

        if (name.Equals("round", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Parameters.Length < 1) 
            {
                args.Result = 0; 
                return;
            }

            var value = Convert.ToDouble(args.Parameters[0].Evaluate());

            args.Result = args.Parameters.Length > 1 ? Math.Round(value, Convert.ToInt32(args.Parameters[1].Evaluate())) : Math.Round(value);
        }
    }

    public string? Resolve(string rawExpression)
    {
        var match = GetStatisticalFunction().Match(rawExpression);
        string expression;
        int quantity;

        if (match.Success)
        {
            quantity = int.Parse(match.Groups[1].Value);
            expression = match.Groups[2].Value.Trim();
        }
        else
        {
            expression = rawExpression.Trim();
            quantity = 1;
        }

        StringBuilder sb = new();

        for (var times = 0; times < quantity; times++)
        {
            var expressionWithValues = ResolveDices(expression, out var diceResults);

            Expression nExpression = new(expressionWithValues, ExpressionOptions.IgnoreCaseAtBuiltInFunctions);

            if (_functions != null)
                nExpression.EvaluateFunction += _functions;

            if (nExpression.HasErrors())
                return null;

            string stringResult;

            try
            {
                var result = nExpression.Evaluate();

                stringResult = result switch
                {
                    bool b => b ? "Verdadeiro" : "Falso",
                    double or int or decimal => FormatNumber(Convert.ToDouble(result)),
                    _ => result?.ToString() ?? "Desconhecido"
                };
            }
            catch (Exception)
            {
                return null;
            }

            sb.Append($"` {stringResult} ` ⟵ {diceResults}");
        }

        return sb.ToString();
    }

    private static string FormatNumber(double value)
    {
        if (double.IsInfinity(value)) return "Infinito";
        return double.IsNaN(value) ? "Não é um número" : value.ToString();
    }

    private string ResolveDices(string expression, out string diceResults)
    {
        List<List<int>> matches = [];
        int currentMatch = 0;

        string resultExpression = GetDice().Replace(expression, match =>
        {
            int dices = string.IsNullOrEmpty(match.Groups[1].Value) ? 1 : int.Parse(match.Groups[1].Value);
            int minimal = 1;
            int maximal = int.Parse(match.Groups[2].Value);

            if (match.Groups[4].Success)
            {
                minimal = int.Parse(match.Groups[2].Value);
                maximal = int.Parse(match.Groups[4].Value);
            }

            bool isExplosive = match.Groups[5].Success;

            int totalRoll = 0;
            List<int> rollHistory = [];

            dices = Math.Clamp(dices, 1, 100); 

            for (int index = 0; index < dices; index++)
            {
                int temp = Random.Shared.Next(minimal, maximal + 1);
                totalRoll += temp;
                rollHistory.Add(temp);

                if (isExplosive && temp == maximal)
                    dices++;

                if (dices > 200) break; 
            }

            matches.Add(rollHistory);
            return totalRoll.ToString();
        });

        diceResults = GetDice().Replace(expression, match =>
        {
            if (currentMatch >= matches.Count) return match.Value;

            List<int> rolls = matches[currentMatch++];
            
            int minimal = 1;
            int maximal = int.Parse(match.Groups[2].Value);
            if (match.Groups[4].Success)
            {
                minimal = int.Parse(match.Groups[2].Value);
                maximal = int.Parse(match.Groups[4].Value);
            }

            StringBuilder sb = new();
            sb.Append('[');
            
            for (int i = 0; i < rolls.Count; i++)
            {
                if (i > 0) sb.Append(", ");

                bool isCrit = rolls[i] == minimal || rolls[i] == maximal;
                if (isCrit) sb.Append("**").Append(rolls[i]).Append("**");
                else sb.Append(rolls[i]);
            }
            sb.Append(']');

            return sb.ToString();
        });

        return resultExpression;
    }

    [GeneratedRegex(@"(\d+)?d(\d+)(_(\d+))?(!)?", RegexOptions.IgnoreCase, "pt-BR")]
    private partial Regex GetDice();

    [GeneratedRegex(@"^(\d+)#(.*)$", RegexOptions.IgnoreCase, "pt-BR")]
    private partial Regex GetStatisticalFunction();
}