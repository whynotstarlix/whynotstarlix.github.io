using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using BlueStacks.Common;
using BlueStacks.Common.Grm;
using BlueStacks.Common.Grm.Evaluators;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

internal class GrmHandler
{
	private static Dictionary<string, Dictionary<string, GrmRuleSet>> sDictAppRuleSet = new Dictionary<string, Dictionary<string, GrmRuleSet>>();

	private static CustomMessageWindow AppCompatErrorWindow = null;

	internal static void RequirementConfigUpdated(string vmName = "Android")
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (AppRequirementsParser.Instance.Requirements == null)
		{
			return;
		}
		foreach (AppInfo item in new JsonParser(vmName).GetAppList().ToList())
		{
			RefreshGrmIndication(item.Package, vmName);
		}
		SendUpdateGrmPackagesToAndroid(vmName);
		SendUpdateGrmPackagesToBrowser(vmName);
	}

	internal static void SendUpdateGrmPackagesToAndroid(string vmName)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		try
		{
			if (sDictAppRuleSet.ContainsKey(vmName) && sDictAppRuleSet[vmName].Count != 0 && Utils.IsGuestBooted(vmName, "bgp64"))
			{
				JObject val = new JObject();
				val.Add("GrmPackageList", (JToken)(object)JArray.FromObject((object)sDictAppRuleSet[vmName].Keys));
				JObject val2 = val;
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("data", ((JToken)val2).ToString((Formatting)0, (JsonConverter[])(object)new JsonConverter[0]));
				Dictionary<string, string> dictionary2 = dictionary;
				HTTPUtils.SendRequestToGuestAsync("grmPackages", dictionary2, vmName, 0, (Dictionary<string, string>)null, false, 1, 0);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SendUpdateGrmPackagesToAndroid: " + ex.ToString());
		}
	}

	internal static void SendUpdateGrmPackagesToBrowser(string vmName)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		try
		{
			if (!sDictAppRuleSet.ContainsKey(vmName))
			{
				return;
			}
			JObject val = new JObject();
			foreach (KeyValuePair<string, GrmRuleSet> item in sDictAppRuleSet[vmName])
			{
				((JContainer)val).Add((object)new JProperty(item.Key, (object)item.Value.MessageWindow.MessageType));
			}
			Publisher.PublishMessage((BrowserControlTags)7, vmName, new JObject((object)new JProperty("GrmPackageData", (object)val)));
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in SendUpdateGrmPackagesToBrowser: " + ex.ToString());
		}
	}

	internal static void RefreshGrmIndication(string package, string vmName = "Android")
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			List<AppRequirement> requirements = AppRequirementsParser.Instance.Requirements;
			if (requirements == null)
			{
				return;
			}
			if (!sDictAppRuleSet.ContainsKey(vmName))
			{
				sDictAppRuleSet[vmName] = new Dictionary<string, GrmRuleSet>();
			}
			AppIconModel appIcon = BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.GetAppIcon(package);
			if ((int)appIcon.AppIncompatType != 0 && !requirements.Any((AppRequirement _) => string.Equals(_.PackageName, package, StringComparison.OrdinalIgnoreCase)))
			{
				RemoveAppCompatError(appIcon, BlueStacksUIUtils.DictWindows[vmName]);
			}
			AppRequirement val = requirements.Where((AppRequirement _) => string.Compare(_.PackageName, package, StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();
			if (val == null)
			{
				val = requirements.Where((AppRequirement _) => _.PackageName.EndsWith("*", StringComparison.InvariantCulture) && package.StartsWith(_.PackageName.Trim(new char[1] { '*' }), StringComparison.InvariantCulture)).FirstOrDefault();
			}
			if (val != null)
			{
				GrmRuleSet val2 = val.EvaluateRequirement(package, vmName);
				if (val2 != null)
				{
					AddGRMIndicationForIncompatibleApp(appIcon, BlueStacksUIUtils.DictWindows[vmName], val2);
				}
				else
				{
					RemoveAppCompatError(appIcon, BlueStacksUIUtils.DictWindows[vmName]);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Exception in RefreshGrmIndication. Exception: " + ex);
		}
	}

	internal static void HandleCompatibility(string package, string vmName)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			AppCompatErrorWindow = new CustomMessageWindow();
			AppCompatErrorWindow.TitleTextBlock.Text = BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.GetAppIcon(package).AppName;
			if (!string.IsNullOrEmpty(AppRequirementsParser.Instance.GetLocalizedString(sDictAppRuleSet[vmName][package].MessageWindow.HeaderStringKey)))
			{
				AppCompatErrorWindow.BodyTextBlockTitle.Text = AppRequirementsParser.Instance.GetLocalizedString(sDictAppRuleSet[vmName][package].MessageWindow.HeaderStringKey);
				((UIElement)AppCompatErrorWindow.BodyTextBlockTitle).Visibility = (Visibility)0;
			}
			AppCompatErrorWindow.BodyTextBlock.Text = AppRequirementsParser.Instance.GetLocalizedString(sDictAppRuleSet[vmName][package].MessageWindow.MessageStringKey);
			if (sDictAppRuleSet[vmName][package].MessageWindow.MessageType == ((object)(MessageType)1/*cast due to constrained. prefix*/).ToString())
			{
				AppCompatErrorWindow.MessageIcon.ImageName = "message_info";
			}
			else if (sDictAppRuleSet[vmName][package].MessageWindow.MessageType == ((object)(MessageType)2/*cast due to constrained. prefix*/).ToString())
			{
				AppCompatErrorWindow.MessageIcon.ImageName = "message_error";
			}
			((UIElement)AppCompatErrorWindow.MessageIcon).Visibility = (Visibility)0;
			if (sDictAppRuleSet[vmName][package].MessageWindow.DontShowOption)
			{
				((ContentControl)AppCompatErrorWindow.CheckBox).Content = LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_GOOGLE_APP_POPUP_STRING_04", "");
				((UIElement)AppCompatErrorWindow.CheckBox).Visibility = (Visibility)0;
			}
			foreach (GrmMessageButton button in sDictAppRuleSet[vmName][package].MessageWindow.Buttons)
			{
				ButtonColors val = EnumHelper.Parse<ButtonColors>(button.ButtonColor, (ButtonColors)4);
				AppCompatErrorWindow.AddButton(val, AppRequirementsParser.Instance.GetLocalizedString(button.ButtonStringKey), (EventHandler)delegate
				{
					PerformGrmActions(button.Actions, package, BlueStacksUIUtils.DictWindows[vmName]);
				}, (string)null, false, (object)null);
			}
			((Window)AppCompatErrorWindow).Owner = (Window)(object)BlueStacksUIUtils.DictWindows[vmName];
			BlueStacksUIUtils.DictWindows[vmName].ShowDimOverlay();
			((Window)AppCompatErrorWindow).ShowDialog();
			BlueStacksUIUtils.DictWindows[vmName].HideDimOverlay();
		}
		catch (Exception ex)
		{
			Logger.Error("Exception while showing appcompat message to user. Exception: " + ex.ToString());
			BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.OpenApp(package, isCheckForGrm: false);
		}
	}

	private static void PerformGrmActions(List<GrmAction> actions, string package, MainWindow ParentWindow)
	{
		using BackgroundWorker backgroundWorker = new BackgroundWorker();
		backgroundWorker.DoWork += delegate(object obj, DoWorkEventArgs e)
		{
			PerformGrmActionsWorker_DoWork(e, actions, package, ParentWindow);
		};
		backgroundWorker.RunWorkerCompleted += delegate(object obj, RunWorkerCompletedEventArgs e)
		{
			PerformGrmActionsWorker_RunWorkerCompleted(e, ParentWindow);
		};
		backgroundWorker.RunWorkerAsync();
	}

	private unsafe static void PerformGrmActionsWorker_DoWork(DoWorkEventArgs e, List<GrmAction> actions, string package, MainWindow ParentWindow)
	{
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected I4, but got Unknown
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				((UIElement)ParentWindow.mFrontendGrid).Visibility = (Visibility)1;
				ParentWindow.mExitProgressGrid.ProgressText = LocaleStrings.GetLocalizedString("STRING_PERFORMING_ACTIONS", "");
				((UIElement)ParentWindow.mExitProgressGrid).Visibility = (Visibility)0;
			}, new object[0]);
			try
			{
				ClientStats.SendMiscellaneousStatsAsync("grm_action_clicked", RegistryManager.Instance.UserGuid, string.Join(",", actions.Select((GrmAction _) => _.ActionType.ToString(CultureInfo.InvariantCulture)).ToArray()), RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, "bgp64", package);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while sending misc stat for grm. " + ex);
			}
			e.Result = false;
			foreach (GrmAction action in actions)
			{
				GrmActionType val = EnumHelper.Parse<GrmActionType>(action.ActionType, (GrmActionType)3);
				bool? flag = false;
				switch ((int)val)
				{
				case 0:
				{
					int num = int.Parse(action.ActionDictionary["actionValue"], CultureInfo.InvariantCulture);
					int num2 = 4096;
					if (int.TryParse(PhysicalRamEvaluator.RAM, out var result))
					{
						num2 = (int)((double)result * 0.5);
					}
					if (RegistryManager.Instance.CurrentEngine == ((object)(EngineState)1/*cast due to constrained. prefix*/).ToString() && num2 >= 3072)
					{
						num2 = 3072;
					}
					if (num2 < num)
					{
						num = num2;
					}
					RegistryManager.Instance.Guest[ParentWindow.mVmName].Memory = num;
					flag = true;
					break;
				}
				case 1:
					ParentWindow.Utils.HandleGenericActionFromDictionary(action.ActionDictionary, "grm");
					flag = false;
					break;
				case 2:
				{
					Random random = new Random();
					string text7 = action.ActionDictionary["fileName"];
					text7 += " ";
					string text8 = text7.Substring(0, text7.IndexOf(' '));
					string text9 = text7.Substring(text7.IndexOf(' ') + 1);
					text8 = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", new object[2]
					{
						random.Next(),
						text8
					});
					text8 = Path.Combine(RegistryStrings.PromotionDirectory, text8);
					try
					{
						using (WebClient webClient = new WebClient())
						{
							webClient.DownloadFile(action.ActionDictionary["url"], text8);
						}
						Thread.Sleep(2000);
						using Process process = new Process();
						process.StartInfo.UseShellExecute = true;
						process.StartInfo.CreateNoWindow = true;
						if ((text8.ToUpperInvariant().EndsWith(".msi", StringComparison.InvariantCultureIgnoreCase) || text8.ToUpperInvariant().EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase)) && !BlueStacksUtils.IsSignedByBlueStacks(text8))
						{
							Logger.Info("Not executing unsigned binary " + text8);
							GrmExceptionMessageBox(ParentWindow);
							return;
						}
						if (text8.ToUpperInvariant().EndsWith(".msi", StringComparison.InvariantCultureIgnoreCase))
						{
							process.StartInfo.FileName = "msiexec";
							text9 = string.Format(CultureInfo.InvariantCulture, "/i {0} {1}", new object[2] { text8, text9 });
							process.StartInfo.Arguments = text9;
						}
						else
						{
							process.StartInfo.FileName = text8;
							process.StartInfo.Arguments = text9;
						}
						Logger.Info("Starting process: {0} {1}", new object[2]
						{
							process.StartInfo.FileName,
							text9
						});
						process.Start();
					}
					catch (Exception ex2)
					{
						GrmExceptionMessageBox(ParentWindow);
						Logger.Error("Failed to download and execute. err: " + ex2);
					}
					flag = false;
					break;
				}
				case 3:
					flag = false;
					break;
				case 5:
				{
					string text5 = action.ActionDictionary["actionValue"];
					int glRenderMode = RegistryManager.Instance.Guest[ParentWindow.mVmName].GlRenderMode;
					int glMode = RegistryManager.Instance.Guest[ParentWindow.mVmName].GlMode;
					GlMode val2 = (GlMode)2;
					if (glRenderMode == 1 && glMode == 1)
					{
						val2 = (GlMode)2;
					}
					else if (glRenderMode == 1 && glMode == 2)
					{
						val2 = (GlMode)3;
					}
					else
					{
						switch (glMode)
						{
						case 1:
							val2 = (GlMode)0;
							break;
						case 2:
							val2 = (GlMode)1;
							break;
						}
					}
					List<string> list = (from _ in text5.Split(new char[1] { ',' })
						select _.Trim()).ToList();
					if (list.Contains<string>(((object)(*(GlMode*)(&val2))/*cast due to constrained. prefix*/).ToString(), StringComparer.InvariantCultureIgnoreCase))
					{
						flag = false;
						break;
					}
					text5 = UsefulExtensionMethod.RandomElement<string>((IEnumerable<string>)list);
					string text6 = "";
					int glRenderMode2;
					if (string.Compare(text5.Split(new char[1] { '_' })[1].Trim(), "GL", StringComparison.OrdinalIgnoreCase) == 0)
					{
						glRenderMode2 = 1;
						text6 += "1";
					}
					else
					{
						glRenderMode2 = 4;
						text6 += "4";
					}
					int glMode2;
					if (string.Compare(text5.Split(new char[1] { '_' })[0].Trim(), "PGA", StringComparison.OrdinalIgnoreCase) == 0)
					{
						glMode2 = 1;
						text6 += " 1";
					}
					else
					{
						glMode2 = 2;
						text6 += " 2";
					}
					if (RunCommand.RunCmd(Path.Combine(RegistryStrings.InstallDir, "HD-GlCheck"), text6, true, true, false, 0).ExitCode == 0)
					{
						RegistryManager.Instance.Guest[ParentWindow.mVmName].GlRenderMode = glRenderMode2;
						Utils.UpdateValueInBootParams("GlMode", glMode2.ToString(CultureInfo.InvariantCulture), ParentWindow.mVmName, true, "bgp64");
						RegistryManager.Instance.Guest[ParentWindow.mVmName].GlMode = glMode2;
					}
					else
					{
						GrmExceptionMessageBox(ParentWindow);
						Logger.Info("GL check execution for the required combination failed.");
					}
					flag = true;
					break;
				}
				case 7:
				{
					string text3 = action.ActionDictionary["param"].Trim();
					string text4 = action.ActionDictionary["actionValue"].Trim();
					Utils.UpdateValueInBootParams(text3, text4, ParentWindow.mVmName, true, "bgp64");
					flag = true;
					break;
				}
				case 6:
				{
					string text11 = action.ActionDictionary["pCode"];
					string empty = string.Empty;
					if (string.Compare(text11, "custom", StringComparison.OrdinalIgnoreCase) == 0)
					{
						empty = "{";
						empty += string.Format(CultureInfo.InvariantCulture, "\"createcustomprofile\":\"{0}\",", new object[1] { "true" });
						empty += string.Format(CultureInfo.InvariantCulture, "\"model\":\"{0}\",", new object[1] { action.ActionDictionary["model"] });
						empty += string.Format(CultureInfo.InvariantCulture, "\"brand\":\"{0}\",", new object[1] { action.ActionDictionary["brand"] });
						empty += string.Format(CultureInfo.InvariantCulture, "\"manufacturer\":\"{0}\"", new object[1] { action.ActionDictionary["manufacturer"] });
						empty += "}";
					}
					else
					{
						List<string> list3 = (from _ in text11.Split(new char[1] { ',' })
							select _.Trim()).ToList();
						string valueInBootParams = Utils.GetValueInBootParams("pcode", ParentWindow.mVmName, "", "bgp64");
						if (list3.Contains(valueInBootParams))
						{
							break;
						}
						text11 = UsefulExtensionMethod.RandomElement<string>((IEnumerable<string>)list3);
						empty = "{";
						empty += string.Format(CultureInfo.InvariantCulture, "\"createcustomprofile\":\"{0}\",", new object[1] { "false" });
						empty += string.Format(CultureInfo.InvariantCulture, "\"pcode\":\"{0}\"", new object[1] { text11 });
						empty += "}";
					}
					if (string.Equals(VmCmdHandler.RunCommand(string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[2] { "changeDeviceProfile", empty }), ParentWindow.mVmName), "ok", StringComparison.InvariantCulture))
					{
						Utils.UpdateValueInBootParams("pcode", text11, ParentWindow.mVmName, false, "bgp64");
						if (ThirdParty.AllCallOfDutyPackageNames.Contains(package))
						{
							HTTPUtils.SendRequestToAgentAsync("clearAppData", new Dictionary<string, string> { { "package", package } }, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
						}
					}
					else
					{
						GrmExceptionMessageBox(ParentWindow);
						Logger.Error("Setting device profile for the required combination failed.");
					}
					flag = false;
					break;
				}
				case 8:
				{
					string text = action.ActionDictionary["actionValue"];
					Utils.SetDPIInBootParameters(RegistryManager.Instance.Guest[ParentWindow.mVmName].BootParameters, text, ParentWindow.mVmName, "bgp64");
					flag = true;
					break;
				}
				case 9:
				{
					int num3 = int.Parse(action.ActionDictionary["actionValue"], CultureInfo.InvariantCulture);
					if (RegistryManager.Instance.CurrentEngine != ((object)(EngineState)1/*cast due to constrained. prefix*/).ToString())
					{
						RegistryManager.Instance.Guest[ParentWindow.mVmName].VCPUs = ((num3 > 8) ? 8 : num3);
					}
					flag = true;
					break;
				}
				case 10:
				{
					string text10 = action.ActionDictionary["actionValue"];
					List<string> list2 = (from _ in text10.Split(new char[1] { ',' })
						select _.Replace(" ", string.Empty)).ToList();
					int guestWidth = RegistryManager.Instance.Guest[ParentWindow.mVmName].GuestWidth;
					int guestHeight = RegistryManager.Instance.Guest[ParentWindow.mVmName].GuestHeight;
					string value3 = guestWidth.ToString(CultureInfo.InvariantCulture) + "x" + guestHeight.ToString(CultureInfo.InvariantCulture);
					if (!list2.Contains<string>(value3, StringComparer.InvariantCultureIgnoreCase))
					{
						text10 = UsefulExtensionMethod.RandomElement<string>((IEnumerable<string>)list2);
						RegistryManager.Instance.Guest[ParentWindow.mVmName].GuestWidth = int.Parse(text10.Split(new char[1] { 'x' })[0].Trim(), CultureInfo.InvariantCulture);
						RegistryManager.Instance.Guest[ParentWindow.mVmName].GuestHeight = int.Parse(text10.Split(new char[1] { 'x' })[1].Trim(), CultureInfo.InvariantCulture);
						flag = true;
					}
					break;
				}
				case 11:
					flag = true;
					break;
				case 12:
					Process.Start("shutdown.exe", "-r -t 0");
					break;
				case 13:
				{
					int num4 = int.Parse(action.ActionDictionary["actionValue"], CultureInfo.InvariantCulture);
					RegistryManager.Instance.Guest[ParentWindow.mVmName].FPS = num4;
					Utils.UpdateValueInBootParams("fps", num4.ToString(CultureInfo.InvariantCulture), ParentWindow.mVmName, true, "bgp64");
					Utils.SendChangeFPSToInstanceASync(ParentWindow.mVmName, num4);
					flag = false;
					break;
				}
				case 14:
				{
					string strA = action.ActionDictionary["location"];
					if (string.Compare(strA, "registryManager", StringComparison.OrdinalIgnoreCase) == 0)
					{
						PropertyInfo property = typeof(RegistryManager).GetProperty(action.ActionDictionary["propertyName"]);
						object value = Convert.ChangeType(action.ActionDictionary["propertyValue"], Type.GetTypeCode(property.PropertyType), CultureInfo.InvariantCulture);
						property.SetValue(RegistryManager.Instance, value, null);
					}
					else if (string.Compare(strA, "instanceManager", StringComparison.OrdinalIgnoreCase) == 0)
					{
						PropertyInfo property2 = typeof(InstanceRegistry).GetProperty(action.ActionDictionary["propertyName"]);
						object value2 = Convert.ChangeType(action.ActionDictionary["propertyValue"], Type.GetTypeCode(property2.PropertyType), CultureInfo.InvariantCulture);
						property2.SetValue(RegistryManager.Instance.Guest[ParentWindow.mVmName], value2, null);
					}
					else
					{
						string text2 = string.Format(CultureInfo.InvariantCulture, "Software\\BlueStacks{0}\\{1}", new object[2]
						{
							Strings.GetOemTag(),
							action.ActionDictionary["propertyPath"].Replace("vmName", ParentWindow.mVmName)
						});
						object obj = null;
						RegistryValueKind registryValueKind = EnumHelper.Parse<RegistryValueKind>(action.ActionDictionary["propertyRegistryKind"], RegistryValueKind.String);
						switch (registryValueKind)
						{
						case RegistryValueKind.String:
							obj = action.ActionDictionary["propertyValue"];
							break;
						case RegistryValueKind.DWord:
							obj = int.Parse(action.ActionDictionary["propertyValue"], CultureInfo.InvariantCulture);
							break;
						}
						RegistryUtils.SetRegistryValue(text2, action.ActionDictionary["propertyName"], obj, registryValueKind, (RegistryKeyKind)0);
					}
					flag = false;
					break;
				}
				case 4:
					((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
					{
						((UIElement)ParentWindow.mFrontendGrid).Visibility = (Visibility)0;
						((UIElement)ParentWindow.mExitProgressGrid).Visibility = (Visibility)1;
						if (sDictAppRuleSet[ParentWindow.mVmName][package].MessageWindow.DontShowOption)
						{
							DonotShowCheckboxHandling(ParentWindow, package);
							RefreshGrmIndication(package, ParentWindow.mVmName);
							SendUpdateGrmPackagesToAndroid(ParentWindow.mVmName);
							SendUpdateGrmPackagesToBrowser(ParentWindow.mVmName);
						}
						ParentWindow.mWelcomeTab.mHomeAppManager.OpenApp(package, isCheckForGrm: false);
					}, new object[0]);
					flag = null;
					break;
				case 15:
					HTTPUtils.SendRequestToAgentAsync("clearAppData", new Dictionary<string, string> { { "package", package } }, ParentWindow.mVmName, 0, (Dictionary<string, string>)null, false, 1, 0, "bgp64");
					break;
				}
				if (flag == true)
				{
					e.Result = true;
				}
				else if (!flag.HasValue)
				{
					e.Result = null;
				}
			}
			Thread.Sleep(1000);
		}
		catch (Exception ex3)
		{
			Logger.Error("Exception in performing grm actions, ex: " + ex3.ToString());
			ClientStats.SendMiscellaneousStatsAsync("grm_action_error", RegistryManager.Instance.UserGuid, sDictAppRuleSet[ParentWindow.mVmName][package].RuleId, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, "bgp64", package, ex3.Message);
			GrmExceptionMessageBox(ParentWindow);
			e.Result = null;
		}
	}

	private static void PerformGrmActionsWorker_RunWorkerCompleted(RunWorkerCompletedEventArgs e, MainWindow ParentWindow)
	{
		if (e.Result == null)
		{
			return;
		}
		if (!(bool)e.Result)
		{
			((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
			{
				((UIElement)ParentWindow.mFrontendGrid).Visibility = (Visibility)0;
				((UIElement)ParentWindow.mExitProgressGrid).Visibility = (Visibility)1;
			}, new object[0]);
			RequirementConfigUpdated(ParentWindow.mVmName);
		}
		else
		{
			BlueStacksUIUtils.RestartInstance(ParentWindow.mVmName);
		}
	}

	private static void GrmExceptionMessageBox(MainWindow ParentWindow)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			((UIElement)ParentWindow.mFrontendGrid).Visibility = (Visibility)0;
			((UIElement)ParentWindow.mExitProgressGrid).Visibility = (Visibility)1;
			CustomMessageWindow val = new CustomMessageWindow();
			val.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ERROR", "");
			val.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_GRM_EXCEPTION_MESSAGE", "");
			val.AddButton((ButtonColors)4, "STRING_OK", (EventHandler)null, (string)null, false, (object)null);
			((Window)val).Owner = (Window)(object)ParentWindow;
			ParentWindow.ShowDimOverlay();
			((Window)val).ShowDialog();
			ParentWindow.HideDimOverlay();
		}, new object[0]);
	}

	private static void DonotShowCheckboxHandling(MainWindow ParentWindow, string package)
	{
		if (((ToggleButton)AppCompatErrorWindow.CheckBox).IsChecked == true)
		{
			List<string> list = RegistryManager.Instance.Guest[ParentWindow.mVmName].GrmDonotShowRuleList.ToList();
			if (!list.Contains(sDictAppRuleSet[ParentWindow.mVmName][package].RuleId))
			{
				list.Add(sDictAppRuleSet[ParentWindow.mVmName][package].RuleId);
			}
			RegistryManager.Instance.Guest[ParentWindow.mVmName].GrmDonotShowRuleList = list.ToArray();
		}
	}

	private static void AddGRMIndicationForIncompatibleApp(AppIconModel appIcon, MainWindow ParentWindow, GrmRuleSet passedRuleSet)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected I4, but got Unknown
			MessageType val = EnumHelper.Parse<MessageType>(passedRuleSet.MessageWindow.MessageType, (MessageType)0);
			switch ((int)val)
			{
			case 1:
				appIcon.AppIncompatType = (AppIncompatType)1;
				sDictAppRuleSet[ParentWindow.mVmName][appIcon.PackageName] = passedRuleSet;
				break;
			case 2:
				appIcon.AppIncompatType = (AppIncompatType)2;
				sDictAppRuleSet[ParentWindow.mVmName][appIcon.PackageName] = passedRuleSet;
				break;
			case 0:
				appIcon.AppIncompatType = (AppIncompatType)2;
				sDictAppRuleSet[ParentWindow.mVmName].Remove(appIcon.PackageName);
				break;
			}
		}, new object[0]);
	}

	private static void RemoveAppCompatError(AppIconModel appIcon, MainWindow ParentWindow)
	{
		((DispatcherObject)ParentWindow).Dispatcher.Invoke((Delegate)(Action)delegate
		{
			appIcon.AppIncompatType = (AppIncompatType)0;
			sDictAppRuleSet[ParentWindow.mVmName].Remove(appIcon.PackageName);
		}, new object[0]);
	}

	internal static void RemovePackageFromGrmList(string packageName, string vmName)
	{
		if (sDictAppRuleSet.ContainsKey(vmName))
		{
			sDictAppRuleSet[vmName].Remove(packageName);
		}
	}
}
