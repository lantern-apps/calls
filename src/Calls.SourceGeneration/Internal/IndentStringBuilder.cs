using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Calls.SourceGeneration;

internal class IndentStringBuilder
{
    private const char IndentChar = ' ';
    private const int IndentLength = 4;
    private readonly StringBuilder _builder;
    private int _indent = 0;

    public IndentStringBuilder()
    {
        _builder = new(4096);
    }

    public IndentStringBuilder(StringBuilder builder, int indent)
    {
        _builder = builder;
        _indent = indent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine() => _builder.AppendLine();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IncreaseIndent()
    {
        _indent += IndentLength;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DecreaseIndent()
    {
        if (_indent >= IndentLength)
            _indent -= IndentLength;
    }

    public void Append(string value) => _builder.Append(value);

    public void Append(char value) => _builder.Append(value);

    public void AppendLine(string value)
    {
        if (_indent > 0)
            _builder.Append(IndentChar, _indent);
        _builder.AppendLine(value);
    }

    public void AppendIndentLine(string value)
    {
        _builder.Append(IndentChar, _indent + IndentLength);
        _builder.AppendLine(value);
    }

    public void AppendBlock(string value, Action block)
    {
        AppendLine(value);
        AppendBlock(block);
    }

    public void AppendBlock(Action block)
    {
        AppendLine("{");
        if (block != null)
        {
            _indent += IndentLength;
            block.Invoke();
            _indent -= IndentLength;
        }
        AppendLine("}");
    }

    public override string ToString() => _builder.ToString();
}
