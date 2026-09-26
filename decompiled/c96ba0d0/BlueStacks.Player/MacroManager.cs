using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using BlueStacks.Common;

namespace BlueStacks.Player;

public class MacroManager
{
	private static object syncRoot = new object();

	private static MacroManager sInstance = null;

	private EventWaitHandle mMultiMacroEventHandle = new EventWaitHandle(initialState: false, EventResetMode.ManualReset);

	private bool mWasMacroPlaybackStopped;

	public static MacroManager Instance
	{
		get
		{
			if (sInstance == null)
			{
				lock (syncRoot)
				{
					if (sInstance == null)
					{
						sInstance = new MacroManager();
					}
				}
			}
			return sInstance;
		}
	}

	public MacroRecording MacroToPlay { get; private set; }

	internal void InitMacroPlayback(string macroPath)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Invalid comparison between Unknown and I4
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		mWasMacroPlaybackStopped = false;
		MacroGraph.ReCreateMacroGraphInstance();
		string macroName = Path.GetFileNameWithoutExtension(macroPath);
		MacroToPlay = (from MacroRecording macro_ in MacroGraph.Instance.Vertices
			where string.Equals(string.IsNullOrEmpty(macro_.FileName) ? macro_.Name : macro_.FileName, macroName, StringComparison.InvariantCultureIgnoreCase)
			select macro_).FirstOrDefault();
		if ((int)MacroToPlay.RecordingType == 1)
		{
			foreach (MacroRecording leafMacro in MacroGraph.Instance.GetAllLeaves(MacroToPlay))
			{
				MacroRecording val = (from MacroRecording marco_ in MacroGraph.Instance.Vertices
					where marco_.Equals(leafMacro)
					select marco_).FirstOrDefault();
				if ((int)val.RecordingType == 0)
				{
					InputMapper.Instance.InitMacroPlayback(Path.Combine(RegistryStrings.MacroRecordingsFolderPath, (string.IsNullOrEmpty(val.FileName) ? val.Name.ToLower() : val.FileName) + ".json"));
				}
			}
			return;
		}
		InputMapper.Instance.InitMacroPlayback(macroPath);
	}

	private void StartMultiMacroInitAndPlaybackLoop(MacroRecording macro, double acceleration)
	{
		Logger.Info("Starting multi-macro playback loop");
		foreach (MergedMacroConfiguration mergedMacroConfiguration in macro.MergedMacroConfigurations)
		{
			for (int i = 0; i < mergedMacroConfiguration.LoopCount; i++)
			{
				foreach (string macroToRun in mergedMacroConfiguration.MacrosToRun)
				{
					if (mWasMacroPlaybackStopped)
					{
						return;
					}
					MacroRecording val = (from MacroRecording macro_ in MacroGraph.Instance.Vertices
						where string.Equals(macro_.Name, macroToRun, StringComparison.InvariantCultureIgnoreCase)
						select macro_).FirstOrDefault();
					if (val != null)
					{
						if (((BiDirectionalVertex<MacroRecording>)(object)val).Childs.Count > 0)
						{
							StartMultiMacroInitAndPlaybackLoop(val, mergedMacroConfiguration.Acceleration * acceleration);
							continue;
						}
						mMultiMacroEventHandle.Reset();
						InputMapper.Instance.RunMacroUnit(macroToRun, mergedMacroConfiguration.Acceleration * acceleration);
						mMultiMacroEventHandle.WaitOne();
					}
				}
				Thread.Sleep(mergedMacroConfiguration.LoopInterval * 1000);
			}
			Thread.Sleep(mergedMacroConfiguration.DelayNextScript * 1000);
		}
	}

	internal void PlaybackCompleteHandlerImpl()
	{
		mMultiMacroEventHandle?.Set();
	}

	internal void RunMacroUnit()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if ((int)MacroToPlay.RecordingType == 1)
		{
			StartMultiMacroInitAndPlaybackLoop(MacroToPlay, MacroToPlay.Acceleration);
			if (mWasMacroPlaybackStopped)
			{
				mWasMacroPlaybackStopped = true;
				return;
			}
		}
		else
		{
			mMultiMacroEventHandle?.Reset();
			InputMapper.Instance.RunMacroUnit(MacroToPlay.Name, MacroToPlay.Acceleration);
			mMultiMacroEventHandle.WaitOne();
		}
		HTTPUtils.SendRequestToClient("macroPlaybackComplete", (Dictionary<string, string>)null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
	}

	internal void StopMacroPlayback()
	{
		mWasMacroPlaybackStopped = true;
		mMultiMacroEventHandle.Set();
		InputMapper.Instance.StopMacroPlayback();
	}
}
