using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace hhotLib.Common
{
    public static class BigNumberUtils
    {
        private static readonly int EXPONENT_DIFF_MAX = 2;

        /// <summary>
        /// Return formatted big number.
        /// </summary>
        /// <returns></returns>
        public static string GetFormattedNumber(BigNumber num, bool floorToInt)
        {
            string expUnit  = GetExponentUnit(num.Exponent);
            string mantissa = "";

            if (floorToInt || num.Exponent == 0)
                mantissa = num.Mantissa.ToString("##0");
            else
                mantissa = num.Mantissa.ToString("##0.00");

            return $"{mantissa}{expUnit}";
        }

        /// <summary>
        /// Convert exponent alphabet unit from int to string.
        /// </summary>
        public static string GetExponentUnit(int exp)
        {
            if (exp <= 0)
                return string.Empty;

            if (exp <= 26)
                return Convert.ToChar('a' + exp - 1).ToString();

            // exp > 26
            int firstUnit  = exp / 26;
            int secondUnit = exp % 26;
            return $"{Convert.ToChar('a' + firstUnit - 1).ToString()}{Convert.ToChar('a' + secondUnit - 1).ToString()}";
        }

        /// <summary>
        /// Convert exponent alphabet unit from string to int.
        /// </summary>
        public static int GetExponentUnit(string exp)
        {
            if (string.IsNullOrEmpty(exp) || exp.All(char.IsLetter) == false)
                return -1;

            char[] exps = exp.ToLower().ToCharArray();
            if (exps.Length > 2)  // Exclude exponent unit bigger than 'ZZ'.
            {
                Debug.LogWarning($"Exponent is too big({exps})!");
                return -1;
            }

            if (exps.Length == 1)
                return Convert.ToInt32(exps[0]) - 'a' + 1;

            // exp.Length == 2
            int firstUnit  = 26 * (Convert.ToInt32(exps[0] - 'a' + 1));
            int secondUnit = Convert.ToInt32(exps[1]) - 'a' + 1;
            return firstUnit + secondUnit;
        }

        /// <summary>
        /// Compare two BigNumbers.
        /// </summary>
        /// <returns>true if left is greater than or same as right, false otherwise.</returns>
        public static bool Compare(BigNumber left, BigNumber right)
        {
            if (left.Exponent < right.Exponent)
                return false;

            if (left.Exponent != right.Exponent)
                return true;

            return left.Mantissa >= right.Mantissa;
        }

        /// <summary>
        /// Compare two BigNumbers.
        /// </summary>
        /// <param name="isSameExponent">Check if both exponents are identical</param>
        /// <param name="isExpDiffAcceptable">Check if the difference of both exponents are acceptable</param>
        /// <returns>true if left is greater than or same as right, false otherwise.</returns>
        public static bool Compare(BigNumber left, BigNumber right, out bool isSameExponent, out bool isExpDiffAcceptable)
        {
            isExpDiffAcceptable = Mathf.Abs(left.Exponent - right.Exponent) <= EXPONENT_DIFF_MAX;
            isSameExponent      = left.Exponent == right.Exponent;
            return Compare(left, right);
        }

        /// <summary>
        /// Add two BigNumbers.
        /// </summary>
        public static BigNumber Add(BigNumber num0, BigNumber num1)
        {
            int expDiff     = num0.Exponent - num1.Exponent;
            int expDiff_Abs = Mathf.Abs(expDiff);
            if (expDiff_Abs > EXPONENT_DIFF_MAX)
            {
                Debug.LogWarning($"Exponent diff is greater than {EXPONENT_DIFF_MAX}! Return bigger one.");
                return expDiff > 0 ? num0 : num1;
            }

            double sum_mant = 0.0;
            int    sum_exp  = 0;

            if (expDiff == 0)
            {
                sum_mant = num0.Mantissa + num1.Mantissa;
                sum_exp  = num0.Exponent;
                return new BigNumber(sum_mant, sum_exp);
            }

            double multiplier = Math.Pow(1000.0, expDiff_Abs);

            if (expDiff > 0)
            {
                sum_mant = (num0.Mantissa * multiplier + num1.Mantissa) / multiplier;
                sum_exp  = num0.Exponent;
                return new BigNumber(sum_mant, sum_exp);
            }

            sum_mant = (num0.Mantissa + num1.Mantissa * multiplier) / multiplier;
            sum_exp = num1.Exponent;
            return new BigNumber(sum_mant, sum_exp);
        }

        /// <summary>
        /// Add multiple BigNumbers.
        /// </summary>
        public static BigNumber Add(List<BigNumber> nums)
        {
            int maxExp = int.MinValue;
            for (int i = 0; i < nums.Count; i++)
            {
                int temp = nums[i].Exponent;
                if (temp > maxExp)
                    maxExp = temp;
            }

            BigNumber sum = new BigNumber();
            for (int i = 0; i < nums.Count; i++)
            {
                if (nums[i].Exponent >= maxExp - EXPONENT_DIFF_MAX)
                    sum = Add(sum, nums[i]);
            }
            return sum;
        }

        /// <summary>
        /// Subtract latter from former.
        /// </summary>
        public static BigNumber Subtract(BigNumber num0, BigNumber num1)
        {
            int expDiff     = num0.Exponent - num1.Exponent;
            int expDiff_Abs = Mathf.Abs(expDiff);
            if (expDiff_Abs > EXPONENT_DIFF_MAX)
            {
            #if UNITY_EDITOR
                if (expDiff > 0) Debug.Log("Failed to subract: Exponent diff is greater than " + EXPONENT_DIFF_MAX + ". num0 is much greater than num1.");
                else             Debug.Log("Failed to subract: Exponent diff is greater than " + EXPONENT_DIFF_MAX + ". num1 is much greater than num0.");
            #endif
                return num0;
            }

            if (expDiff == 0)
            {
                double mantDiff = num0.Mantissa - num1.Mantissa;
                if (mantDiff > 0.0) return new BigNumber(mantDiff, num0.Exponent);
                if (mantDiff < 0.0) return num0;
                return BigNumber.IDENTITY;
            }

            if (expDiff > 0)
            {
                double multiplier = Math.Pow(1000.0, (double)expDiff_Abs);
                return new BigNumber((num0.Mantissa * multiplier - num1.Mantissa) / multiplier, num0.Exponent);
            }
            return num0;
        }

        /// <summary>
        /// Multiply num by multiplier.
        /// </summary>
        public static BigNumber Multiply(BigNumber num, double multiplier)
        {
            return new BigNumber(num.Mantissa * multiplier, num.Exponent);
        }

        /// <summary>
        /// Divide num by divider.
        /// </summary>
        public static BigNumber Divide(BigNumber num, double divisor)
        {
            double result = num.Mantissa / divisor;
            return result < 1.0
                    ? new BigNumber(result * 1000.0, num.Exponent - 1)
                    : new BigNumber(result         , num.Exponent);
        }

        /// <summary>
        /// Try to get proportion of two BigNumbers.
        /// </summary>
        public static bool TryGetProportion(BigNumber num0, BigNumber num1, out double proportion)
        {
            int expDiff = num0.Exponent - num1.Exponent;
            if (expDiff < 0)
            {
                if (expDiff == -1)
                {
                    proportion = num0.Mantissa / (num1.Mantissa * 1000.0);
                    return true;
                }
                Debug.Log("Failed to get proportion: Exponent of num0 is too small than that of num1!");
                proportion = 0.0;
                return false;
            }

            if (expDiff == 0)
            {
                double mantDiff = num0.Mantissa - num1.Mantissa;
                if (mantDiff < 0)
                {
                    proportion = num0.Mantissa / num1.Mantissa;
                    return true;
                }

                if (mantDiff > 0)
                {
                    proportion = 1.0;
                    return true;
                }

                // mantDiff == 0
                Debug.Log("Failed to get proportion: num0 is exactly same as num1.");
                proportion = 0.0;
                return false;
            }
            
            // expDiff > 0
            proportion = 1.0;
            return true;
        }
    }
}