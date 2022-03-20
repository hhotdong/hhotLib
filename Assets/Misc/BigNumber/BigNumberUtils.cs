using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BigNumberUtils
{
    private static readonly int EXPONENT_DIFF_MAX = 2;

    /// <summary>
    /// Convert exponent value(int) to exponent unit(string).
    /// </summary>
    public static string GetExponentUnit(int exp)
    {
        if (exp <= 0)
            return string.Empty;
        else if (exp <= 26)
            return Convert.ToChar('a' + exp - 1).ToString();
        else  // exp > 26
        {
            int firstUnit = exp / 26;
            int secondUnit = exp % 26;
            return $"{Convert.ToChar('a' + firstUnit - 1).ToString()}{Convert.ToChar('a' + secondUnit - 1).ToString()}";
        }
    }

    /// <summary>
    /// Convert exponent value(string) to exponent unit(string).
    /// </summary>
    public static int GetExponentUnit(string exp)
    {
        if (exp.Length < 1 || !exp.All(char.IsLetter))
            return -1;

        char[] exps = exp.ToLower().ToCharArray();
        if (exps.Length > 2)
        {
            Debug.Log($"Exponent is too big({exps})!");
            return -1;
        }

        if (exps.Length == 1)
            return Convert.ToInt32(exps[0]) - 'a' + 1;
        else  // exp.Length == 2
        {
            int firstUnit = 26 * (Convert.ToInt32(exps[0] - 'a' + 1));
            int secondUnit = Convert.ToInt32(exps[1]) - 'a' + 1;
            return firstUnit + secondUnit;
        }
    }

    /// <summary>
    /// Compare two BigNumbers.
    /// </summary>
    /// <returns>true if former is greater than or same as latter, false otherwise.</returns>
    public static bool Compare(BigNumber former, BigNumber latter)
    {
        if (former.Exponent < latter.Exponent)
            return false;
        else if (former.Exponent == latter.Exponent)
        {
            if (former.Mantissa >= latter.Mantissa)
                return true;
            else
                return false;
        }
        else
            return true;
    }

    /// <summary>
    /// Compare two BigNumbers.
    /// </summary>
    /// <param name="isSameExponent">Check if both exponents are identical</param>
    /// <param name="isExpDiffAcceptable">Check if the difference of both exponents are acceptable</param>
    /// <returns>true if former is greater than or same as latter, false otherwise.</returns>
    public static bool Compare(BigNumber former, BigNumber latter, out bool isSameExponent, out bool isExpDiffAcceptable)
    {
        bool formerIsBigger = true;
        bool sameExponent = false;
        if (former.Exponent < latter.Exponent)
            formerIsBigger = false;
        else if (former.Exponent == latter.Exponent)
        {
            sameExponent = true;
            if (former.Mantissa >= latter.Mantissa)
                formerIsBigger = true;
            else
                formerIsBigger = false;
        }

        if (Mathf.Abs(former.Exponent - latter.Exponent) >= EXPONENT_DIFF_MAX)
            isExpDiffAcceptable = true;
        else
            isExpDiffAcceptable = false;

        isSameExponent = sameExponent;
        return formerIsBigger;
    }

    /// <summary>
    /// Add two BigNumbers.
    /// </summary>
    public static BigNumber Add(BigNumber former, BigNumber latter)
    {
        int expDiff = former.Exponent - latter.Exponent;
        int expDiff_Abs = Mathf.Abs(expDiff);
        if (expDiff_Abs > EXPONENT_DIFF_MAX)
        {
            Debug.Log($"Exponent diff is greater than {EXPONENT_DIFF_MAX}! Return bigger one.");
            return expDiff > 0 ? former : latter;
        }

        double sum_mant = 0.0;
        int sum_exp = 0;
        if (expDiff == 0)
        {
            sum_mant = former.Mantissa + latter.Mantissa;
            sum_exp = former.Exponent;
        }
        else
        {
            double multiplier = Math.Pow(1000.0, expDiff_Abs);
            if (expDiff > 0)
            {
                sum_mant = (former.Mantissa * multiplier + latter.Mantissa) / multiplier;
                sum_exp = former.Exponent;
            }
            else
            {
                sum_mant = (former.Mantissa + latter.Mantissa * multiplier) / multiplier;
                sum_exp = latter.Exponent;
            }
        }

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
    public static BigNumber Subtract(BigNumber former, BigNumber latter)
    {
        int expDiff = former.Exponent - latter.Exponent;
        int expDiff_Abs = Mathf.Abs(expDiff);
        if (expDiff_Abs > EXPONENT_DIFF_MAX)
        {
#if UNITY_EDITOR
            if (expDiff > 0)
                Debug.Log("Failed to subract : Exponent diff is greater than " + EXPONENT_DIFF_MAX + ". Former is much greater than latter.");
            else
                Debug.Log("Failed to subract : Exponent diff is greater than " + EXPONENT_DIFF_MAX + ". Latter is much greater than former.");
#endif
            return former;
        }

        if (expDiff == 0)
        {
            double mantDiff = former.Mantissa - latter.Mantissa;
            if (mantDiff > 0.0)
            {
                return new BigNumber(mantDiff, former.Exponent);
            }
            else if (mantDiff < 0.0)
            {
                Debug.Log("Failed to subtract : Mantissa of former is less than that of latter. Former is returned.");
                return former;
            }
            else  // mantDiff == 0
            {
                Debug.Log("Former is exactly same as latter. Return BigNumber.Identity.");
                return BigNumber.Identity;
            }
        }
        else if (expDiff > 0)
        {
            double multiplier = Math.Pow(1000.0, (double)expDiff_Abs);
            return new BigNumber((former.Mantissa * multiplier - latter.Mantissa) / multiplier, former.Exponent);
        }
        else  // expDiff < 0
        {
            Debug.Log("Failed to subtract : Exponent of former is less than that of latter. Former is returned.");
            return former;
        }
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
    public static BigNumber Divide(BigNumber num, double divider)
    {
        double divideResult = num.Mantissa / divider;
        return divideResult < 1.0
                ? new BigNumber(divideResult * 1000.0, num.Exponent - 1)
                : new BigNumber(divideResult, num.Exponent);
    }

    /// <summary>
    /// Try to get proportion of two BigNumbers.
    /// </summary>
    public static bool TryGetProportion(BigNumber former, BigNumber latter, out double proportion)
    {
        int expDiff = former.Exponent - latter.Exponent;
        if (expDiff < 0)
        {
            if (expDiff == -1)
            {
                proportion = former.Mantissa / (latter.Mantissa * 1000.0);
                return true;
            }
            else  // expDiff < -1
            {
                Debug.Log("Failed to get proportion : Exponent of former is too small than that of latter!");
                proportion = 0.0;
                return false;
            }
        }
        else if (expDiff == 0)
        {
            double mantDiff = former.Mantissa - latter.Mantissa;
            if (mantDiff < 0)
            {
                proportion = former.Mantissa / latter.Mantissa;
                return true;
            }
            else if (mantDiff > 0)
            {
                proportion = 1.0;
                return true;
            }
            else  // mantDiff == 0
            {
                Debug.Log("Failed to get proportion : Former is exactly same as latter.");
                proportion = 0.0;
                return false;
            }
        }
        else  // expDiff > 0
        {
            proportion = 1.0;
            return true;
        }
    }


    //public static float Round(float value, uint uptoSecondDigitAfterDecimalPoint)
    //{
    //    float mult_A = Mathf.Pow(10.0F, (float)uptoSecondDigitAfterDecimalPoint);
    //    float mult_B = Mathf.Pow(0.1F, (float)uptoSecondDigitAfterDecimalPoint);

    //    return Mathf.Round(value * mult_A) * mult_B;
    //}

    //public static float RoundUptoSecondDigit(float value)
    //{
    //    return Mathf.Round(value * 100.0F) * 0.01F;
    //}

    //public static ulong FormatNumber(ulong value)
    //{
    //    int n = (int)Math.Log(value, 1000);
    //    var m = value / Math.Pow(1000, n);
    //    var unit = "";

    //    if (n < units.Count)
    //    {
    //        unit = units[n];
    //    }
    //    else
    //    {
    //        var unitInt = n - units.Count;
    //        var secondUnit = unitInt % 26;
    //        var firstUnit = unitInt / 26;
    //        unit = Convert.ToChar(firstUnit + charA).ToString() + Convert.ToChar(secondUnit + charA).ToString();
    //    }

    //    // Math.Floor(m * 100) / 100) fixes rounding errors
    //    return (Math.Floor(m * 100) / 100).ToString("0.##") + unit;
    //}
}
