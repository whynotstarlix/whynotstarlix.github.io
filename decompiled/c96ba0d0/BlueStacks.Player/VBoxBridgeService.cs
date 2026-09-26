using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using BlueStacks.Common;
using BstkTypeLib;

namespace BlueStacks.Player;

public class VBoxBridgeService : VBoxBridgeBase
{
	public delegate void BooleanCallback(bool success);

	public delegate void StateChangeCallback(MachineState state);

	private static VBoxBridgeService sInstance;

	private BooleanCallback mStartCallback;

	private BooleanCallback mStopCallback;

	private IConsole mConsole;

	private IMachine mMachine;

	private IDisplay mDisplay;

	private IProgress mPowerProgress;

	internal static VBoxBridgeService Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new VBoxBridgeService();
			}
			return sInstance;
		}
	}

	public bool StartMachineAsync(string name, BooleanCallback startCont, BooleanCallback stopCont)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected Obj, but got Unknown
		Logger.Info("{0} -> {1}", new object[2]
		{
			MethodBase.GetCurrentMethod().Name,
			name
		});
		bool success = false;
		mWorkQueue.DispatchSync((Work)(() =>
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Expected Obj, but got Unknown
			mStartCallback = startCont;
			mStopCallback = stopCont;
			if (!StartMachine_Begin(name))
			{
				Logger.Info("Cannot begin starting guest");
			}
			else
			{
				mWorkQueue.DispatchAsync((Work)StartMachine_Tick);
				success = true;
			}
		}));
		return success;
	}

	public bool StopMachineAsync()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected Obj, but got Unknown
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		bool success = false;
		mWorkQueue.DispatchSync((Work)(() =>
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected Obj, but got Unknown
			if (!StopMachine_Begin())
			{
				Logger.Info("Cannot begin stopping guest");
			}
			else
			{
				mWorkQueue.DispatchAsync((Work)StopMachine_Tick);
				success = true;
			}
		}));
		return success;
	}

	private int ProcessEvent(IEvent ev, StateChangeCallback cb)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected Obj, but got Unknown
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Invalid comparison between Unknown and I4
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Invalid comparison between Unknown and I4
		VBoxEventType type = ev.Type;
		if ((int)type != 32)
		{
			Logger.Info("Unexpected event type {0}", new object[1] { type });
			return -1;
		}
		IMachineStateChangedEvent val = (IMachineStateChangedEvent)ev;
		if (val == null)
		{
			Logger.Info("Cannot query interface");
			return -1;
		}
		if ((int)val.State == 1)
		{
			InternalCloseMachine();
		}
		cb(val.State);
		if ((int)val.State == 1)
		{
			mMachine = null;
			return 0;
		}
		return -1;
	}

	public void RegisterStateChangeEvent(StateChangeCallback cb)
	{
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		IEventSource es = mVirtualBox.EventSource;
		IEventListener listener = es.CreateListener();
		VBoxEventType[] array = new VBoxEventType[1] { (VBoxEventType)32 };
		es.RegisterListener(listener, array, 0);
		Thread thread = new Thread(() =>
		{
			int i;
			for (i = 0; i < 60; i++)
			{
				IEvent val = es.GetEvent(listener, 1000);
				if (val != null)
				{
					int num = ProcessEvent(val, cb);
					es.EventProcessed(listener, val);
					if (num == 0)
					{
						break;
					}
				}
				else
				{
					Logger.Info("Waited for approx {0} seconds for PowerOff event", new object[1] { i + 1 });
				}
			}
			if (i == 60)
			{
				cb((MachineState)7);
			}
			es.UnregisterListener(listener);
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private IMediumAttachment[] GetMediumAttachments(ref string controllerName)
	{
		IMediumAttachment[] array = null;
		try
		{
			array = mMachine.GetMediumAttachmentsOfController("SCSI");
			if (array == null || array.Length == 0)
			{
				Logger.Debug("SCSI controller doesn't exist {0}. Trying to get attachments on SATA");
				array = mMachine.GetMediumAttachmentsOfController("SATA");
				controllerName = "SATA";
			}
			else
			{
				Logger.Debug("Found SCSI controller");
				controllerName = "SCSI";
			}
		}
		catch (Exception ex)
		{
			Logger.Info("Neither SCSI nor SATA controller exists {0} - DEBUG!!!", new object[1] { ex.ToString() });
			throw;
		}
		return array;
	}

	private void CreateDifferencingDiskOfBaseMedium(IMedium srcMedium, string diffMediumName)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("In CreateDifferencingDiskofBaseMedium");
		try
		{
			string controllerName = null;
			if ((int)srcMedium.Type != 0)
			{
				throw new Exception("Unexpected medium type");
			}
			if (srcMedium.Children.Length == 0 && srcMedium.MachineIds == null)
			{
				string text = Path.Combine(Path.GetDirectoryName(srcMedium.Location), diffMediumName);
				IMedium val = mVirtualBox.CreateMedium("vdi", text, (AccessMode)2, (DeviceType)3);
				MediumVariant[] array = new MediumVariant[1] { (MediumVariant)131072 };
				IProgress val2 = srcMedium.CreateDiffStorage(val, array);
				Logger.Info("Successfully created difference image when medium is not attached to any machine");
				while (val2.Completed == 0)
				{
					Thread.Sleep(10);
				}
				return;
			}
			IMediumAttachment[] mediumAttachments = GetMediumAttachments(ref controllerName);
			for (int i = 0; i < mediumAttachments.Length; i++)
			{
				if (mediumAttachments[i].Medium.Base.Name.Equals(srcMedium.Name))
				{
					int port = mediumAttachments[i].Port;
					int device = mediumAttachments[i].Device;
					string text2 = Path.Combine(Path.GetDirectoryName(srcMedium.Location), diffMediumName);
					IMedium val3 = mVirtualBox.CreateMedium("vdi", text2, (AccessMode)2, (DeviceType)3);
					MediumVariant[] array2 = new MediumVariant[1] { (MediumVariant)131072 };
					mMachine.DetachDevice(controllerName, port, device);
					mMachine.SaveSettings();
					IProgress val4 = srcMedium.CreateDiffStorage(val3, array2);
					Logger.Info("Successfully created difference image for medium {0}", new object[1] { srcMedium.Name });
					while (val4.Completed == 0)
					{
						Thread.Sleep(10);
					}
					mMachine.AttachDevice(controllerName, port, device, (DeviceType)3, val3);
					mMachine.SaveSettings();
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error(" Exception in creating differencing storage. Err : " + ex.ToString());
		}
	}

	internal bool CheckIfAppPlayerRooted()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Invalid comparison between Unknown and I4
		if (mVirtualBoxClient == null)
		{
			return false;
		}
		string controllerName = null;
		IMediumAttachment[] mediumAttachments = GetMediumAttachments(ref controllerName);
		for (int i = 0; i < mediumAttachments.Length; i++)
		{
			if (mediumAttachments[i].Port == 0)
			{
				IMedium medium = mediumAttachments[i].Medium;
				if (medium.Name == "Root.vdi" && medium.Id == "fca296ce-8268-4ed7-a57f-d32ec11ab304" && (int)medium.Type == 4)
				{
					return false;
				}
				return true;
			}
		}
		return false;
	}

	private void AddExtraDataKeysIfNotExists()
	{
		Logger.Info("In AddExtraDataKeysIfNotExists");
		List<string> list = mMachine.GetExtraDataKeys().ToList();
		if (!list.Contains("VBoxInternal/Devices/bstsensor/0/PCIBusNo"))
		{
			mMachine.SetExtraData("VBoxInternal/Devices/bstsensor/0/PCIBusNo", "0");
			mMachine.SetExtraData("VBoxInternal/Devices/bstsensor/0/PCIDeviceNo", "15");
			mMachine.SetExtraData("VBoxInternal/Devices/bstsensor/0/PCIFunctionNo", "0");
		}
		else
		{
			Logger.Info("bstsensor is already present");
		}
		if (list.Contains("VBoxInternal/Devices/bstcamera/0/PCIDeviceNo") && mMachine.GetExtraData("VBoxInternal/Devices/bstcamera/0/PCIDeviceNo") != "16")
		{
			mMachine.SetExtraData("VBoxInternal/Devices/bstcamera/0/PCIDeviceNo", "16");
		}
		if (!list.Contains("VBoxInternal/Devices/bstaudio/0/PCIBusNo"))
		{
			mMachine.SetExtraData("VBoxInternal/Devices/bstaudio/0/PCIBusNo", "0");
			mMachine.SetExtraData("VBoxInternal/Devices/bstaudio/0/PCIDeviceNo", "9");
			mMachine.SetExtraData("VBoxInternal/Devices/bstaudio/0/PCIFunctionNo", "0");
		}
		else
		{
			Logger.Info("bstaudio is already present");
		}
		mMachine.SaveSettings();
	}

	private void DisableAudioAdapterSettingIfEnabled()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("In DisableAudioAdapterSettingIfEnabled");
		if ((int)mMachine.AudioAdapter.AudioDriver != 0)
		{
			mMachine.AudioAdapter.AudioDriver = (AudioDriverType)0;
			mMachine.AudioAdapter.Enabled = 0;
			mMachine.SaveSettings();
		}
	}

	private void CreateDifferencingImagesIfRequired()
	{
		string text = "Data.vdi";
		string text2 = Path.Combine(RegistryStrings.DataDir, "Android\\" + text);
		if (!File.Exists(text2))
		{
			Logger.Info("Medium {0} doesn't exist, mediumPath is {1}", new object[2] { text, text2 });
			return;
		}
		IMedium srcMedium = mVirtualBox.OpenMedium(text2, (DeviceType)3, (AccessMode)2, 0);
		string path = text;
		string? directoryName = Path.GetDirectoryName(text2);
		string text3 = Path.GetFileNameWithoutExtension(path) + "_0" + Path.GetExtension(path);
		if (!File.Exists(Path.Combine(directoryName, text3)))
		{
			Logger.Info("Difference image {0} don't exist. Creating now!", new object[1] { text3 });
			CreateDifferencingDiskOfBaseMedium(srcMedium, text3);
		}
		else
		{
			Logger.Info("Differencing disk {0} already exists", new object[1] { text3 });
		}
	}

	private bool StartMachine_Begin(string name)
	{
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("In StartMachine_Begin");
		RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BootParameters = Utils.GetUpdatedBootParamsString("EngineState", RegistryManager.Instance.CurrentEngine, RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BootParameters);
		SerialQueueCheck();
		try
		{
			InternalOpenMachine(name);
		}
		catch (Exception ex)
		{
			Logger.Info("Cannot open virtual machine");
			Logger.Info(ex.ToString());
			return false;
		}
		string text = null;
		text += mMachine.SharedFolders[0].Name;
		for (int i = 1; i < mMachine.SharedFolders.Length; i++)
		{
			text += ",";
			text += mMachine.SharedFolders[i].Name;
		}
		RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BootParameters = Utils.GetUpdatedBootParamsString("SF", text, RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BootParameters);
		if (RegistryManager.Instance.Guest[name].FixVboxConfig)
		{
			AddExtraDataKeysIfNotExists();
			DisableAudioAdapterSettingIfEnabled();
			RegistryManager.Instance.Guest[name].FixVboxConfig = false;
		}
		Logger.Info("Setting machine memory to {0} ", new object[1] { RegistryManager.Instance.DefaultGuest.Memory });
		mMachine.MemorySize = (uint)RegistryManager.Instance.DefaultGuest.Memory;
		int vCPUs = RegistryManager.Instance.DefaultGuest.VCPUs;
		try
		{
			if (vCPUs <= 0 || vCPUs > 32)
			{
				mPowerProgress = mConsole.PowerUp();
			}
			else
			{
				if (mMachine.CPUCount != vCPUs)
				{
					Logger.Info("Overriding VCPUs({0}) from registry", new object[1] { vCPUs });
					mMachine.CPUCount = (uint)vCPUs;
				}
				if (string.Compare(RegistryManager.Instance.EnginePreference, "raw", ignoreCase: true) == 0)
				{
					Logger.Info("Overriding cpu count to 1 for raw mode");
					mMachine.CPUCount = 1u;
					mMachine.SetHWVirtExProperty((HWVirtExPropertyType)1, 0);
					RegistryManager.Instance.CurrentEngine = ((object)(EngineState)1/*cast due to constrained. prefix*/).ToString();
					MultiInstanceUtils.SetDeviceCapsRegistry("Forcefully setting to raw", ((object)(EngineState)1/*cast due to constrained. prefix*/).ToString());
					RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BootParameters = Utils.GetUpdatedBootParamsString("EngineState", ((object)(EngineState)1/*cast due to constrained. prefix*/).ToString(), RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BootParameters);
				}
				mPowerProgress = mConsole.PowerUp();
			}
			TimelineStatsSender.HandleEngineBootEvent(((object)(EngineStatsEvent)3/*cast due to constrained. prefix*/).ToString());
		}
		catch (Exception ex2)
		{
			Logger.Info("Cannot power up virtual machine");
			Logger.Info(ex2.ToString());
			return false;
		}
		return true;
	}

	private bool StopMachine_Begin()
	{
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		SerialQueueCheck();
		try
		{
			mPowerProgress = mConsole.PowerDown();
		}
		catch (Exception ex)
		{
			Logger.Info("Cannot power down virtual machine:");
			Logger.Info(ex.ToString());
			return false;
		}
		return true;
	}

	private void InternalCloseMachine()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		if (mSession != null && (int)((ISession)mSession).State != 1)
		{
			((ISession)mSession).UnlockMachine();
			Logger.Info("Successfully unlocked the machine");
		}
	}

	private void InternalOpenMachine(string name)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("In InternalOpenMachine with vmName: {0}", new object[1] { name });
		SerialQueueCheck();
		IMachine val = null;
		try
		{
			val = mVirtualBox.FindMachine(name);
			Logger.Info("Current machine state: " + ((object)val.State/*cast due to constrained. prefix*/).ToString());
			val.LockMachine(mSession, (LockType)3);
		}
		catch (InvalidCastException ex)
		{
			Logger.Warning("exception in lock vm machine: {0}", new object[1] { ex });
			ComRegistration.Register();
			Logger.Info("Restarting player in case of invalid cast exception after Registering COM components");
			HTTPUtils.SendRequestToClient("restartFrontend", new Dictionary<string, string> { { "vmname", name } }, name, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
		}
		mConsole = ((ISession)mSession).Console;
		mMachine = mConsole.Machine;
		mDisplay = mConsole.Display;
		Logger.Info("Shared folders for the VM are");
		for (int i = 0; i < mMachine.SharedFolders.Length; i++)
		{
			Logger.Info("SF[{0}] -> {1}", new object[2]
			{
				i,
				val.SharedFolders[i].Name
			});
		}
	}

	private void StartMachine_Tick()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected Obj, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Expected Obj, but got Unknown
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		SerialQueueCheck();
		try
		{
			if (IsPowerOperationPending())
			{
				mWorkQueue.DispatchAfter(500.0, (Work)StartMachine_Tick);
				return;
			}
			if (mPowerProgress.ResultCode == 0)
			{
				TimelineStatsSender.HandleEngineBootEvent(((object)(EngineStatsEvent)4/*cast due to constrained. prefix*/).ToString());
				StartMachine_End();
				mStartCallback(success: true);
				if (mMachine.GetHWVirtExProperty((HWVirtExPropertyType)1) != 0)
				{
					MultiInstanceUtils.SetDeviceCapsRegistry("", ((object)(EngineState)0/*cast due to constrained. prefix*/).ToString(), (CpuHvmState)1, (BiosHvmState)1);
				}
				return;
			}
			IVirtualBoxErrorInfo errorInfo = mPowerProgress.ErrorInfo;
			Logger.Info("IProgress:ErrInfo: mPowerProgress.ResultCode={0:X}, ResultDetail={1:X}, mPowerProgress.ErrorInfo.Text={2}", new object[3] { errorInfo.ResultCode, errorInfo.ResultDetail, errorInfo.Text });
			bool flag = errorInfo.Text.Contains("VERR_VMX_NO_VMX") || errorInfo.Text.Contains("VERR_SVM_NO_SVM");
			CpuHvmState val = (CpuHvmState)(!flag);
			bool flag2 = errorInfo.Text.Contains("VERR_VMX_MSR_VMXON_DISABLED") || errorInfo.Text.Contains("VERR_SVM_DISABLED") || errorInfo.Text.Contains("VERR_VMX_MSR_ALL_VMX_DISABLED") || errorInfo.Text.Contains("VERR_VMX_MSR_VMX_DISABLED") || errorInfo.Text.Contains("VERR_VMX_INVALID_VXMON_PTR") || errorInfo.Text.Contains("VERR_VMX_IN_VMX_ROOT_MODE");
			BiosHvmState val2 = (BiosHvmState)(!flag2);
			if (errorInfo.Text.Contains("VERR_UNSUPPORTED_CPU"))
			{
				Logger.Info("UnSupported CPU error came, so quiting frontend with MessageBox");
				QuitFrontEnd("Cannot start virtual machine");
				return;
			}
			if (errorInfo.Text.Contains("VERR_VM_DRIVER_NOT_INSTALLED"))
			{
				Logger.Error("Driver not installed error, exiting with {0}", new object[1] { (object)(PlayerErrorCodes)(-7) });
				AndroidBootUp.SendBootFailureLogs();
				Environment.Exit(-7);
				return;
			}
			Logger.Info("VM Name: " + Strings.CurrentDefaultVmName);
			if (flag | flag2)
			{
				Logger.Info("Restarting service in raw mode...");
				RegistryManager.Instance.CurrentEngine = ((object)(EngineState)1/*cast due to constrained. prefix*/).ToString();
				MultiInstanceUtils.SetDeviceCapsRegistry(errorInfo.Text, ((object)(EngineState)1/*cast due to constrained. prefix*/).ToString(), val, val2);
				RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BootParameters = Utils.GetUpdatedBootParamsString("EngineState", RegistryManager.Instance.CurrentEngine, RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BootParameters);
				if (!Oem.Instance.IsAndroid64Bit)
				{
					Logger.Info("Closing the previous vbox instance before starting raw mode...");
					try
					{
						InternalCloseMachine();
						DisConnect();
					}
					catch (Exception ex)
					{
						Logger.Error("failed in closing/disconnect: " + ex.ToString());
						return;
					}
					Logger.Info("Opening up new connection...");
					try
					{
						Connect();
						InternalOpenMachine(MultiInstanceStrings.VmName);
					}
					catch (Exception ex2)
					{
						Logger.Error("failed in opening/connect: " + ex2.ToString());
						return;
					}
					Logger.Info("Overriding cpu count to 1 for raw mode");
					mMachine.CPUCount = 1u;
					mMachine.SetHWVirtExProperty((HWVirtExPropertyType)1, 0);
					mPowerProgress = mConsole.PowerUp();
					mWorkQueue.DispatchAfter(100.0, (Work)StartMachine_Tick);
				}
				else
				{
					Environment.Exit(-10);
				}
				return;
			}
			throw new Exception("Unexpected Power operation failure...");
		}
		catch (Exception ex3)
		{
			Logger.Info("Cannot start virtual machine");
			Logger.Info(ex3.ToString());
			AndroidBootUp.HandleBootError();
		}
	}

	private void QuitFrontEnd(string plus_failure_reason)
	{
		try
		{
			AndroidBootUp.HandleBootError();
		}
		catch (Exception ex)
		{
			Logger.Info(ex.ToString());
		}
	}

	private void StopMachine_Tick()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected Obj, but got Unknown
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		SerialQueueCheck();
		try
		{
			if (IsPowerOperationPending())
			{
				mWorkQueue.DispatchAfter(100.0, (Work)StopMachine_Tick);
				return;
			}
			StopMachine_End();
			InternalCloseMachine();
			mStopCallback(success: true);
		}
		catch (Exception ex)
		{
			Logger.Info("Cannot stop virtual machine");
			Logger.Info(ex.ToString());
			mStopCallback(success: false);
		}
	}

	private bool IsPowerOperationPending()
	{
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		SerialQueueCheck();
		return mPowerProgress.Completed == 0;
	}

	private void StartMachine_End()
	{
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		SerialQueueCheck();
		CompletePowerOperation();
	}

	private void StopMachine_End()
	{
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		SerialQueueCheck();
		CompletePowerOperation();
	}

	private void CompletePowerOperation()
	{
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		mPowerProgress.WaitForCompletion(-1);
		int resultCode = mPowerProgress.ResultCode;
		if (resultCode != 0)
		{
			throw new COMException("Cannot get power progress", resultCode);
		}
	}

	public bool AddNetworkRedirect(bool isTcp, int guestPort, int hostPort)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("Adding network redirect {0} guestPort({1}) hostPort({2})", new object[3] { isTcp, guestPort, hostPort });
		NATProtocol val = (NATProtocol)(isTcp ? 1 : 0);
		INATEngine nATEngine = mMachine.GetNetworkAdapter(0u).NATEngine;
		if (RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].AllowRemoteAccess == "May I please have remote access?")
		{
			nATEngine.AddRedirect((string)null, val, (string)null, (ushort)hostPort, (string)null, (ushort)guestPort);
		}
		else
		{
			nATEngine.AddRedirect((string)null, val, "127.0.0.1", (ushort)hostPort, (string)null, (ushort)guestPort);
		}
		if (!isTcp)
		{
			Logger.Info("Setting hostforwardsensorport");
			RegistryManager.Instance.DefaultGuest.HostForwardSensorPort = hostPort;
		}
		return true;
	}
}
