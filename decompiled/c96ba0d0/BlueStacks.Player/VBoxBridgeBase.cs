using System;
using System.Reflection;
using System.Runtime.InteropServices;
using BlueStacks.Common;
using BstkTypeLib;

namespace BlueStacks.Player;

public class VBoxBridgeBase
{
	protected SerialWorkQueue mWorkQueue;

	protected IVirtualBoxClient mVirtualBoxClient;

	protected IVirtualBox mVirtualBox;

	protected Session mSession;

	protected VBoxBridgeBase()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected Obj, but got Unknown
		//IL_0028: Expected Obj, but got Unknown
		mWorkQueue = new SerialWorkQueue("VBoxBridge")
		{
			ExceptionHandler = HandleWorkQueueException
		};
		mWorkQueue.Start();
	}

	private void HandleWorkQueueException(Exception exc)
	{
		Logger.Info("Exception in VBoxBridge work queue:");
		Logger.Info("{0}", new object[1] { exc.ToString() });
		Environment.Exit(-8);
	}

	protected void SerialQueueCheck()
	{
		if (!mWorkQueue.IsCurrentWorkQueue())
		{
			throw new ApplicationException("Cannot run VBoxBridge on this thread");
		}
	}

	public void Connect()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected Obj, but got Unknown
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		mVirtualBoxClient = (IVirtualBoxClient)new VirtualBoxClientClass();
		mVirtualBox = (IVirtualBox)(object)mVirtualBoxClient.VirtualBox;
		Logger.Info("Version: " + mVirtualBox.Version);
		mSession = mVirtualBoxClient.Session;
	}

	public void DisConnect()
	{
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		Marshal.FinalReleaseComObject(mVirtualBoxClient);
		mSession = null;
		mVirtualBox = null;
		mVirtualBoxClient = null;
	}
}
