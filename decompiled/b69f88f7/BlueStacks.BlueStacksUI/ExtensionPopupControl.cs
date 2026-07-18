using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

public class ExtensionPopupControl : UserControl, IComponentConnector
{
	[JsonObject(/*Could not decode attribute arguments.*/)]
	internal class ExtensionPopupContext
	{
		[JsonProperty(/*Could not decode attribute arguments.*/)]
		internal string Title;

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		internal string SubTitle;

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		internal string TagLine;

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		internal string Description;

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		internal string FeaturesText;

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		internal IEnumerable<string> features;

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		internal string ExtensionDetailText;

		[JsonProperty(/*Could not decode attribute arguments.*/)]
		internal IEnumerable<KeyValuePair<string, string>> ExtensionDetails;

		public static ExtensionPopupContext ReadJson(JObject input)
		{
			ExtensionPopupContext extensionPopupContext = ((JToken)input).ToObject<ExtensionPopupContext>();
			extensionPopupContext.features = JsonExtensions.ToIenumerableString(input["features"]);
			extensionPopupContext.ExtensionDetails = JsonExtensions.ToStringStringEnumerableKvp(input["ExtensionDetails"]);
			return extensionPopupContext;
		}

		public void WriteJson(JObject writer)
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			writer.Add("Title", JToken.op_Implicit(Title));
			writer.Add("SubTitle", JToken.op_Implicit(SubTitle));
			writer.Add("TagLine", JToken.op_Implicit(TagLine));
			writer.Add("Description", JToken.op_Implicit(Description));
			writer.Add("FeaturesText", JToken.op_Implicit(FeaturesText));
			JArray val = new JArray();
			((JContainer)val).Add((object)features.ToList());
			writer.Add("features", (JToken)val);
			writer.Add("ExtensionDetailText", JToken.op_Implicit(ExtensionDetailText));
			((JContainer)writer).Add((object)"ExtensionDetails");
			foreach (KeyValuePair<string, string> extensionDetail in ExtensionDetails)
			{
				writer.Add(extensionDetail.Key, JToken.op_Implicit(extensionDetail.Value));
			}
		}
	}

	private StackPanel mFeaturesStack;

	private StackPanel mDetailsStack;

	private TextBlock mDetailsText;

	private TextBlock mTagLine;

	private TextBlock mDescription;

	private TextBlock mFeaturesText;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label mTitle;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Label mSubTitle;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomButton mDownloadButton;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mCloseBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal SlideShowControl slideShow;

	private bool _contentLoaded;

	public event EventHandler DownloadClicked;

	public ExtensionPopupControl()
	{
		InitializeComponent();
	}

	internal void LoadExtensionPopupFromFolder(string folderPath)
	{
		if (!Path.IsPathRooted(folderPath))
		{
			folderPath = Path.Combine(CustomPictureBox.AssetsDir, folderPath);
		}
		if (!Directory.Exists(folderPath))
		{
			return;
		}
		try
		{
			string path = Path.Combine(folderPath, "extensionPopup.json");
			if (File.Exists(path))
			{
				ExtensionPopupContext context = ExtensionPopupContext.ReadJson(JObject.Parse(File.ReadAllText(path)));
				slideShow.ImagesFolderPath = folderPath;
				ApplyContext(context);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error while trying to read extensionpopup.json from " + folderPath + "." + ex.ToString());
		}
	}

	private void ApplyContext(ExtensionPopupContext context)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Expected O, but got Unknown
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Expected O, but got Unknown
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Expected O, but got Unknown
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Expected O, but got Unknown
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Expected O, but got Unknown
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Expected O, but got Unknown
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		BlueStacksUIBinding.Bind(mTitle, context.Title);
		BlueStacksUIBinding.Bind(mSubTitle, context.SubTitle);
		BlueStacksUIBinding.Bind(mTagLine, context.TagLine, "");
		BlueStacksUIBinding.Bind(mDescription, context.Description, "");
		if (context.features != null && context.features.Any())
		{
			BlueStacksUIBinding.Bind(mFeaturesText, context.FeaturesText, "");
			foreach (string feature in context.features)
			{
				TextBlock val = new TextBlock
				{
					FontSize = 13.0
				};
				BlueStacksUIBinding.BindColor((DependencyObject)(object)val, Control.ForegroundProperty, "SettingsWindowTabMenuItemForeground");
				((FrameworkElement)val).Margin = new Thickness(7.0, 0.0, 0.0, 5.0);
				val.TextWrapping = (TextWrapping)2;
				BlueStacksUIBinding.Bind(val, feature, "");
				TextBlock val2 = new TextBlock
				{
					Text = "•"
				};
				BlueStacksUIBinding.BindColor((DependencyObject)(object)val2, Control.ForegroundProperty, "SettingsWindowTabMenuItemForeground");
				val2.FontSize = 13.0;
				val2.FontWeight = FontWeights.Bold;
				((FrameworkElement)val2).Margin = new Thickness(0.0, 0.0, 0.0, 5.0);
				BulletDecorator val3 = new BulletDecorator
				{
					Bullet = (UIElement)(object)val2,
					Child = (UIElement)(object)val
				};
				((Panel)mFeaturesStack).Children.Add((UIElement)(object)val3);
			}
		}
		else
		{
			mFeaturesText.Text = "";
		}
		if (context.ExtensionDetails != null && context.ExtensionDetails.Any())
		{
			BlueStacksUIBinding.Bind(mDetailsText, context.ExtensionDetailText, "");
			{
				foreach (KeyValuePair<string, string> extensionDetail in context.ExtensionDetails)
				{
					Grid val4 = new Grid();
					val4.ColumnDefinitions.Add(new ColumnDefinition
					{
						Width = new GridLength(1.0, (GridUnitType)2)
					});
					val4.ColumnDefinitions.Add(new ColumnDefinition
					{
						Width = new GridLength(1.6, (GridUnitType)2)
					});
					TextBlock val5 = new TextBlock
					{
						FontSize = 13.0
					};
					BlueStacksUIBinding.BindColor((DependencyObject)(object)val5, Control.ForegroundProperty, "SettingsWindowTabMenuItemForeground");
					((FrameworkElement)val5).Margin = new Thickness(0.0, 0.0, 0.0, 5.0);
					val5.TextWrapping = (TextWrapping)2;
					BlueStacksUIBinding.Bind(val5, extensionDetail.Key, "");
					((Panel)val4).Children.Add((UIElement)(object)val5);
					Grid.SetColumn((UIElement)(object)val5, 0);
					TextBlock val6 = new TextBlock
					{
						FontSize = 13.0
					};
					BlueStacksUIBinding.BindColor((DependencyObject)(object)val6, Control.ForegroundProperty, "SettingsWindowTabMenuItemForeground");
					((FrameworkElement)val6).Margin = new Thickness(7.0, 0.0, 0.0, 5.0);
					val6.TextWrapping = (TextWrapping)2;
					BlueStacksUIBinding.Bind(val6, extensionDetail.Value, "");
					((Panel)val4).Children.Add((UIElement)(object)val6);
					Grid.SetColumn((UIElement)(object)val6, 1);
					((Panel)mDetailsStack).Children.Add((UIElement)(object)val4);
				}
				return;
			}
		}
		mDetailsText.Text = "";
	}

	private void CloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)this);
	}

	private void mDownloadButton_Click(object sender, RoutedEventArgs e)
	{
		Logger.Info("Clicked DownloadNow Button");
		this.DownloadClicked?.Invoke(sender, (EventArgs)(object)e);
	}

	private void DetailsStack_Initialized(object sender, EventArgs e)
	{
		mDetailsStack = (StackPanel)((sender is StackPanel) ? sender : null);
	}

	private void DetailsText_Initialized(object sender, EventArgs e)
	{
		mDetailsText = (TextBlock)((sender is TextBlock) ? sender : null);
	}

	private void TagLine_Initialized(object sender, EventArgs e)
	{
		mTagLine = (TextBlock)((sender is TextBlock) ? sender : null);
	}

	private void Description_Initialized(object sender, EventArgs e)
	{
		mDescription = (TextBlock)((sender is TextBlock) ? sender : null);
	}

	private void FeaturesText_Initialized(object sender, EventArgs e)
	{
		mFeaturesText = (TextBlock)((sender is TextBlock) ? sender : null);
	}

	private void FeaturesStack_Initialized(object sender, EventArgs e)
	{
		mFeaturesStack = (StackPanel)((sender is StackPanel) ? sender : null);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/extensionpopupcontrol.xaml", UriKind.Relative);
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
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		switch (connectionId)
		{
		case 1:
			mTitle = (Label)target;
			break;
		case 2:
			mSubTitle = (Label)target;
			break;
		case 3:
			mDownloadButton = (CustomButton)target;
			((ButtonBase)mDownloadButton).Click += new RoutedEventHandler(mDownloadButton_Click);
			break;
		case 4:
			mCloseBtn = (CustomPictureBox)target;
			((UIElement)mCloseBtn).MouseLeftButtonUp += new MouseButtonEventHandler(CloseBtn_MouseLeftButtonUp);
			break;
		case 5:
			((FrameworkElement)(TextBlock)target).Initialized += TagLine_Initialized;
			break;
		case 6:
			((FrameworkElement)(TextBlock)target).Initialized += Description_Initialized;
			break;
		case 7:
			((FrameworkElement)(StackPanel)target).Initialized += FeaturesStack_Initialized;
			break;
		case 8:
			((FrameworkElement)(TextBlock)target).Initialized += FeaturesText_Initialized;
			break;
		case 9:
			((FrameworkElement)(StackPanel)target).Initialized += DetailsStack_Initialized;
			break;
		case 10:
			((FrameworkElement)(TextBlock)target).Initialized += DetailsText_Initialized;
			break;
		case 11:
			slideShow = (SlideShowControl)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
