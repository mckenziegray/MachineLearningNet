using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetExtensions
{
    public enum NameDisplayType
    {
        Default,
        FullCapitalized,    // John
        FullUpper,          // JOHN
        FullLower,          // john
        InitialUpper,       // J
        InitialLower,       // j
        InitialDotUpper,    // J.
        InitialDotLower,    // j.
        None
    }

    public enum NameOrder
    {
        Western,            // John Doe
        Eastern,            // Doe John
        WesternReversed    // Doe, John
    }

    public record NameFormat
    {
        public NameOrder NameOrder { get; init; } = NameOrder.Western;
        public NameDisplayType TitleDisplayType { get; init; } = NameDisplayType.Default;
        public NameDisplayType ForenameDisplayType { get; init; } = NameDisplayType.Default;
        public NameDisplayType MiddleNameDisplayType { get; init; } = NameDisplayType.Default;
        public NameDisplayType SurnameDisplayType { get; init; } = NameDisplayType.Default;
        public NameDisplayType SuffixDisplayType { get; init; } = NameDisplayType.Default;
        public string SuffixSeparator { get; init; } = " ";
    }
}
