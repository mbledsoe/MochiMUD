namespace MochiMud.WebApp.Commands
{
    public static class CommandTextParser
    {
        public static string? GetCommandName(string command)
        {
            var trimmedCommand = command.AsSpan().Trim();

            if (trimmedCommand.IsEmpty)
            {
                return null;
            }

            for (var index = 0; index < trimmedCommand.Length; index++)
            {
                if (char.IsWhiteSpace(trimmedCommand[index]))
                {
                    return trimmedCommand[..index].ToString();
                }
            }

            return trimmedCommand.ToString();
        }

        public static string? GetArguments(string command)
        {
            var trimmedCommand = command.AsSpan().Trim();

            if (trimmedCommand.IsEmpty)
            {
                return null;
            }

            for (var index = 0; index < trimmedCommand.Length; index++)
            {
                if (char.IsWhiteSpace(trimmedCommand[index]))
                {
                    var arguments = trimmedCommand[index..].Trim();

                    return arguments.IsEmpty
                        ? null
                        : arguments.ToString();
                }
            }

            return null;
        }

        public static TEnum? GetEnumArgument<TEnum>(string command)
            where TEnum : struct
        {
            var arguments = GetArguments(command);

            if (arguments is null)
            {
                return null;
            }

            return Enum.TryParse<TEnum>(arguments, true, out var value)
                ? value
                : null;
        }
    }
}
