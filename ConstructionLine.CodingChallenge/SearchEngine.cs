using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly List<Shirt> _shirts;

        public SearchEngine(List<Shirt> shirts)
        {
            _shirts = shirts ?? throw new ArgumentNullException(nameof(shirts));
        }

        /// <summary>
        /// Search for shirts by colours and sizes
        /// N.B. Performant for datasets <=50000; therefore no optimisations performed
        /// </summary>
        /// <param name="options">Search criteria</param>
        /// <returns>Search results</returns>
        public SearchResults Search(SearchOptions options)
        {
            if (options?.Colors == null || options.Sizes == null) throw new ArgumentNullException(nameof(options));

            // potential for parallel processing
            var results = _shirts.Where(s => (!options.Colors.Any() || options.Colors.Any(x => x == s.Color))
                && (!options.Sizes.Any() || options.Sizes.Any(x => x == s.Size))).ToList();

            var colorCountsForMatches = GetColorCountsForMatches(results).ToList();

            var sizeCountsForMatches = GetSizeCountsForMatches(results).ToList();

            var unmatchedColorCounts = GetUnmatchedColorCounts(colorCountsForMatches);

            var unmatchedSizeCounts = GetUnmatchedSizeCounts(sizeCountsForMatches);

            return new SearchResults
            {
                ColorCounts = colorCountsForMatches.Union(unmatchedColorCounts).ToList(),
                Shirts = results,
                SizeCounts = sizeCountsForMatches.Union(unmatchedSizeCounts).ToList()
            };
        }

        private static IEnumerable<SizeCount> GetUnmatchedSizeCounts(IEnumerable<SizeCount> sizeCountsForMatches) =>
            Size.All.Where(s => sizeCountsForMatches.All(x => x.Size != s))
                .Select(x => new SizeCount {Size = x, Count = 0});

        private static IEnumerable<ColorCount> GetUnmatchedColorCounts(IEnumerable<ColorCount> colorCountsForMatches) =>
            Color.All.Where(c => colorCountsForMatches.All(x => x.Color != c))
                .Select(x => new ColorCount {Color = x, Count = 0});

        private static IEnumerable<SizeCount> GetSizeCountsForMatches(IEnumerable<Shirt> results) =>
            results.GroupBy(x => x.Size)
                .Select(x => new SizeCount
                {
                    Size = x.Key,
                    Count = x.Count()
                });

        private static IEnumerable<ColorCount> GetColorCountsForMatches(IEnumerable<Shirt> results) =>
            results.GroupBy(x => x.Color)
                .Select(x => new ColorCount
                {
                    Color = x.Key,
                    Count = x.Count()
                });
    }
}