using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordFinder
{
    public enum WordDirection
    {
        Vertical = 1,
        Horizontal = 2,
    }

    public class WordSearchData
    {
        public string Word { get; set; }

        public WordDirection Direction { get; set; }

        public Queue<char> RemainingChars;

        // y first just to keep it consistent with the search that starts at y
        public (int y , int x) NextPosition { get; set; }
    }


    /*
     * Complexity is kinda log(h*w*(log(n) + n)), let's just call it log(n)
     * h -> height, w -> width, n -> number of words in word stream
     */
    public class WordFinder
    {
        private readonly string[] matrix;

        public WordFinder(IEnumerable<string> matrix)
        {
            this.matrix = matrix.ToArray();

            if (this.matrix.GroupBy(r => r.Length).Count() > 1)
            {
                throw new ApplicationException("All rows in matrix should be the same length.");
            }
        }

        /// <summary>
        /// Returns the top 10 most repeated words found in the object's matrix.
        /// </summary>
        /// <param name="wordstream">Words to look for inside of the matrix.</param>
        /// <returns>List of top 10 most repeated words </returns>
        public IEnumerable<string> Find(IEnumerable<string> wordstream)
        {

            var foundWords = new List<string>();
            var words = wordstream.ToArray();

            if (!matrix.Any() || !words.Any())
            {
                // return an empty list if the matrix or stream are empty
                return foundWords;
            }

            var minWordLength = words.Min(w => w.Length);
            var currentSearches = new List<WordSearchData>();
            var matrixWidth = matrix[0].Length;

            if (minWordLength > matrixWidth)
            {
                // return an empty list if the shortest word is larger than the matrix
                return foundWords;
            }

            for (var y = 0; y < matrix.Length; y++)
            {
                for (var x = 0; x < matrix[y].Length; x++)
                {
                    if (!currentSearches.Any() && matrix[y].Length - x - minWordLength < 0)
                    {
                        break;
                    }

                    // 1.- remove words from current searches if the next character doesn't match the current one, OR if the word is fully matched
                    // if the word is fully matched it will be added to the foundWords
                    currentSearches = currentSearches.Where(s =>
                    {
                        if (s.NextPosition != (y, x))
                        {
                            return true;
                        }

                        var charMatches = s.RemainingChars.Dequeue() == matrix[y][x];
                        s.NextPosition = this.GetNextPosition((y, x), s.Direction);
                        if (charMatches && s.RemainingChars.Count == 0)
                        {
                            foundWords.Add(s.Word);
                            return false;
                        }

                        return charMatches;
                    }).ToList();

                    // 2.- get words that start with the current char and that have enough space to fit 
                    // add them to currentSearches
                    var firstMatchWords =
                        words
                        .Where(w => w[0] == matrix[y][x] && (matrix.Length - y - w.Length >= 0 || matrixWidth - x - w.Length >= 0))
                        .SelectMany(w =>
                        {
                            var result = new List<WordSearchData>();

                            if (matrix.Length - y - w.Length >= 0)
                            {
                                var remainingChars = new Queue<char>(w.ToCharArray());
                                remainingChars.Dequeue();
                                
                                var wordSearchData = new WordSearchData
                                {
                                    Direction = WordDirection.Vertical,
                                    RemainingChars = remainingChars,
                                    Word = w,
                                    NextPosition = this.GetNextPosition((y, x), WordDirection.Vertical),
                                };

                                result.Add(wordSearchData);
                            }
                            
                            if (matrixWidth - x - w.Length >= 0)
                            {
                                var remainingChars = new Queue<char>(w.ToCharArray());
                                remainingChars.Dequeue();

                                var wordSearchData = new WordSearchData
                                {
                                    Direction = WordDirection.Horizontal,
                                    RemainingChars = remainingChars,
                                    Word = w,
                                    NextPosition = this.GetNextPosition((y, x), WordDirection.Horizontal),
                                };

                                result.Add(wordSearchData);
                            }


                            return result;
                        }).ToArray();

                    currentSearches.AddRange(firstMatchWords);
                }
            }

            var result = foundWords.GroupBy(w => w).OrderByDescending(g => g.Count()).Select(g => g.Key).Take(10).ToArray();
            return result;
        }

        private (int y, int x) GetNextPosition((int y, int x) currentPosition, WordDirection direction)
        {
            if (direction == WordDirection.Horizontal)
            {
                return (currentPosition.y, currentPosition.x + 1);
            }
            else
            {
                return (currentPosition.y + 1, currentPosition.x);
            }
        }
    }
}
