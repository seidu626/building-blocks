namespace BuildingBlocks.Utilities;

public static class IdGeneratorSequence
{
    private static readonly char MinChar = 'A';
    private static readonly char MaxChar = 'Z';
    private static readonly int MinDigit = 1;
    private static readonly int MaxDigit = 10;
    private static int _fixedLength = 20;//zero means variable length
    private static int _currentDigit = 1;
    private static string _currentBase = "A";

    public static string NextId()
    {
        if (_currentBase[_currentBase.Length - 1] <= MaxChar)
        {
            if (_currentDigit <= MaxDigit)
            {
                var result = string.Empty;
                if (_fixedLength > 0)
                {
                    var prefixZeroCount = _fixedLength - _currentBase.Length;
                    if (prefixZeroCount < _currentDigit.ToString().Length)
                        throw new InvalidOperationException("The maximum length possible has been exeeded.");
                    result = result = _currentBase + _currentDigit.ToString("D" + prefixZeroCount.ToString());
                }
                else
                {
                    result = _currentBase + _currentDigit.ToString();
                }
                _currentDigit++;
                return result;
            }
            else
            {
                _currentDigit = MinDigit;
                if (_currentBase[_currentBase.Length - 1] == MaxChar)
                {
                    _currentBase = _currentBase.Remove(_currentBase.Length - 1) + MinChar;
                    _currentBase += MinChar.ToString();
                }
                else
                {
                    var newChar = _currentBase[_currentBase.Length - 1];
                    newChar++;
                    _currentBase = _currentBase.Remove(_currentBase.Length - 1) + newChar.ToString();
                }

                return NextId();
            }
        }
        else
        {
            _currentDigit = MinDigit;
            _currentBase += MinChar.ToString();
            return NextId();

        }
    }

    public static string NextId(string currentId)
    {
        if (string.IsNullOrWhiteSpace(currentId))
            return NextId();

        var charCount = currentId.Length;
        var indexFound = -1;
        for (int i = 0; i < charCount; i++)
        {
            if (!char.IsNumber(currentId[i]))
                continue;

            indexFound = i;
            break;
        }
        if (indexFound > -1)
        {
            _currentBase = currentId.Substring(0, indexFound);
            _currentDigit = int.Parse(currentId.Substring(indexFound)) + 1;
        }
        return NextId();
    }
}