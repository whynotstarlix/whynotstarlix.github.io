using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI;

public class SpeedUpBlueStacks : UserControl, IComponentConnector
{
	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox CloseBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal SpeedUpBluestacksUserControl mEnableVt;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal SpeedUpBluestacksUserControl mConfigureAntivirus;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal SpeedUpBluestacksUserControl mDiasbleHyperV;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal SpeedUpBluestacksUserControl mPowerPlan;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal SpeedUpBluestacksUserControl mUpgradeComputer;

	private bool _contentLoaded;

	public SpeedUpBlueStacks()
	{
		InitializeComponent();
		SetUrl();
		SetContent();
	}

	private void SetUrl()
	{
		if (FeatureManager.Instance.IsCustomUIForDMM)
		{
			mEnableVt.mHyperLink.NavigateUri = new Uri("http://help.dmm.com/-/detail/=/qid=45997/");
			mUpgradeComputer.mHyperLink.NavigateUri = new Uri("http://help.dmm.com/-/detail/=/qid=45997/");
			mConfigureAntivirus.mHyperLink.NavigateUri = new Uri("http://help.dmm.com/-/detail/=/qid=45997/");
			mDiasbleHyperV.mHyperLink.NavigateUri = new Uri("http://help.dmm.com/-/detail/=/qid=45997/");
			mPowerPlan.mHyperLink.NavigateUri = new Uri("http://help.dmm.com/-/detail/=/qid=45997/");
		}
		else
		{
			string text = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
			{
				WebHelper.GetServerHost(),
				"help_articles"
			})) + "&article=";
			mEnableVt.mHyperLink.NavigateUri = new Uri(text + "enable_virtualization");
			mUpgradeComputer.mHyperLink.NavigateUri = new Uri(text + "bs3_nougat_min_requirements");
			mConfigureAntivirus.mHyperLink.NavigateUri = new Uri(text + "disable_antivirus");
			mDiasbleHyperV.mHyperLink.NavigateUri = new Uri(text + "disable_hypervisor");
			mPowerPlan.mHyperLink.NavigateUri = new Uri(text + "change_powerplan");
		}
	}

	private void SetContent()
	{
		BlueStacksUIBinding.Bind(mEnableVt.mTitleText, "STRING_ENABLE_VIRT", "");
		BlueStacksUIBinding.Bind(mEnableVt.mBodyText, "STRING_ENABLE_VIRT_BODY", "");
		((TextElementCollection<Inline>)(object)((Span)mEnableVt.mHyperLink).Inlines).Clear();
		((Span)mEnableVt.mHyperLink).Inlines.Add(LocaleStrings.GetLocalizedString("STRING_ENABLE_VIRT_HYPERLINK", ""));
		mEnableVt.mImage.ImageName = "virtualization";
		BlueStacksUIBinding.Bind(mDiasbleHyperV.mTitleText, "STRING_DISABLE_HYPERV", "");
		BlueStacksUIBinding.Bind(mDiasbleHyperV.mBodyText, "STRING_DISABLE_HYPERV_BODY", "");
		((TextElementCollection<Inline>)(object)((Span)mDiasbleHyperV.mHyperLink).Inlines).Clear();
		((Span)mDiasbleHyperV.mHyperLink).Inlines.Add(LocaleStrings.GetLocalizedString("STRING_DISABLE_HYPERV_HYPERLINK", ""));
		mDiasbleHyperV.mImage.ImageName = "hypervisor";
		BlueStacksUIBinding.Bind(mConfigureAntivirus.mTitleText, "STRING_CONFIGURE_ANTIVIRUS", "");
		BlueStacksUIBinding.Bind(mConfigureAntivirus.mBodyText, "STRING_CONFIGURE_ANTIVIRUS_BODY", "");
		((TextElementCollection<Inline>)(object)((Span)mConfigureAntivirus.mHyperLink).Inlines).Clear();
		((Span)mConfigureAntivirus.mHyperLink).Inlines.Add(LocaleStrings.GetLocalizedString("STRING_CONFIGURE_ANTIVIRUS_HYPERLINK", ""));
		mConfigureAntivirus.mImage.ImageName = "antivirus";
		BlueStacksUIBinding.Bind(mPowerPlan.mTitleText, "STRING_POWER_PLAN", "");
		BlueStacksUIBinding.Bind(mPowerPlan.mBodyText, "STRING_POWER_PLAN_BODY", "");
		((TextElementCollection<Inline>)(object)((Span)mPowerPlan.mHyperLink).Inlines).Clear();
		((Span)mPowerPlan.mHyperLink).Inlines.Add(LocaleStrings.GetLocalizedString("STRING_POWER_PLAN_HYPERLINK", ""));
		mPowerPlan.mImage.ImageName = "powerplan";
		BlueStacksUIBinding.Bind(mUpgradeComputer.mTitleText, "STRING_UPGRADE_SYSTEM", "");
		BlueStacksUIBinding.Bind(mUpgradeComputer.mBodyText, "STRING_UPGRADE_SYSTEM_BODY", "");
		((TextElementCollection<Inline>)(object)((Span)mUpgradeComputer.mHyperLink).Inlines).Clear();
		((Span)mUpgradeComputer.mHyperLink).Inlines.Add(LocaleStrings.GetLocalizedString("STRING_UPGRADE_SYSTEM_HYPERLINK", ""));
		mUpgradeComputer.mImage.ImageName = "upgrade";
	}

	private void CloseBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		Logger.Info("Clicked close button speedUpBluestacks");
		BlueStacksUIUtils.CloseContainerWindow((FrameworkElement)(object)this);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/speedupbluestacks.xaml", UriKind.Relative);
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
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			CloseBtn = (CustomPictureBox)target;
			((UIElement)CloseBtn).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(CloseBtn_PreviewMouseLeftButtonUp);
			break;
		case 2:
			mEnableVt = (SpeedUpBluestacksUserControl)target;
			break;
		case 3:
			mConfigureAntivirus = (SpeedUpBluestacksUserControl)target;
			break;
		case 4:
			mDiasbleHyperV = (SpeedUpBluestacksUserControl)target;
			break;
		case 5:
			mPowerPlan = (SpeedUpBluestacksUserControl)target;
			break;
		case 6:
			mUpgradeComputer = (SpeedUpBluestacksUserControl)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
