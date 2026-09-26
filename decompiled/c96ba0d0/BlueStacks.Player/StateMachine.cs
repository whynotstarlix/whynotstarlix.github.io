using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using BlueStacks.Common;
using BstkTypeLib;

namespace BlueStacks.Player;

internal class StateMachine
{
	private enum State
	{
		Init,
		Starting,
		StartingCancel,
		WaitingForNetwork,
		Running,
		ShuttingDown,
		Stopping,
		Stopped,
		Error
	}

	public delegate void VoidCallback();

	public delegate void BooleanCallback(bool success);

	private string mVmName;

	private SerialWorkQueue mWorkQueue;

	private EventWaitHandle mTerminationEvent;

	private State mState;

	private BooleanCallback mStartCallback;

	private VoidCallback mRunningCallback;

	private Timer mShutdownTimer;

	internal static int mForceShutdownDueTime = 3000;

	public VoidCallback RunningCallback
	{
		set
		{
			mRunningCallback = value;
		}
	}

	public StateMachine(string vmName)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		//IL_002f: Expected Obj, but got Unknown
		mVmName = vmName;
		mWorkQueue = new SerialWorkQueue("StateMachine")
		{
			ExceptionHandler = HandleWorkQueueException
		};
		mWorkQueue.Start();
		mTerminationEvent = new EventWaitHandle(initialState: false, EventResetMode.ManualReset);
		mState = State.Init;
	}

	private void HandleWorkQueueException(Exception exc)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected Obj, but got Unknown
		Logger.Info("Exception in StateMachine work queue:");
		Logger.Info(exc.ToString());
		mWorkQueue.DispatchAsync((Work)(() =>
		{
			EnterStateError();
		}));
	}

	private void SerialQueueCheck()
	{
		if (!mWorkQueue.IsCurrentWorkQueue())
		{
			throw new ApplicationException("Cannot run StateMachine on this thread");
		}
	}

	public void Start(BooleanCallback startCallback)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected Obj, but got Unknown
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		Exception exc = null;
		mWorkQueue.DispatchSync((Work)(() =>
		{
			if (mState != State.Init)
			{
				exc = new ApplicationException("Cannot start state machine in state " + mState);
			}
			else
			{
				mStartCallback = startCallback;
				EnterStateStarting();
			}
		}));
		if (exc != null)
		{
			throw exc;
		}
	}

	public void RequestTermination()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		mWorkQueue.DispatchAsync((Work)(() =>
		{
			Logger.Info("{0} -> {1}", new object[2]
			{
				MethodBase.GetCurrentMethod().Name,
				mState
			});
			switch (mState)
			{
			case State.Init:
				EnterStateError();
				break;
			case State.Starting:
				EnterStateStartingCancel();
				break;
			case State.WaitingForNetwork:
			case State.Running:
				EnterStateShuttingDown();
				break;
			case State.StartingCancel:
			case State.ShuttingDown:
			case State.Stopping:
			case State.Stopped:
			case State.Error:
				break;
			}
		}));
	}

	public void WaitForTermination()
	{
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		mTerminationEvent.WaitOne();
	}

	private void EnterStateStarting()
	{
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		SerialQueueCheck();
		mState = State.Starting;
		try
		{
			VBoxBridgeService.Instance.Connect();
		}
		catch (Exception ex)
		{
			Logger.Info("Cannot connect VBoxBridge");
			Logger.Info(ex.ToString());
			try
			{
				ComRegistration.Register();
				Logger.Info("Reconnecting to VBoxBridge");
				VBoxBridgeService.Instance.Connect();
			}
			catch (Exception ex2)
			{
				Logger.Info("Got exception {0} while re-registering COM", new object[1] { ex2.ToString() });
				AndroidBootUp.HandleBootError();
			}
		}
		if (!VBoxBridgeService.Instance.StartMachineAsync(mVmName, (bool success) =>
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected Obj, but got Unknown
			mWorkQueue.DispatchAsync((Work)(() =>
			{
				StartMachineCompletion(success);
			}));
		}, (bool success) =>
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected Obj, but got Unknown
			mWorkQueue.DispatchAsync((Work)(() =>
			{
				StopMachineCompletion(success);
			}));
		}))
		{
			Logger.Info("Cannot begin starting guest");
			EnterStateError();
			AndroidBootUp.HandleBootError();
		}
	}

	private void StartMachineCompletion(bool success)
	{
		Logger.Info("Start callback -> {0}", new object[1] { success });
		if (success)
		{
			switch (mState)
			{
			case State.Starting:
				EnterStateWaitingForNetwork();
				break;
			case State.StartingCancel:
				EnterStateStopping();
				break;
			case State.WaitingForNetwork:
				Logger.Info("Start machine continuationcalled in state {0}", new object[1] { mState });
				mStartCallback = null;
				return;
			default:
				Logger.Info("Start machine continuation called in state " + mState);
				break;
			}
		}
		else
		{
			EnterStateError();
		}
		mStartCallback?.Invoke(success);
		mStartCallback = null;
	}

	private void StopMachineCompletion(bool success)
	{
		Logger.Info("Start callback -> {0}", new object[1] { success });
		if (success)
		{
			if (mState == State.Stopping)
			{
				EnterStateStopped();
			}
			else
			{
				Logger.Info("Stop machine in state: " + mState);
			}
		}
		else
		{
			EnterStateError();
		}
		mTerminationEvent.Set();
	}

	private void EnterStateStartingCancel()
	{
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		SerialQueueCheck();
	}

	private void EnterStateWaitingForNetwork()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected Obj, but got Unknown
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		SerialQueueCheck();
		mState = State.WaitingForNetwork;
		mWorkQueue.DispatchAfter(1.0, (Work)(() =>
		{
			if (mState == State.WaitingForNetwork)
			{
				EnterStateRunning();
			}
		}));
	}

	private void EnterStateRunning()
	{
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		SerialQueueCheck();
		mState = State.Running;
		mRunningCallback?.Invoke();
	}

	private void EnterStateShuttingDown()
	{
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		SerialQueueCheck();
		mState = State.ShuttingDown;
		VBoxBridgeService.Instance.RegisterStateChangeEvent((MachineState newMstate) =>
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected Obj, but got Unknown
			mWorkQueue.DispatchSync((Work)(() =>
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Invalid comparison between Unknown and I4
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Invalid comparison between Unknown and I4
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Invalid comparison between Unknown and I4
				Logger.Info("Callback for state {0} called...", new object[1] { newMstate });
				MachineState val = newMstate;
				if ((int)val != 1)
				{
					if ((int)val != 7)
					{
						if ((int)val == 11)
						{
							mState = State.Stopping;
						}
						else
						{
							Logger.Info("Invalid machine state");
						}
					}
					else
					{
						StopMachineCompletion(success: false);
					}
				}
				else
				{
					mShutdownTimer.Change(-1, -1);
					StopMachineCompletion(success: true);
				}
			}));
		});
		if (mShutdownTimer == null)
		{
			mShutdownTimer = new Timer((object x) =>
			{
				ForceShutdown();
			}, null, mForceShutdownDueTime, -1);
			Logger.Info("Shutdown timer started with due time {0}", new object[1] { mForceShutdownDueTime });
		}
		using InputManagerProxy inputManagerProxy = new InputManagerProxy(Process.GetCurrentProcess().Id);
		inputManagerProxy.SendControlShutdown();
	}

	private void ForceShutdown()
	{
		Logger.Warning("No callback recieved, exiting player");
		mShutdownTimer.Change(-1, -1);
		Environment.Exit(-9);
	}

	private void EnterStateStopping()
	{
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		SerialQueueCheck();
		mState = State.Stopping;
		if (!VBoxBridgeService.Instance.StopMachineAsync())
		{
			Logger.Info("Cannot stop guest");
			EnterStateError();
		}
	}

	private void EnterStateStopped()
	{
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		SerialQueueCheck();
		VBoxBridgeService.Instance.DisConnect();
		mState = State.Stopped;
	}

	private void EnterStateError()
	{
		Logger.Info("{0}", new object[1] { MethodBase.GetCurrentMethod().Name });
		SerialQueueCheck();
		mStartCallback?.Invoke(success: false);
		mState = State.Error;
		mTerminationEvent.Set();
	}
}
