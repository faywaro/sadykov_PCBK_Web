namespace sadykov_PCBK_Web.Helpers
{
    public static class DiscountCalculator
    {
        private const int ThresholdLevel1 = 10_000;
        private const int ThresholdLevel2 = 50_000;
        private const int ThresholdLevel3 = 300_000;

        private const int DiscountLevel0 = 0;
        private const int DiscountLevel1 = 5;
        private const int DiscountLevel2 = 10;
        private const int DiscountLevel3 = 15;

        public static int CalculateDiscount(int totalQuantity)
        {
            if (totalQuantity >= ThresholdLevel3) return DiscountLevel3;
            if (totalQuantity >= ThresholdLevel2) return DiscountLevel2;
            if (totalQuantity >= ThresholdLevel1) return DiscountLevel1;
            return DiscountLevel0;
        }
    }
}
