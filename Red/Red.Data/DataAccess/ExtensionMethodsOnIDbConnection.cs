using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Red.Data.DataAccess
{
    /// <summary>
    /// Syntax sugar for IDbConnection
    /// </summary>
    public static class ExtensionMethodsOnIDbConnection
    {
        /// <summary>
        /// Standard function you can assign to the ParameterFormattingFunc
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string PrefixWithAtSymbol(string name) => $"@{name}";
        /// <summary>
        /// Standard function you can assign to the ParameterFormattingFunc
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string PrefixWithAtColon(string name) => $":{name}";
        /// <summary>
        /// Allows external influence on the way a token is transformed to a parameter
        /// </summary>
        public static Func<string, string> ParameterFormattingFunc { get; set; } = PrefixWithAtSymbol;

        /// <summary>
        /// Formats a token to a parameter using the ParameterFormattingFunc
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string FormatParameterName(string name) => ParameterFormattingFunc(name);

        /// <summary>
        /// Represents some syntactical sugar over tuple of string (commandText) and a string mapped Dictionary of objects (arguments)
        /// </summary>
        private class CommandTextWithParameters
        {
            /// <summary>
            /// Analogue to IDbCommand.CommandText
            /// </summary>
            public string CommandText { get; set; }
            /// <summary>
            /// Analogue to IDbCommand.Parameters
            /// </summary>
            public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();
        }
        /// <summary>
        /// Expands a text with positional arguments to a text with named arguments
        /// </summary>
        /// <param name="text"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private static CommandTextWithParameters ExpandPositionalParameters(string text, object[] arguments)
        {
            const char ESCAPE_CHAR = '\\';

            var result = new CommandTextWithParameters();
            var sb = new StringBuilder();

            var previous = default(char);
            var numberOfPlaceholdersSeen = 0;
            var parameterMap = new Dictionary<object,string>();

            for (var index = 0; index < text.Length; index++)
            {
                var current = text[index];
                switch (current)
                {
                    case '\\':
                        index++; // skip the escape
                        sb.Append(text[index]);
                        //index++; // skip the character we just escaped
                        continue;

                    case '?':
                        if (previous != ESCAPE_CHAR)
                        {
                            var value = arguments[numberOfPlaceholdersSeen];
                            var parameterName = "";

                            if (parameterMap.ContainsKey(value)) // if we've already seen this value...
                                parameterName = parameterMap[value]; // reuse its' name instead of binding it again under a new name
                            else
                            {
                                parameterName = FormatParameterName($"p{numberOfPlaceholdersSeen}");
                                parameterMap[value] = parameterName;
                                result.Parameters.Add(parameterName, value);
                            }

                            sb.Append(parameterName);
                            numberOfPlaceholdersSeen++;
                        }
                        else
                            sb.Append(current);
                        break;

                    default:
                        sb.Append(current);
                        break;
                }
                previous = current;
            }

            if (numberOfPlaceholdersSeen != arguments.Length)
                throw new Exception("Number of supplied arguments does not match number of placeholders.");

            result.CommandText = sb.ToString();
            return result;
        }

        /// <summary>
        /// Create a command with the given commandText and populate its' parameters with the named arguments provided
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static IDbCommand CreateCommand(this IDbConnection connection, string commandText, Dictionary<string, object> arguments)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            foreach (var item in arguments)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = item.Key;
                parameter.Value = item.Value;
                command.Parameters.Add(parameter);
            }

            return command;
        }

        /// <summary>
        /// Creates a command with the given positional arguments expanded to named parameters
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static IDbCommand CreateCommand(this IDbConnection connection, string commandText, params object[] arguments)
        {
            var intermediate = ExpandPositionalParameters(commandText, arguments);
            return connection.CreateCommand(intermediate.CommandText, intermediate.Parameters);
        }
    }
}