using System;
using UnityEngine;

[Serializable]
public struct BigNumber
{
    [SerializeField] private int exponent;
    [SerializeField] private double mantissa;

    public int Exponent => exponent;
    public double Mantissa => mantissa;

    private static readonly int MAX_EXPONENT_FOR_INTEGER_EXPRESSION = 1;
    private static readonly int MAX_VALUE_IN_INTEGER_EXPRESSION = (int)Math.Pow(1000.0, MAX_EXPONENT_FOR_INTEGER_EXPRESSION) - 1;
    public static readonly BigNumber Identity = new BigNumber(0.0, 0);

    public BigNumber(double mant, int exp)
    {
        // exp must be bigger than 0 to be valid BigNumber.
        // Surely, there is the case that exp, which is negative, can be valid when mant is bigger than 1000.0.
        // That case is ignored in this example. You should implement it if you need.
        if (exp < 0)
        {
            mantissa = 0.0;
            exponent = 0;
            Debug.Log("Exp must be greater than zero!");
            return;
        }

        if (mant >= 1000.0)
        {
            double tempExp = Math.Floor(Math.Log(mant, 1000.0));
            double tempMant = mant * Math.Pow(1000.0, -tempExp);

            mantissa = tempMant;
            exponent = exp + (int)tempExp;
        }
        else if (mant < 1.0)
        {
            if (exp == 0)
            {
                mantissa = 0.0;
                exponent = 0;
                Debug.Log($"Mant({mant}) is less than 1.0 and exp is zero. BigNumber is set to Identity!");
            }
            else
            {
                // tempExp is now always under zero.
                double tempExp = Math.Floor(Math.Log(mant, 1000.0));
                double tempMant = mant * Math.Pow(1000.0, -tempExp);
                int expSum = exp + (int)tempExp;

                if (expSum < 0)
                {
                    mantissa = 0.0;
                    exponent = 0;
                    Debug.Log($"Mant({mant}) is less than 1.0 and exp({exp}) is greater than zero.\n" +
                        $"But, expSum({expSum}) is less than zero!\n" +
                        $"BigNumber is set to Identity!\n" +
                        $"tempExp({tempExp}), tempMant({tempMant})\n");
                }
                else
                {
                    mantissa = tempMant;
                    exponent = expSum;

                    // Force mantissa to be without decimal point if exponent is zero.
                    if (exponent == 0)
                    {
                        mantissa = Math.Floor(mantissa);
                    }
                }
            }
        }
        else  // 1.0 <= mant < 1000.0
        {
            mantissa = mant;
            exponent = exp;

            // Force mantissa to be without decimal point if exponent is zero.
            if (exponent == 0)
            {
                mantissa = Math.Floor(mantissa);
            }
        }
    }

    public string ToBigNumberString(bool isInteger)
    {
        string exponentUnitCharacters = BigNumberUtils.GetExponentUnit(exponent);
        return isInteger || exponent == 0
                ? $"{mantissa.ToString("##0")}{exponentUnitCharacters}"
                : $"{mantissa.ToString("##0.00")}{exponentUnitCharacters}";
    }

    public int ToBigNumberInteger()
    {
        if (exponent > MAX_EXPONENT_FOR_INTEGER_EXPRESSION)
        {
            Debug.Log($"This BigNumer({mantissa}{exponent}) exceeds max exponent{MAX_EXPONENT_FOR_INTEGER_EXPRESSION}!\n" +
                $"{MAX_VALUE_IN_INTEGER_EXPRESSION} is returned.");
            return MAX_VALUE_IN_INTEGER_EXPRESSION;
        }
        else
            return (int)((float)mantissa * Mathf.Pow(1000.0F, exponent));
    }
}