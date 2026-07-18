using System;
using System.Globalization;

namespace BlueStacks.BlueStacksUI;

public class Fraction : IEquatable<Fraction>, IComparable<Fraction>, IComparable
{
	private long m_iNumerator;

	private long m_iDenominator;

	public long Denominator
	{
		get
		{
			return m_iDenominator;
		}
		set
		{
			Initialize(m_iNumerator, value);
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
			Initialize(value, m_iDenominator);
		}
	}

	public double DoubleValue
	{
		get
		{
			return (double)m_iNumerator / (double)m_iDenominator;
		}
		set
		{
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
		Fraction fraction = ToFraction(Math.Abs(dDecimalValue));
		Initialize(fraction.Numerator, fraction.Denominator);
	}

	public Fraction(string strValue)
	{
		Fraction fraction = ToFraction(strValue);
		Initialize(Math.Abs(fraction.Numerator), Math.Abs(fraction.Denominator));
	}

	public Fraction(long iNumerator, long iDenominator)
	{
		Initialize(iNumerator, iDenominator);
	}

	private void Initialize(long iNumerator, long iDenominator)
	{
		if (iDenominator == 0L)
		{
			throw new DivideByZeroException("Denominator cannot be zero");
		}
		long num = Math.Abs(iNumerator);
		long num2 = Math.Abs(iDenominator);
		if (num == 0L)
		{
			m_iNumerator = 0L;
			m_iDenominator = 1L;
		}
		else
		{
			long num3 = GCD(num, num2);
			m_iNumerator = num / num3;
			m_iDenominator = num2 / num3;
		}
	}

	public double ToDouble()
	{
		return (double)m_iNumerator / (double)m_iDenominator;
	}

	public override string ToString()
	{
		if (m_iDenominator == 1)
		{
			return m_iNumerator.ToString(CultureInfo.InvariantCulture);
		}
		return m_iNumerator.ToString(CultureInfo.InvariantCulture) + "/" + m_iDenominator.ToString(CultureInfo.InvariantCulture);
	}

	public static Fraction ToFraction(string strValue)
	{
		if (string.IsNullOrEmpty(strValue))
		{
			return null;
		}
		int num = strValue.IndexOf('/');
		if (num < 0)
		{
			if (double.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
			{
				return ToFraction(result);
			}
			return new Fraction(0L);
		}
		string s = strValue.Substring(0, num);
		string s2 = strValue.Substring(num + 1);
		long iNumerator = long.Parse(s, NumberStyles.Any, CultureInfo.InvariantCulture);
		long iDenominator = long.Parse(s2, NumberStyles.Any, CultureInfo.InvariantCulture);
		return new Fraction(iNumerator, iDenominator);
	}

	public static Fraction ToFraction(double dValue)
	{
		dValue = Math.Abs(dValue);
		double num = 1E-10;
		if (dValue % 1.0 == 0.0)
		{
			return new Fraction((long)dValue);
		}
		long num2 = 1L;
		long num3 = 0L;
		long num4 = (long)dValue;
		long num5 = 1L;
		double num6 = dValue;
		double num7 = num6 - (double)num4;
		while (num7 > num)
		{
			num6 = 1.0 / num7;
			long num8 = (long)num6;
			checked
			{
				try
				{
					long num9 = num4 * num8 + num2;
					long num10 = num5 * num8 + num3;
					num2 = num4;
					num3 = num5;
					num4 = num9;
					num5 = num10;
				}
				catch (OverflowException)
				{
					break;
				}
				num7 = num6 - (double)num8;
			}
		}
		return new Fraction(num4, num5);
	}

	public Fraction Duplicate()
	{
		return new Fraction(m_iNumerator, m_iDenominator);
	}

	public static Fraction Inverse(Fraction frac1)
	{
		if (frac1 == null || frac1.Numerator == 0L)
		{
			throw new Exception("Operation not possible");
		}
		return new Fraction(frac1.Denominator, frac1.Numerator);
	}

	public static Fraction operator -(Fraction frac1)
	{
		if (frac1 == null)
		{
			return null;
		}
		return frac1;
	}

	public static Fraction operator +(Fraction frac1, Fraction frac2)
	{
		if (frac1 == null || frac2 == null)
		{
			return null;
		}
		checked
		{
			try
			{
				if (frac1.Denominator == frac2.Denominator)
				{
					return new Fraction(frac1.Numerator + frac2.Numerator, frac1.Denominator);
				}
				return new Fraction(frac1.Numerator * frac2.Denominator + frac2.Numerator * frac1.Denominator, frac1.Denominator * frac2.Denominator);
			}
			catch
			{
				return new Fraction(0L);
			}
		}
	}

	public static Fraction operator +(int iNo, Fraction frac1)
	{
		if (frac1 == null)
		{
			return null;
		}
		return Add(frac1, new Fraction(iNo));
	}

	public static Fraction operator +(Fraction frac1, int iNo)
	{
		if (frac1 == null)
		{
			return null;
		}
		return Add(frac1, new Fraction(iNo));
	}

	public static Fraction operator +(double dbl, Fraction frac1)
	{
		if (frac1 == null)
		{
			return null;
		}
		return Add(frac1, ToFraction(dbl));
	}

	public static Fraction operator +(Fraction frac1, double dbl)
	{
		if (frac1 == null)
		{
			return null;
		}
		return Add(frac1, ToFraction(dbl));
	}

	public static Fraction operator -(Fraction frac1, Fraction frac2)
	{
		if (frac1 == null || frac2 == null)
		{
			return null;
		}
		checked
		{
			try
			{
				long iNumerator = frac1.Numerator * frac2.Denominator - frac2.Numerator * frac1.Denominator;
				long iDenominator = frac1.Denominator * frac2.Denominator;
				return new Fraction(iNumerator, iDenominator);
			}
			catch
			{
				return new Fraction(0L);
			}
		}
	}

	public static Fraction operator -(int iNo, Fraction frac1)
	{
		return new Fraction(iNo) - frac1;
	}

	public static Fraction operator -(Fraction frac1, int iNo)
	{
		return frac1 - new Fraction(iNo);
	}

	public static Fraction operator -(double dbl, Fraction frac1)
	{
		return ToFraction(dbl) - frac1;
	}

	public static Fraction operator -(Fraction frac1, double dbl)
	{
		return frac1 - ToFraction(dbl);
	}

	public static Fraction operator *(Fraction frac1, Fraction frac2)
	{
		if (frac1 == null || frac2 == null)
		{
			return null;
		}
		try
		{
			return checked(new Fraction(frac1.Numerator * frac2.Numerator, frac1.Denominator * frac2.Denominator));
		}
		catch
		{
			return new Fraction(0L);
		}
	}

	public static Fraction operator *(int iNo, Fraction frac1)
	{
		return new Fraction(iNo) * frac1;
	}

	public static Fraction operator *(Fraction frac1, int iNo)
	{
		return frac1 * new Fraction(iNo);
	}

	public static Fraction operator *(double dbl, Fraction frac1)
	{
		return ToFraction(dbl) * frac1;
	}

	public static Fraction operator *(Fraction frac1, double dbl)
	{
		return frac1 * ToFraction(dbl);
	}

	public static Fraction operator /(Fraction frac1, Fraction frac2)
	{
		if (frac1 == null || frac2 == null)
		{
			return null;
		}
		if (frac2.Numerator == 0L)
		{
			return null;
		}
		try
		{
			return checked(new Fraction(frac1.Numerator * frac2.Denominator, frac1.Denominator * frac2.Numerator));
		}
		catch
		{
			return new Fraction(0L);
		}
	}

	public static Fraction operator /(int iNo, Fraction frac1)
	{
		return new Fraction(iNo) / frac1;
	}

	public static Fraction operator /(Fraction frac1, int iNo)
	{
		return frac1 / new Fraction(iNo);
	}

	public static Fraction operator /(double dbl, Fraction frac1)
	{
		return ToFraction(dbl) / frac1;
	}

	public static Fraction operator /(Fraction frac1, double dbl)
	{
		return frac1 / ToFraction(dbl);
	}

	public static bool operator ==(Fraction frac1, Fraction frac2)
	{
		if ((object)frac1 == frac2)
		{
			return true;
		}
		if ((object)frac1 == null || (object)frac2 == null)
		{
			return false;
		}
		if (frac1.Numerator == frac2.Numerator)
		{
			return frac1.Denominator == frac2.Denominator;
		}
		return false;
	}

	public static bool operator !=(Fraction frac1, Fraction frac2)
	{
		return !(frac1 == frac2);
	}

	public static bool operator ==(Fraction frac1, int iNo)
	{
		return frac1 == new Fraction(iNo);
	}

	public static bool operator !=(Fraction frac1, int iNo)
	{
		return !(frac1 == iNo);
	}

	public static bool operator ==(Fraction frac1, double dbl)
	{
		return frac1 == new Fraction(dbl);
	}

	public static bool operator !=(Fraction frac1, double dbl)
	{
		return !(frac1 == dbl);
	}

	public static bool operator <(Fraction frac1, Fraction frac2)
	{
		if (frac1 != null && frac2 != null)
		{
			return frac1.Numerator * frac2.Denominator < frac2.Numerator * frac1.Denominator;
		}
		return false;
	}

	public static bool operator >(Fraction frac1, Fraction frac2)
	{
		if (frac1 != null && frac2 != null)
		{
			return frac1.Numerator * frac2.Denominator > frac2.Numerator * frac1.Denominator;
		}
		return false;
	}

	public static bool operator <=(Fraction frac1, Fraction frac2)
	{
		if (frac1 != null && frac2 != null)
		{
			return frac1.Numerator * frac2.Denominator <= frac2.Numerator * frac1.Denominator;
		}
		return false;
	}

	public static bool operator >=(Fraction frac1, Fraction frac2)
	{
		if (frac1 != null && frac2 != null)
		{
			return frac1.Numerator * frac2.Denominator >= frac2.Numerator * frac1.Denominator;
		}
		return false;
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
		if (frac == null)
		{
			return 0.0;
		}
		return frac.ToDouble();
	}

	public static implicit operator string(Fraction frac)
	{
		if (frac == null)
		{
			return string.Empty;
		}
		return frac.ToString();
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as Fraction);
	}

	public override int GetHashCode()
	{
		return (int)((m_iNumerator * 397) ^ m_iDenominator);
	}

	public static Fraction Negate(Fraction frac1)
	{
		return frac1;
	}

	public static Fraction Add(Fraction frac1, Fraction frac2)
	{
		return frac1 + frac2;
	}

	public static Fraction Multiply(Fraction frac1, Fraction frac2)
	{
		return frac1 * frac2;
	}

	private static long GCD(long iNo1, long iNo2)
	{
		while (iNo2 != 0L)
		{
			long num = iNo2;
			iNo2 = iNo1 % iNo2;
			iNo1 = num;
		}
		return iNo1;
	}

	public static void ReduceFraction(Fraction frac)
	{
	}

	public static Fraction Subtract(Fraction frac1, Fraction frac2)
	{
		return frac1 - frac2;
	}

	public static Fraction Divide(Fraction frac1, Fraction frac2)
	{
		return frac1 / frac2;
	}

	public int CompareTo(Fraction frac)
	{
		if (frac == null)
		{
			return 1;
		}
		long num = Numerator * frac.Denominator;
		long num2 = frac.Numerator * Denominator;
		if (num < num2)
		{
			return -1;
		}
		if (num > num2)
		{
			return 1;
		}
		return 0;
	}

	public bool Equals(Fraction other)
	{
		if (other != null && m_iNumerator == other.Numerator)
		{
			return m_iDenominator == other.Denominator;
		}
		return false;
	}

	public int CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		Fraction fraction = obj as Fraction;
		if (fraction != null)
		{
			return CompareTo(fraction);
		}
		throw new ArgumentException("Object must be of type Fraction");
	}
}
