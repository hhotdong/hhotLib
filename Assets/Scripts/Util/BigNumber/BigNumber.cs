using System;
using UnityEngine;

namespace hhotLib.Common
{
    [Serializable]
    public struct BigNumber
    {
        public static readonly BigNumber IDENTITY  = new BigNumber(0.0,     0);
        public static readonly BigNumber MAX_VALUE = new BigNumber(1.0, 10000);

        public double Mantissa => mantissa;
        public int    Exponent => exponent;
        public bool   HasValue => mantissa > 0.0 && exponent >= 0;

        [SerializeField] private double mantissa;
        [SerializeField] private int    exponent;

        public BigNumber(double mant, int exp)
        {
            if (exp < 0)  // exp가 0보다 작은 경우는 비정상적인 입력으로 판단해서 예외 처리함(mant가 1000.0 보다 큰 숫자일 때 exp가 음수여도 가능한 경우는 무시)
            {
                Debug.LogWarning("Exponent cannot be smaller than zero!");
                mantissa = 0.0;
                exponent = 0;
                return;
            }

            if (mant < 1.0 && exp == 0)
            {
                mantissa = 0.0;
                exponent = 0;
                return;
            }

            if (mant >= 1.0 && mant < 1000.0)
            {
                mantissa = mant;
                exponent = exp;
            }
            else
            {
                double tempExp = Math.Floor(Math.Log(mant, 1000.0));
                mantissa = mant * Math.Pow(1000.0, -tempExp);
                exponent = exp + (int)tempExp;
            }

            if (exponent < 0)
            {
                mantissa = 0.0;
                exponent = 0;
                return;
            }

            if (exponent == 0)   // mantissa가 정수가 아닌 유리수가 되지 않도록 예외 처리
                mantissa = Math.Floor(mantissa);
        }

        public override string ToString()
        {
            return $"{mantissa}_{exponent}";
        }
    }
}