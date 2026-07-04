using System;

namespace Flower.Backend.Utils
{
    public static class PriceHelper
    {
        public static decimal RoundPrice(decimal price)
        {
            return Math.Round(price / 1000) * 1000;
        }
    }
}
