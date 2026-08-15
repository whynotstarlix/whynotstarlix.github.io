using System;
using System.Globalization;

namespace BlueStacks.BlueStacksUI;

public class Fraction
{
	private long m_iNumerator;

	private long m_iDenominator;

	private double m_iDoubleValue;

	public long Denominator
	{
		get
		{
			return m_iDenominator;
		}
		set
		{
			if (value != 0L)
			{
				m_iDenominator = value;
				CalculateDoubleValue();
				return;
			}
			throw new FractionException("Denominator cannot be assigned a ZERO Value");
		}
	}

	public long Numerator
	{
		get
		{
			return m_iNumerator;
		}
		set
		{
			m_iNumerator = value;
			CalculateDoubleValue();
		}
	}

	public double DoubleValue
	{
		get
		{
			return m_iDoubleValue;
		}
		set
		{
			m_iDoubleValue = value;
		}
	}

	public Fraction()
	{
		Initialize(0L, 1L);
	}

	public Fraction(long iWholeNumber)
	{
		Initialize(iWholeNumber, 1L);
	}

	public Fraction(double dDecimalValue)
	{
		Fraction fraction = ToFraction(dDecimalValue);
		Initialize(fraction.Numerator, fraction.Denominator);
	}

	public Fraction(string strValue)
	{
		Fraction fraction = ToFraction(strValue);
		Initialize(fraction.Numerator, fraction.Denominator);
	}

	public Fraction(long iNumerator, long iDenominator)
	{
		Initialize(iNumerator, iDenominator);
	}

	private void Initialize(long iNumerator, long iDenominator)
	{
		Numerator = iNumerator;
		Denominator = iDenominator;
		ReduceFraction(this);
	}

	private void CalculateDoubleValue()
	{
		m_iDoubleValue = (double)m_iNumerator / (double)m_iDenominator;
	}

	public double ToDouble()
	{
		return (double)Numerator / (double)Denominator;
	}

	public override string ToString()
	{
		if (Denominator == 1)
		{
			return Numerator.ToString(CultureInfo.InvariantCulture);
		}
		return Numerator + "/" + Denominator;
	}

	public static Fraction ToFraction(string strValue)
	{
		if (!string.IsNullOrEmpty(strValue))
		{
			int i;
			for (i = 0; i < strValue.Length; i++)
			{
				char num = strValue[i];
				int num2 = ((1438011532 > 1707479384) ? 62 : 47);
				if (num == num2)
				{
					break;
				}
			}
			if (i == strValue.Length)
			{
				return Convert.ToDouble(strValue, CultureInfo.InvariantCulture);
			}
			long iNumerator = Convert.ToInt64(strValue.Substring(0, i), CultureInfo.InvariantCulture);
			long iDenominator = Convert.ToInt64(strValue.Substring(i + 1), CultureInfo.InvariantCulture);
			return new Fraction(iNumerator, iDenominator);
		}
		return null;
	}

	public static Fraction ToFraction(double dValue)
	{
		checked
		{
			try
			{
				if (dValue % 1.0 == 0.0)
				{
					return new Fraction((long)dValue);
				}
				double num = dValue;
				long num2 = 1L;
				string text = dValue.ToString(CultureInfo.InvariantCulture);
				while (text.IndexOf("E", StringComparison.InvariantCulture) > 0)
				{
					num *= 10.0;
					num2 *= 10;
					text = num.ToString(CultureInfo.InvariantCulture);
				}
				int i;
				for (i = 0; text[i] != '.'; i++)
				{
				}
				for (int num3 = text.Length - i - 1; num3 > 0; num3--)
				{
					num *= 10.0;
					num2 *= 10;
				}
				return new Fraction((int)Math.Round(num), num2);
			}
			catch (OverflowException)
			{
				throw new FractionException("Conversion not possible due to overflow");
			}
			catch (Exception)
			{
				throw new FractionException("Conversion not possible");
			}
		}
	}

	public Fraction Duplicate()
	{
		return new Fraction
		{
			Numerator = Numerator,
			Denominator = Denominator
		};
	}

	public static Fraction Inverse(Fraction frac1)
	{
		if (null == frac1 || frac1.Numerator == 0L)
		{
			throw new FractionException("Operation not possible (Denominator cannot be assigned a ZERO Value)");
		}
		long denominator = frac1.Denominator;
		long numerator = frac1.Numerator;
		return new Fraction(denominator, numerator);
	}

	public static Fraction operator -(Fraction frac1)
	{
		if (!(null != frac1))
		{
			return null;
		}
		return Negate(frac1);
	}

	public static Fraction operator +(Fraction frac1, Fraction frac2)
	{
		if (!(null != frac1) || !(null != frac2))
		{
			return null;
		}
		return Add(frac1, frac2);
	}

	public static Fraction operator +(int iNo, Fraction frac1)
	{
		if (!(null != frac1))
		{
			return null;
		}
		return Add(frac1, new Fraction(iNo));
	}

	public static Fraction operator +(Fraction frac1, int iNo)
	{
		if (!(null != frac1))
		{
			return null;
		}
		return Add(frac1, new Fraction(iNo));
	}

	public static Fraction operator +(double dbl, Fraction frac1)
	{
		if (!(null != frac1))
		{
			return null;
		}
		return Add(frac1, ToFraction(dbl));
	}

	public static Fraction operator +(Fraction frac1, double dbl)
	{
		if (!(null != frac1))
		{
			return null;
		}
		return Add(frac1, ToFraction(dbl));
	}

	public static Fraction operator -(Fraction frac1, Fraction frac2)
	{
		if (!(null != frac1) || !(null != frac2))
		{
			return null;
		}
		return Add(frac1, -frac2);
	}

	public static Fraction operator -(int iNo, Fraction frac1)
	{
		if (!(null != frac1))
		{
			return null;
		}
		return Add(-frac1, new Fraction(iNo));
	}

	public static Fraction operator -(Fraction frac1, int iNo)
	{
		if (!(null != frac1))
		{
			return null;
		}
		return Add(frac1, -new Fraction(iNo));
	}

	public static Fraction operator -(double dbl, Fraction frac1)
	{
		if (!(null != frac1))
		{
			return null;
		}
		return Add(-frac1, ToFraction(dbl));
	}

	public static Fraction operator -(Fraction frac1, double dbl)
	{
		if (!(null != frac1))
		{
			return null;
		}
		return Add(frac1, -ToFraction(dbl));
	}

	public static Fraction operator *(Fraction frac1, Fraction frac2)
	{
		if (!(null != frac1) || !(null != frac2))
		{
			return null;
		}
		return Multiply(frac1, frac2);
	}

	public static Fraction operator *(int iNo, Fraction frac1)
	{
		if (!(null != frac1))
		{
			return null;
		}
		return Multiply(frac1, new Fraction(iNo));
	}

	public static Fraction operator *(Fraction frac1, int iNo)
	{
		if (!(null != frac1))
		{
			return null;
		}
		return Multiply(frac1, new Fraction(iNo));
	}

	public static Fraction operator *(double dbl, Fraction frac1)
	{
		if (!(null != frac1))
		{
			return null;
		}
		return Multiply(frac1, ToFraction(dbl));
	}

	public static Fraction operator *(Fraction frac1, double dbl)
	{
		if (!(null != frac1))
		{
			return null;
		}
		return Multiply(frac1, ToFraction(dbl));
	}

	public static Fraction operator /(Fraction frac1, Fraction frac2)
	{
		if (!(null != frac1) || !(null != frac2))
		{
			return null;
		}
		return Multiply(frac1, Inverse(frac2));
	}

	public static Fraction operator /(int iNo, Fraction frac1)
	{
		if (!(null != frac1))
		{
			return null;
		}
		return Multiply(Inverse(frac1), new Fraction(iNo));
	}

	public static Fraction operator /(Fraction frac1, int iNo)
	{
		if (!(null != frac1))
		{
			return null;
		}
		return Multiply(frac1, Inverse(new Fraction(iNo)));
	}

	public static Fraction operator /(double dbl, Fraction frac1)
	{
		if (!(null != frac1))
		{
			return null;
		}
		return Multiply(Inverse(frac1), ToFraction(dbl));
	}

	public static Fraction operator /(Fraction frac1, double dbl)
	{
		if (!(null != frac1))
		{
			return null;
		}
		return Multiply(frac1, Inverse(ToFraction(dbl)));
	}

	public static bool operator ==(Fraction frac1, Fraction frac2)
	{
		return frac1?.Equals(frac2) ?? ((object)frac2 == null);
	}

	public static bool operator !=(Fraction frac1, Fraction frac2)
	{
		return !(frac1 == frac2);
	}

	public static bool operator ==(Fraction frac1, int iNo)
	{
		return frac1?.Equals(new Fraction(iNo)) ?? false;
	}

	public static bool operator !=(Fraction frac1, int iNo)
	{
		return !(frac1 == iNo);
	}

	public static bool operator ==(Fraction frac1, double dbl)
	{
		return frac1?.Equals(new Fraction(dbl)) ?? false;
	}

	public static bool operator !=(Fraction frac1, double dbl)
	{
		return !(frac1 == dbl);
	}

	public static bool operator <(Fraction frac1, Fraction frac2)
	{
		if (!(null != frac1) || !(null != frac2))
		{
			return false;
		}
		return frac1.Numerator * frac2.Denominator < frac2.Numerator * frac1.Denominator;
	}

	public static bool operator >(Fraction frac1, Fraction frac2)
	{
		if (!(null != frac1) || !(null != frac2))
		{
			return false;
		}
		return frac1.Numerator * frac2.Denominator > frac2.Numerator * frac1.Denominator;
	}

	public static bool operator <=(Fraction frac1, Fraction frac2)
	{
		if (!(null != frac1) || !(null != frac2))
		{
			if (!(null == frac1) || !(null == frac2))
			{
				return false;
			}
			return true;
		}
		return frac1.Numerator * frac2.Denominator <= frac2.Numerator * frac1.Denominator;
	}

	public static bool operator >=(Fraction frac1, Fraction frac2)
	{
		if (!(null != frac1) || !(null != frac2))
		{
			if (!(null == frac1) || !(null == frac2))
			{
				return false;
			}
			return true;
		}
		return frac1.Numerator * frac2.Denominator >= frac2.Numerator * frac1.Denominator;
	}

	public static implicit operator Fraction(long lNo)
	{
		return new Fraction(lNo);
	}

	public static implicit operator Fraction(double dNo)
	{
		return new Fraction(dNo);
	}

	public static implicit operator Fraction(string strNo)
	{
		return new Fraction(strNo);
	}

	public static explicit operator double(Fraction frac)
	{
		if (!(null != frac))
		{
			return 0.0;
		}
		return frac.ToDouble();
	}

	public static implicit operator string(Fraction frac)
	{
		if (!(null != frac))
		{
			return string.Empty;
		}
		return frac.ToString();
	}

	public override bool Equals(object obj)
	{
		Fraction fraction = (Fraction)obj;
		if (Numerator == fraction?.Numerator)
		{
			return Denominator == fraction?.Denominator;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Convert.ToInt32((Numerator ^ Denominator) & 0xFFFFFFFFu);
	}

	public static Fraction Negate(Fraction frac1)
	{
		if ((object)frac1 != null)
		{
			long iNumerator = -frac1.Numerator;
			long denominator = frac1.Denominator;
			return new Fraction(iNumerator, denominator);
		}
		return null;
	}

	public static Fraction Add(Fraction frac1, Fraction frac2)
	{
		checked
		{
			try
			{
				if ((object)frac1 != null && (object)frac2 != null)
				{
					long iNumerator = frac1.Numerator * frac2.Denominator + frac2.Numerator * frac1.Denominator;
					long iDenominator = frac1.Denominator * frac2.Denominator;
					return new Fraction(iNumerator, iDenominator);
				}
				if ((object)frac1 != null)
				{
					return frac1;
				}
				return frac2;
			}
			catch (OverflowException)
			{
				throw new FractionException("Overflow occurred while performing arithemetic operation");
			}
			catch (Exception)
			{
				throw new FractionException("An error occurred while performing arithemetic operation");
			}
		}
	}

	public static Fraction Multiply(Fraction frac1, Fraction frac2)
	{
		checked
		{
			try
			{
				if ((object)frac1 != null && (object)frac2 != null)
				{
					long iNumerator = frac1.Numerator * frac2.Numerator;
					long iDenominator = frac1.Denominator * frac2.Denominator;
					return new Fraction(iNumerator, iDenominator);
				}
				return null;
			}
			catch (OverflowException)
			{
				throw new FractionException("Overflow occurred while performing arithemetic operation");
			}
			catch (Exception)
			{
				throw new FractionException("An error occurred while performing arithemetic operation");
			}
		}
	}

	private static long GCD(long iNo1, long iNo2)
	{
		if (iNo1 < 0)
		{
			iNo1 = -iNo1;
		}
		if (iNo2 < 0)
		{
			iNo2 = -iNo2;
		}
		do
		{
			if (iNo1 < iNo2)
			{
				long num = iNo1;
				iNo1 = iNo2;
				iNo2 = num;
			}
			iNo1 %= iNo2;
		}
		while (iNo1 != 0L);
		return iNo2;
	}

	public static void ReduceFraction(Fraction frac)
	{
		try
		{
			if (!(null != frac))
			{
				return;
			}
			if (frac.Numerator == 0L)
			{
				frac.Denominator = 1L;
				return;
			}
			long num = GCD(frac.Numerator, frac.Denominator);
			frac.Numerator /= num;
			frac.Denominator /= num;
			if (frac.Denominator < 0)
			{
				frac.Numerator *= -1L;
				frac.Denominator *= -1L;
			}
		}
		catch (Exception ex)
		{
			throw new FractionException("Cannot reduce Fraction: " + ex.Message);
		}
	}

	public static Fraction Subtract(Fraction frac1, Fraction frac2)
	{
		if (!(null != frac1) || !(null != frac2))
		{
			return null;
		}
		return Add(frac1, -frac2);
	}

	public static Fraction Divide(Fraction frac1, Fraction frac2)
	{
		if (!(null != frac1) || !(null != frac2))
		{
			return null;
		}
		return Multiply(frac1, Inverse(frac2));
	}

	public int CompareTo(Fraction frac)
	{
		if ((object)frac == null)
		{
			return 1;
		}
		if (Numerator * Denominator == frac.Numerator * frac.Denominator)
		{
			return 0;
		}
		if (Numerator * Denominator < frac.Numerator * frac.Denominator)
		{
			return 1;
		}
		return -68798400 + 842798237 % 85999982;
	}
}
