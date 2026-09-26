using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using BlueStacks.Common;

namespace BlueStacks.Player;

public class DeviceEnumerator : IDisposable
{
	private IMoniker m_Moniker;

	private string m_FriendlyName;

	public string FriendlyName => m_FriendlyName;

	public IMoniker Moniker => m_Moniker;

	public Guid ClassGUID
	{
		get
		{
			m_Moniker.GetClassID(out var pClassID);
			return pClassID;
		}
	}

	public string GetDisplayName
	{
		get
		{
			string ppszDisplayName = null;
			try
			{
				m_Moniker.GetDisplayName(null, null, out ppszDisplayName);
			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString());
			}
			return ppszDisplayName;
		}
	}

	public static List<DeviceEnumerator> ListDevices(Guid filterType)
	{
		List<DeviceEnumerator> list = new List<DeviceEnumerator>();
		DeviceEnumerator deviceEnumerator = null;
		_ = (ErrorHandler)((ICreateDevEnum)new CreateDevEnum()).CreateClassEnumerator(filterType, out var ppEnumMoniker, 0);
		if (ppEnumMoniker != null)
		{
			try
			{
				IMoniker[] array = null;
				try
				{
					while (true)
					{
						array = new IMoniker[1];
						if (ppEnumMoniker.Next(1, array, IntPtr.Zero) != 0)
						{
							break;
						}
						deviceEnumerator = new DeviceEnumerator
						{
							m_Moniker = array[0]
						};
						deviceEnumerator.m_FriendlyName = deviceEnumerator.getProperty("FriendlyName");
						string property = deviceEnumerator.getProperty("DevicePath");
						if (deviceEnumerator.m_FriendlyName != null && property != null && (property.Contains("\\usb#vid") || property.Contains("\\pci#ven")))
						{
							list.Add(deviceEnumerator);
							Logger.Info("Camera device {0}", new object[1] { deviceEnumerator.m_FriendlyName });
						}
					}
					Logger.Info("Breaking out of loop..");
				}
				catch (Exception ex)
				{
					if (array != null)
					{
						Marshal.ReleaseComObject(array[0]);
					}
					list = null;
					Logger.Error("Failed to enumerate Video input devices: {0}", new object[1] { ex.ToString() });
					throw;
				}
			}
			finally
			{
				Marshal.ReleaseComObject(ppEnumMoniker);
			}
		}
		else
		{
			Logger.Error("Cannot enumerate the device");
			list = null;
		}
		return list;
	}

	public void Dispose()
	{
		if (m_Moniker != null)
		{
			Marshal.ReleaseComObject(m_Moniker);
			m_Moniker = null;
		}
		m_FriendlyName = null;
	}

	public string getProperty(string sProperty)
	{
		object ppvObj = null;
		IPropertyBag propertyBag = null;
		string result = null;
		Guid riid = typeof(IPropertyBag).GUID;
		try
		{
			m_Moniker.BindToStorage(null, null, ref riid, out ppvObj);
			propertyBag = (IPropertyBag)ppvObj;
			_ = (ErrorHandler)propertyBag.Read(sProperty, out var pVar, null);
			result = (string)pVar;
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in fetch Property.  {0} Err :", new object[2]
			{
				sProperty,
				ex.ToString()
			});
		}
		finally
		{
			ppvObj = null;
			if (propertyBag != null)
			{
				Marshal.ReleaseComObject(propertyBag);
				propertyBag = null;
			}
		}
		return result;
	}
}
