using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace WordFinder.Test
{
    [TestClass]
    public class WordFinderTests
    {
        [TestMethod]
        public void InvalidMatrixThrowsException()
        {
            Assert.ThrowsException<ApplicationException>(() =>
                new WordFinder(new[]
                    {
                        "abc",
                        "defg"
                    }));
        }

        [TestMethod]
        public void ValidMatrixDoesNotThrowException()
        {
            _ = new WordFinder(new[] 
                { "abdc",
                  "defg",
                });
        }

        [TestMethod]
        public void NoWordsOnEmptyMatrix()
        {
            var wordFinder = new WordFinder(Array.Empty<string>());
            var results = wordFinder.Find(new[] { "word" });

            Assert.AreEqual(0 ,results.Count());
        }

        [TestMethod]
        public void NoWordsOnEmptyStream()
        {
            var wordFinder = new WordFinder(new[] { "word" });
            var results = wordFinder.Find(Array.Empty<string>());

            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public void FindOneHorizontalWord()
        {
            var wordFinder = new WordFinder(new[] {
                "ssfg",
                "word" 
            });
            var results = wordFinder.Find(new[] { "word" });

            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public void FindOneVerticalWord()
        {
            var wordFinder = new WordFinder(new[] {
                "wsfg",
                "oord",
                "rasd",
                "dsdf",
            });
            var results = wordFinder.Find(new[] { "word" });

            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public void FindVerticalAndHorizontal()
        {
            var wordFinder = new WordFinder(new[] {
                "word",
                "oord",
                "rasd",
                "dsdf",
            });
            var results = wordFinder.Find(new[] { "word" });

            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public void FindTwoWords()
        {
            var wordFinder = new WordFinder(new[] {
                "waspa",
                "oordb",
                "rasdc",
                "dsdfd",
            });
            var results = wordFinder.Find(new[] { "word", "wasp" });

            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public void Find3OutOf4Words()
        {
            var wordFinder = new WordFinder(new[] {
                "waspa",
                "oohdb",
                "raodc",
                "dstfd",
            });
            var results = wordFinder.Find(new[] { "word", "wasp", "hot", "world", });

            Assert.AreEqual(3, results.Count());

            Assert.IsTrue(results.Contains("word"));
            Assert.IsTrue(results.Contains("wasp"));
            Assert.IsTrue(results.Contains("hot"));
        }

        [TestMethod]
        public void TestCorrectOrder()
        {
            var wordFinder = new WordFinder(new[] {
                "waspaw",
                "oohota",
                "rhotcs",
                "dstfdp",
            });
            var results = wordFinder.Find(new[] { "word", "wasp", "hot", "world", }).ToArray();

            Assert.AreEqual(3, results.Count());

            Assert.AreEqual("hot", results[0]);
            Assert.AreEqual("wasp", results[1]);
            Assert.AreEqual("word", results[2]);
        }

        [TestMethod]
        public void TestMoreThan10Words()
        {
            var wordFinder = new WordFinder(new[] {
                "waspawsw",
                "oohotaeo",
                "rhotcsar",
                "dstfdpvl",
                "dstfdpvd",
                "dogcatab",
                "dogninea",
                "forlovea",
                "happyvea",
            });
            var results = wordFinder.Find(new[] { "word", "wasp", "hot", "world", "dog", "cat", "for", "nine", "love", "happy", "sea"}).ToArray();

            Assert.AreEqual(10, results.Count());
        }
    }
}
