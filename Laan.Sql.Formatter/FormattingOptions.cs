using System;

namespace Laan.Sql.Formatter
{
    /// <summary>
    /// Configuration options for SQL formatting
    /// </summary>
    public class FormattingOptions
    {
        /// <summary>
        /// Number of spaces or tabs per indent level (default: 4)
        /// </summary>
        public int IndentSize { get; set; } = 4;

        /// <summary>
        /// Whether to use spaces (true) or tabs (false) for indentation (default: true)
        /// </summary>
        public bool UseSpaces { get; set; } = true;

        /// <summary>
        /// Maximum line length before wrapping (default: 80)
        /// </summary>
        public int MaxLineLength { get; set; } = 80;

        /// <summary>
        /// Casing style for SQL keywords (default: Upper)
        /// </summary>
        public KeywordCasing KeywordCasing { get; set; } = KeywordCasing.Upper;

        /// <summary>
        /// Spacing style for brackets/parentheses (default: NoSpaces)
        /// </summary>
        public BracketSpacing BracketSpacing { get; set; } = BracketSpacing.NoSpaces;

        /// <summary>
        /// Number of blank lines between major clauses (default: 1)
        /// </summary>
        public int BlankLinesBetweenClauses { get; set; } = 1;

        /// <summary>
        /// Maximum number of SELECT columns to display inline before wrapping (default: 1)
        /// </summary>
        public int MaxInlineSelectColumns { get; set; } = 1;

        /// <summary>
        /// Maximum number of INSERT columns to display inline before wrapping (default: 4)
        /// </summary>
        public int MaxInlineInsertColumns { get; set; } = 4;

        /// <summary>
        /// Creates a new instance with default values
        /// </summary>
        public FormattingOptions()
        {
        }

        /// <summary>
        /// Creates a copy of this options instance
        /// </summary>
        public FormattingOptions Clone()
        {
            return new FormattingOptions
            {
                IndentSize = this.IndentSize,
                UseSpaces = this.UseSpaces,
                MaxLineLength = this.MaxLineLength,
                KeywordCasing = this.KeywordCasing,
                BracketSpacing = this.BracketSpacing,
                BlankLinesBetweenClauses = this.BlankLinesBetweenClauses,
                MaxInlineSelectColumns = this.MaxInlineSelectColumns,
                MaxInlineInsertColumns = this.MaxInlineInsertColumns
            };
        }

        /// <summary>
        /// Validates the options and throws if any are invalid
        /// </summary>
        public void Validate()
        {
            if (IndentSize < 0 || IndentSize > 16)
                throw new ArgumentOutOfRangeException(nameof(IndentSize), "IndentSize must be between 0 and 16");

            if (MaxLineLength < 20 || MaxLineLength > 1000)
                throw new ArgumentOutOfRangeException(nameof(MaxLineLength), "MaxLineLength must be between 20 and 1000");

            if (BlankLinesBetweenClauses < 0 || BlankLinesBetweenClauses > 5)
                throw new ArgumentOutOfRangeException(nameof(BlankLinesBetweenClauses), "BlankLinesBetweenClauses must be between 0 and 5");

            if (MaxInlineSelectColumns < 0)
                throw new ArgumentOutOfRangeException(nameof(MaxInlineSelectColumns), "MaxInlineSelectColumns must be non-negative");

            if (MaxInlineInsertColumns < 0)
                throw new ArgumentOutOfRangeException(nameof(MaxInlineInsertColumns), "MaxInlineInsertColumns must be non-negative");
        }
    }
}
