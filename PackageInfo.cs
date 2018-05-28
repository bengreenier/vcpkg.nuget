using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VcpkgBuildTask
{
    public class PackageInfo
    {
        public static readonly char InfoSeparator = ':';
        public static readonly char TripletSeparator = '-';

        public string Name { get; private set; }
        public string Architecture { get; private set; }
        public string Platform { get; private set; }
        public string Triplet
        {
            get
            {
                if (string.IsNullOrEmpty(Platform) ||
                    string.IsNullOrEmpty(Architecture))
                {
                    return null;
                }
                else
                {
                    return Platform + TripletSeparator + Architecture;
                }
            }
        }

        public PackageInfo(string formattedString)
        {
            var parts = formattedString.Split(InfoSeparator);

            this.Name = parts[0];

            if (parts.Length == 2)
            {
                ParseAndSetTripletProps(parts[1]);
            }
            else if (parts.Length != 1)
            {
                throw new ArgumentException(nameof(formattedString));
            }
        }
        
        private void ParseAndSetTripletProps(string triplet)
        {
            var parts = triplet.Split(TripletSeparator);

            if (parts.Length != 2)
            {
                throw new ArgumentException(nameof(triplet));
            }

            this.Platform = parts[0];
            this.Architecture = parts[1];
        }
    }
}
