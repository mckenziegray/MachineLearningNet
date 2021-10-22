using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetExtensions
{
    public class Name
    {
        /// <summary>
        /// Title, such as: Sir, Ms., Dr., or Queen.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// All given names, including first and last name(s).
        /// </summary>
        public IList<string> GivenNames { get; protected set; } = new List<string>();

        /// <summary>
        /// First given name.
        /// </summary>
        public string Forename => GivenNames.FirstOrDefault();

        /// <summary>
        /// All given names other than the forename.
        /// </summary>
        public IEnumerable<string> MiddleNames => GivenNames.Count > 1 ? GivenNames.Skip(1) : new List<string>();

        /// <summary>
        /// Family name (last name).
        /// </summary>
        public string Surname { get; protected set; }

        /// <summary>
        /// Suffix, suck as: Jr., III, Esq., or The Great.
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// The name format to be used by default when printing
        /// </summary>
        public NameFormat DefaultNameFormat { get; set; }

        #region Constructors
        public Name() 
        {
            Initialize();
        }

        public Name(string firstName, string middleName = null, string lastName = null, string title = null, string suffix = null, NameFormat defaultNameFormat = null)
        {
            Initialize(new string[] { firstName, middleName }, lastName, title, suffix, defaultNameFormat);
        }

        public Name(IList<string> givenNames, string surname = null, string title = null, string suffix = null, NameFormat defaultNameFormat = null)
        {
            Initialize(givenNames, surname, title, suffix, defaultNameFormat);
        }
        #endregion

        #region ToString methods
        public override string ToString()
        {
            return ToString(DefaultNameFormat);
        }

        public string ToString(NameFormat format)
        {
            string titleFormatted = Title?.ToString(format.TitleDisplayType) ?? string.Empty;
            string givenNamesFormatted = $"{Forename?.ToString(format.ForenameDisplayType)} {string.Join(' ', MiddleNames.Select(n => n?.ToString(format.MiddleNameDisplayType)))}".Trim();
            string surnameFormatted = Surname?.ToString(format.SurnameDisplayType) ?? string.Empty;
            string suffixFormatted = Suffix?.ToString(format.SuffixDisplayType) ?? string.Empty;

            StringBuilder fullName = new StringBuilder(titleFormatted);

            switch (format.NameOrder)
            {
                case NameOrder.Eastern:
                    if (fullName.Length > 0 && !string.IsNullOrWhiteSpace(surnameFormatted))
                        fullName.Append(' '); 
                    fullName.Append(surnameFormatted);
                    if (fullName.Length > 0 && !string.IsNullOrWhiteSpace(givenNamesFormatted))
                        fullName.Append(' ');
                    fullName.Append(givenNamesFormatted);
                    break;
                case NameOrder.WesternReversed:
                    if (fullName.Length > 0 && !string.IsNullOrWhiteSpace(surnameFormatted))
                        fullName.Append(' ');
                    fullName.Append(surnameFormatted);
                    if (fullName.Length > 0 && !string.IsNullOrWhiteSpace(givenNamesFormatted))
                        fullName.Append(", ");
                     fullName.Append(givenNamesFormatted);
                    break;
                case NameOrder.Western:
                default:
                    if (fullName.Length > 0 && !string.IsNullOrWhiteSpace(givenNamesFormatted))
                        fullName.Append(' ');
                    fullName.Append(givenNamesFormatted);
                    if (fullName.Length > 0 && !string.IsNullOrWhiteSpace(surnameFormatted))
                        fullName.Append(' ');
                    fullName.Append(surnameFormatted);
                    break;
            }

            if (fullName.Length > 0 && !string.IsNullOrWhiteSpace(suffixFormatted))
                fullName.Append(' '); 
            fullName.Append(suffixFormatted);

            return fullName.ToString();
        }
        #endregion

        protected void Initialize(IList<string> givenNames = null, string surname = null, string title = null, string suffix = null, NameFormat defaultNameFormat = null)
        {
            Title = title?.Trim();
            if (givenNames is not null)
                GivenNames = givenNames.Where(n => n is not null).Select(n => n.Trim()).ToList();
            Surname = surname?.Trim();
            Suffix = suffix?.Trim();
            DefaultNameFormat = defaultNameFormat ?? new NameFormat();
        }
    }
}
