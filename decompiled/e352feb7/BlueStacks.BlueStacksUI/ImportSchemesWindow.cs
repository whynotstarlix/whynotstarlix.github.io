using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

public class ImportSchemesWindow : CustomWindow, IComponentConnector
{
	private KeymapCanvasWindow CanvasWindow;

	private MainWindow ParentWindow;

	internal StackPanel mSchemesStackPanel;

	internal int mNumberOfSchemesSelectedForImport;

	private Dictionary<string, IMControlScheme> dict = new Dictionary<string, IMControlScheme>();

	private Dictionary<string, Dictionary<string, string>> mStringsToImport;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Border mMaskBorder;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ScrollViewer mSchemesListScrollbar;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomCheckbox mSelectAllBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mImportBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal ProgressBar mLoadingGrid;

	private bool _contentLoaded;

	public ImportSchemesWindow(KeymapCanvasWindow window, MainWindow mainWindow)
	{
		InitializeComponent();
		CanvasWindow = window;
		ParentWindow = mainWindow;
		object content = ((ContentControl)mSchemesListScrollbar).Content;
		mSchemesStackPanel = (StackPanel)((content is StackPanel) ? content : null);
	}

	private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		CloseWindow();
	}

	private void CloseWindow()
	{
		((Window)this).Close();
		CanvasWindow.SidebarWindow.mImportSchemesWindow = null;
		((UIElement)CanvasWindow.SidebarWindow.mOverlayGrid).Visibility = (Visibility)1;
		((UIElement)CanvasWindow.SidebarWindow).Focus();
	}

	internal void Init(string fileName)
	{
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Expected O, but got Unknown
		List<string> schemeNames;
		try
		{
			schemeNames = new List<string>();
			foreach (IMControlScheme controlScheme in ParentWindow.SelectedConfig.ControlSchemes)
			{
				schemeNames.Add(controlScheme.Name);
			}
			((Panel)mSchemesStackPanel).Children.Clear();
			JObject val = JObject.Parse(File.ReadAllText(fileName));
			IMConfig deserializedIMConfigObject;
			if (ConfigConverter.GetConfigVersion(val) < 14)
			{
				JObject obj = ConfigConverter.Convert(val, "14", false, true);
				JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
				serializerSettings.Formatting = (Formatting)1;
				deserializedIMConfigObject = KMManager.GetDeserializedIMConfigObject(JsonConvert.SerializeObject((object)obj, serializerSettings), isFileNameUsed: false);
			}
			else if (ConfigConverter.GetConfigVersion(val) < 16 && (string.Equals(ParentWindow.StaticComponents.mSelectedTabButton.PackageName, "com.dts.freefireth", StringComparison.InvariantCultureIgnoreCase) || ThirdParty.AllCallOfDutyPackageNames.Contains(ParentWindow.StaticComponents.mSelectedTabButton.PackageName)))
			{
				JObject val2 = val;
				foreach (JObject item in (IEnumerable<JToken>)val["ControlSchemes"])
				{
					JObject val3 = item;
					val3["Images"] = (JToken)(object)ConfigConverter.ConvertImagesArrayForPV16(val3);
				}
				val2["MetaData"][(object)"Comment"] = JToken.op_Implicit(string.Format(CultureInfo.InvariantCulture, "Generated automatically from ver {0}", new object[1] { (int)val2["MetaData"][(object)"ParserVersion"] }));
				val2["MetaData"][(object)"ParserVersion"] = JToken.op_Implicit(16);
				JsonSerializerSettings serializerSettings2 = Utils.GetSerializerSettings();
				serializerSettings2.Formatting = (Formatting)1;
				deserializedIMConfigObject = KMManager.GetDeserializedIMConfigObject(JsonConvert.SerializeObject((object)val2, serializerSettings2), isFileNameUsed: false);
			}
			else
			{
				deserializedIMConfigObject = KMManager.GetDeserializedIMConfigObject(fileName);
			}
			mStringsToImport = deserializedIMConfigObject.Strings;
			mNumberOfSchemesSelectedForImport = 0;
			deserializedIMConfigObject.ControlSchemes.Where((IMControlScheme scheme) => scheme.BuiltIn).ToList().ForEach(delegate(IMControlScheme scheme)
			{
				AddSchemeToImportCheckbox(scheme);
			});
			deserializedIMConfigObject.ControlSchemes.Where((IMControlScheme scheme) => !scheme.BuiltIn).ToList().ForEach(delegate(IMControlScheme scheme)
			{
				if (Enumerable.Contains(dict.Keys, scheme.Name.ToLower(CultureInfo.InvariantCulture).Trim()))
				{
					scheme.Name += " (Edited)";
					scheme.Name = KMManager.GetUniqueName(scheme.Name, schemeNames);
				}
				AddSchemeToImportCheckbox(scheme);
			});
		}
		catch (Exception ex)
		{
			Logger.Error("Error in import window init err: " + ex.ToString());
		}
		void AddSchemeToImportCheckbox(IMControlScheme scheme)
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			dict.Add(scheme.Name.ToLower(CultureInfo.InvariantCulture).Trim(), scheme);
			ImportSchemesWindowControl importSchemesWindowControl = new ImportSchemesWindowControl(this, ParentWindow);
			((FrameworkElement)importSchemesWindowControl).Width = ((FrameworkElement)mSchemesStackPanel).Width;
			ImportSchemesWindowControl importSchemesWindowControl2 = importSchemesWindowControl;
			((ContentControl)importSchemesWindowControl2.mContent).Content = scheme.Name;
			((FrameworkElement)importSchemesWindowControl2).Margin = new Thickness(0.0, 1.0, 0.0, 1.0);
			foreach (string key in ParentWindow.SelectedConfig.ControlSchemesDict.Keys)
			{
				if (string.Equals(key.Trim(), ((ContentControl)importSchemesWindowControl2.mContent).Content.ToString().Trim(), StringComparison.InvariantCultureIgnoreCase))
				{
					((UIElement)importSchemesWindowControl2.mBlock).Visibility = (Visibility)0;
					((TextBox)importSchemesWindowControl2.mImportName).Text = KMManager.GetUniqueName(((ContentControl)importSchemesWindowControl2.mContent).Content.ToString().Trim(), schemeNames);
					break;
				}
			}
			((Panel)mSchemesStackPanel).Children.Add((UIElement)(object)importSchemesWindowControl2);
		}
	}

	internal void Box_Unchecked(object sender, RoutedEventArgs e)
	{
		mNumberOfSchemesSelectedForImport--;
		if (mNumberOfSchemesSelectedForImport == ((Panel)mSchemesStackPanel).Children.Count - 1)
		{
			((ToggleButton)mSelectAllBtn).IsChecked = false;
		}
		if (mNumberOfSchemesSelectedForImport == 0)
		{
			((UIElement)mImportBtn).IsEnabled = false;
		}
	}

	internal void Box_Checked(object sender, RoutedEventArgs e)
	{
		mNumberOfSchemesSelectedForImport++;
		if (mNumberOfSchemesSelectedForImport == ((Panel)mSchemesStackPanel).Children.Count)
		{
			((ToggleButton)mSelectAllBtn).IsChecked = true;
		}
		if (mNumberOfSchemesSelectedForImport == 1)
		{
			((UIElement)mImportBtn).IsEnabled = true;
		}
	}

	private bool EditedNameIsAllowed(string text, ImportSchemesWindowControl item)
	{
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(text.Trim()))
		{
			BlueStacksUIBinding.Bind(item.mWarningMsg, LocaleStrings.GetLocalizedString("STRING_INVALID_SCHEME_NAME", ""), "");
			return false;
		}
		if (text.Trim().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
		{
			BlueStacksUIBinding.Bind(item.mWarningMsg, LocaleStrings.GetLocalizedString("STRING_INVALID_SCHEME_NAME", ""), "");
			return false;
		}
		foreach (IMControlScheme controlScheme in ParentWindow.SelectedConfig.ControlSchemes)
		{
			if (controlScheme.Name.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim())
			{
				return false;
			}
		}
		foreach (ImportSchemesWindowControl child in ((Panel)mSchemesStackPanel).Children)
		{
			if (((ToggleButton)child.mContent).IsChecked == true && (int)((UIElement)child.mBlock).Visibility == 0 && ((TextBox)child.mImportName).Text.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim() && ((ContentControl)child.mContent).Content.ToString().Trim().ToLower(CultureInfo.InvariantCulture) != ((ContentControl)item.mContent).Content.ToString().Trim().ToLower(CultureInfo.InvariantCulture))
			{
				return false;
			}
			if (((ToggleButton)child.mContent).IsChecked == true && ((ContentControl)child.mContent).Content.ToString().ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim())
			{
				return false;
			}
		}
		return true;
	}

	private void ImportBtn_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			int num = 0;
			bool flag = true;
			List<IMControlScheme> list = new List<IMControlScheme>();
			foreach (ImportSchemesWindowControl child in ((Panel)mSchemesStackPanel).Children)
			{
				if (((ToggleButton)child.mContent).IsChecked == true)
				{
					list.Add(dict.ElementAt(num).Value);
					if (ParentWindow.SelectedConfig.ControlSchemesDict.Keys.Select((string key) => key.ToLower(CultureInfo.InvariantCulture).Trim()).Contains(((ContentControl)child.mContent).Content.ToString().ToLower(CultureInfo.InvariantCulture).Trim()))
					{
						if (!EditedNameIsAllowed(((TextBox)child.mImportName).Text, child))
						{
							((XTextBox)child.mImportName).InputTextValidity = (TextValidityOptions)(-1);
							if (!string.IsNullOrEmpty(((TextBox)child.mImportName).Text) && ((TextBox)child.mImportName).Text.Trim().IndexOfAny(Path.GetInvalidFileNameChars()) < 0)
							{
								BlueStacksUIBinding.Bind(child.mWarningMsg, LocaleStrings.GetLocalizedString("STRING_DUPLICATE_SCHEME_NAME_WARNING", ""), "");
							}
							((UIElement)child.mWarningMsg).Visibility = (Visibility)0;
							flag = false;
						}
						else
						{
							((XTextBox)child.mImportName).InputTextValidity = (TextValidityOptions)1;
							((UIElement)child.mWarningMsg).Visibility = (Visibility)2;
						}
					}
				}
				num++;
			}
			if (list.Count == 0)
			{
				ParentWindow.mCommonHandler.AddToastPopup((Window)(object)this, LocaleStrings.GetLocalizedString("STRING_NO_SCHEME_SELECTED", ""));
			}
			else
			{
				if (!flag)
				{
					return;
				}
				foreach (IMControlScheme item in list)
				{
					ImportSchemesWindowControl controlFromScheme = GetControlFromScheme(item);
					if (ParentWindow.SelectedConfig.ControlSchemesDict.Keys.Select((string key) => key.ToLower(CultureInfo.InvariantCulture)).Contains(((ContentControl)controlFromScheme.mContent).Content.ToString().ToLower(CultureInfo.InvariantCulture).Trim()))
					{
						item.Name = ((TextBox)controlFromScheme.mImportName).Text.Trim();
					}
				}
				mStringsToImport = KMManager.CleanupGuidanceAccordingToSchemes(list, mStringsToImport);
				ImportSchemes(list, mStringsToImport);
				KeymapCanvasWindow.sIsDirty = true;
				KMManager.SaveIMActions(ParentWindow, isSavedFromGameControlWindow: false);
				CanvasWindow.SidebarWindow.FillProfileCombo();
				CanvasWindow.SidebarWindow.ProfileChanged();
				KMManager.SendSchemeChangedStats(ParentWindow, "import_scheme");
				ParentWindow.mCommonHandler.AddToastPopup((Window)(object)CanvasWindow.SidebarWindow, LocaleStrings.GetLocalizedString("STRING_CONTROLS_IMPORTED", ""));
				CloseWindow();
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error while importing script. err:" + ex.ToString());
		}
	}

	private ImportSchemesWindowControl GetControlFromScheme(IMControlScheme scheme)
	{
		foreach (ImportSchemesWindowControl child in ((Panel)mSchemesStackPanel).Children)
		{
			if (((ContentControl)child.mContent).Content.ToString().Trim().ToLower(CultureInfo.InvariantCulture) == scheme.Name.ToLower(CultureInfo.InvariantCulture).Trim())
			{
				return child;
			}
		}
		return null;
	}

	private void SelectAllBtn_Click(object sender, RoutedEventArgs e)
	{
		if (((ToggleButton)mSelectAllBtn).IsChecked.Value)
		{
			foreach (ImportSchemesWindowControl child in ((Panel)mSchemesStackPanel).Children)
			{
				((ToggleButton)child.mContent).IsChecked = true;
			}
			return;
		}
		foreach (ImportSchemesWindowControl child2 in ((Panel)mSchemesStackPanel).Children)
		{
			((ToggleButton)child2.mContent).IsChecked = false;
		}
	}

	internal void ImportSchemes(List<IMControlScheme> toCopyFromSchemes, Dictionary<string, Dictionary<string, string>> stringsToImport)
	{
		bool flag = false;
		bool flag2 = false;
		KMManager.MergeConflictingGuidanceStrings(ParentWindow.SelectedConfig, toCopyFromSchemes, stringsToImport);
		if (ParentWindow.SelectedConfig.ControlSchemes.Count > 0)
		{
			flag = true;
		}
		foreach (IMControlScheme toCopyFromScheme in toCopyFromSchemes)
		{
			IMControlScheme iMControlScheme = toCopyFromScheme.DeepCopy();
			if (flag)
			{
				iMControlScheme.Selected = false;
			}
			iMControlScheme.BuiltIn = false;
			iMControlScheme.IsBookMarked = false;
			CanvasWindow.SidebarWindow.mSchemeComboBox.mName.Text = iMControlScheme.Name;
			ParentWindow.SelectedConfig.ControlSchemes.Add(iMControlScheme);
			ParentWindow.SelectedConfig.ControlSchemesDict.Add(iMControlScheme.Name, iMControlScheme);
			ComboBoxSchemeControl comboBoxSchemeControl = new ComboBoxSchemeControl(CanvasWindow, ParentWindow);
			((TextBox)comboBoxSchemeControl.mSchemeName).Text = LocaleStrings.GetLocalizedString(iMControlScheme.Name, "");
			((UIElement)comboBoxSchemeControl).IsEnabled = true;
			BlueStacksUIBinding.BindColor((DependencyObject)(object)comboBoxSchemeControl, Control.BackgroundProperty, "ComboBoxBackgroundColor");
			((Panel)CanvasWindow.SidebarWindow.mSchemeComboBox.Items).Children.Add((UIElement)(object)comboBoxSchemeControl);
		}
		if (flag)
		{
			return;
		}
		foreach (IMControlScheme controlScheme in ParentWindow.SelectedConfig.ControlSchemes)
		{
			if (controlScheme.Selected)
			{
				flag2 = true;
				break;
			}
		}
		if (!flag2)
		{
			ParentWindow.SelectedConfig.ControlSchemes[0].Selected = true;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/importschemeswindow.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			mMaskBorder = (Border)target;
			break;
		case 2:
			((UIElement)(CustomPictureBox)target).MouseLeftButtonUp += new MouseButtonEventHandler(Close_MouseLeftButtonUp);
			break;
		case 3:
			mSchemesListScrollbar = (ScrollViewer)target;
			break;
		case 4:
			mSelectAllBtn = (CustomCheckbox)target;
			((ButtonBase)mSelectAllBtn).Click += new RoutedEventHandler(SelectAllBtn_Click);
			break;
		case 5:
			mImportBtn = (CustomButton)target;
			((ButtonBase)mImportBtn).Click += new RoutedEventHandler(ImportBtn_Click);
			break;
		case 6:
			mLoadingGrid = (ProgressBar)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
