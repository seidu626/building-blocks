using System.Diagnostics;

namespace BuildingBlocks.Exceptions
{
    public static class ExceptionExtensions
    {
        public static Stacky PrettifyStackTrace(this Exception ex)
        {
            var stacky = new Stacky
            {
                ExceptionMessage = ex.Message,
                ExceptionType = ex.GetType().ToString()
            };
            ParseStackTrace(ex, stacky);
            return stacky;
        }

        public static Stacky PrettifyStackTraceWithParameters(this Exception ex, params object[] args)
        {
            var stackTrace = new StackTrace(ex, false);
            var parameters = stackTrace.GetFrame(0)?.GetMethod()?.GetParameters();

            var stacky = new Stacky
            {
                ExceptionMessage = ex.Message,
                ExceptionType = ex.GetType().ToString(),
                MethodArguments = parameters != null
                    ? parameters.Select(p => p.Name).Zip(args, (name, value) => new { name, value })
                                .ToDictionary(x => x.name, x => x.value)
                    : new Dictionary<string, object>()
            };

            ParseStackTrace(ex, stacky);
            return stacky;
        }

        private static void ParseStackTrace(Exception ex, Stacky stacky)
        {
            if (string.IsNullOrEmpty(ex.StackTrace))
                return;

            var stackTraceLines = ex.StackTrace.Split(Constants.Stacky.NewLineArray, StringSplitOptions.RemoveEmptyEntries);
            int subStackCount = 0;

            for (int index = 0; index < stackTraceLines.Length; index++)
            {
                var line = stackTraceLines[index];

                if (index == 0)
                {
                    var methodInfo = line.Split(Constants.Stacky.InArray, StringSplitOptions.RemoveEmptyEntries);
                    stacky.Method = methodInfo[0].Split(Constants.Stacky.AtArray, StringSplitOptions.RemoveEmptyEntries)[0];

                    var locationInfo = methodInfo.Length > 1
                        ? methodInfo[1].Split(Constants.Stacky.LineArray, StringSplitOptions.RemoveEmptyEntries)
                        : new[] { methodInfo[0], string.Empty };

                    stacky.FileName = locationInfo[0].Contains(".cs") ? locationInfo[0] : "SystemException";
                    stacky.Line = int.TryParse(locationInfo.ElementAtOrDefault(1), out int lineNumber) ? lineNumber : 0;

                    stacky.StackLines.Add($"{index}: {stacky.Method}");
                }
                else if (line.StartsWith("---"))
                {
                    subStackCount++;
                    stacky.StackLines.Add($"=== Sub-stack {subStackCount} ===");
                }
                else
                {
                    stacky.StackLines.Add($"{index}: @ {line.Replace("   at ", "")}");
                }
            }
        }
    }
}
