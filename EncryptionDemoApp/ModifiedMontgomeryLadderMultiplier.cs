namespace Org.BouncyCastle.Math.EC.Multiplier
{
    public class ModifiedMontgomeryLadderMultiplier : AbstractECMultiplier
    {
        protected override ECPoint MultiplyPositive(ECPoint p, BigInteger k)
        {
            var array = new ECPoint[2]
            {
                p.Curve.Infinity,
                p
            };
            int bitLength = k.BitLength;
            int num = bitLength;
            while (--num >= 0)
            {
                int num2 = (k.TestBit(num) ? 1 : 0);
                int num3 = 1 - num2;
                array[num3] = array[num3].Add(array[num2]);
                array[num2] = array[num2].Twice();
            }

            return array[0];
        }
    }
}