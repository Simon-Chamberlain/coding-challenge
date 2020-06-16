using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly List<Shirt> _shirts;

        public SearchEngine(List<Shirt> shirts)
        {
            _shirts = shirts;

            // TODO: data preparation and initialisation of additional data structures to improve performance goes here.

        }


        public SearchResults Search(SearchOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (options.Colors == null && options.Sizes == null) throw new ArgumentNullException(nameof(options));

            var results = _shirts.Where(s => (!options.Colors.Any() || options.Colors.Any(x => x == s.Color)
                && (!options.Sizes.Any() || options.Sizes.Any(x => x == s.Size)))).ToList();




            var byColour = results.GroupBy(x => x.Color)
                .Select(x => new ColorCount
                {
                    Color = x.Key,
                    Count = x.Count()
                }).ToList();

            var bySize = results.GroupBy(x => x.Size)
                .Select(x => new SizeCount
                {
                    Size = x.Key,
                    Count = x.Count()
                }).ToList();

            var missingColours = Color.All.Where(c => byColour.All(x => x.Color != c))
                .Select(x => new ColorCount {Color = x, Count = 0});

            var missingSizes = Size.All.Where(c => bySize.All(x => x.Size != c))
                .Select(x => new SizeCount { Size = x, Count = 0 });

            return new SearchResults
            {
                ColorCounts = byColour.Union(missingColours).ToList(),
                Shirts = results,
                SizeCounts = bySize.Union(missingSizes).ToList()
            };
        }
    }
}