using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class HandRankerTests
{
    [TestFixture]
    public class NoneTests
    {
        [Test]
        public void TestBestNone()
        {
            List<Card> emptyHand = new();
            HandRankerResult result = HandRanker.RankHand(emptyHand);
            Assert.IsTrue(result.BestHand == HandType.None);
        }
    }
}
