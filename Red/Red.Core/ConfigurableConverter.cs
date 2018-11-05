using System;
using System.Collections.Generic;

namespace Red.Core
{
    /// <summary>
    /// This is not yet fleshed out _at_all_
    /// </summary>
    public class ConfigurableConverter
    {
        private class Conversion
        {
            public Conversion(Type input, Type output)
            {
                Input = input ?? throw new ArgumentNullException(nameof(input));
                Output = output ?? throw new ArgumentNullException(nameof(output));
            }

            private Type Input { get; set; }
            private Type Output { get; set; }
        }
        
        private List<Conversion> Conversions { get; set; } = new List<Conversion>();

        public void Register<TInput, TOutput>(Func<TInput, TOutput> conversionFunction)
        {
            var inputType = typeof(TInput);
            var outputType = typeof(TOutput);

            Conversions.Add(new Conversion(inputType, outputType));
        }

        public TOutput To<TOutput>(object input)
        {
            var inputType = input.GetType();
            var outputType = typeof(TOutput);

            if (Conversions.Contains(new Conversion(inputType, outputType)))
            {
            }

            return default(TOutput);
        }
    }
}