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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI;

public class SlideShowControl : UserControl, IDisposable, IComponentConnector
{
	[JsonObject(/*Could not decode attribute arguments.*/)]
	internal class SlideShowContext
	{
		[JsonProperty("key")]
		internal int Key;

		[JsonProperty("imagename")]
		internal string ImageName;

		[JsonProperty("description")]
		internal string Description;
	}

	public enum SlideAnimationType
	{
		Fade,
		Slide
	}

	private SortedDictionary<int, SlideShowContext> mSlideShowDict = new SortedDictionary<int, SlideShowContext>();

	private static string[] ValidImageExtensions = new string[5] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };

	public static readonly DependencyProperty TransitionTypeProperty = DependencyProperty.Register("TransitionType", typeof(SlideAnimationType), typeof(SlideShowControl), new PropertyMetadata((object)SlideAnimationType.Fade));

	public static readonly DependencyProperty TextVerticalAlignmentProperty = DependencyProperty.Register("TextVerticalAlignment", typeof(VerticalAlignment), typeof(SlideShowControl), new PropertyMetadata((object)(VerticalAlignment)2));

	public static readonly DependencyProperty TextHorizontalAlignmentProperty = DependencyProperty.Register("TextHorizontalAlignment", typeof(HorizontalAlignment), typeof(SlideShowControl), new PropertyMetadata((object)(HorizontalAlignment)1));

	public static readonly DependencyProperty IsAutoPlayProperty = DependencyProperty.Register("IsAutoPlay", typeof(bool), typeof(SlideShowControl), new PropertyMetadata((object)false));

	public static readonly DependencyProperty HideArrowOnLeaveProperty = DependencyProperty.Register("HideArrowOnLeave", typeof(bool), typeof(SlideShowControl), new PropertyMetadata((object)true));

	public static readonly DependencyProperty IsArrowVisibleProperty = DependencyProperty.Register("IsArrowVisible", typeof(bool), typeof(SlideShowControl), new PropertyMetadata((object)true));

	public static readonly DependencyProperty SlideDelayProperty = DependencyProperty.Register("SlideDelay", typeof(int), typeof(SlideShowControl), new PropertyMetadata((object)5));

	public static readonly DependencyProperty ImagesFolderPathProperty = DependencyProperty.Register("ImagesFolderPath", typeof(string), typeof(SlideShowControl), new PropertyMetadata((object)""));

	private int _slide;

	private Timer timer;

	private bool disposedValue;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal SlideShowControl slideControl;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal Grid SlideshowGrid;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox image1;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mPrevBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal CustomPictureBox mNextBtn;

	[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
	internal TextBlock SlideshowName;

	private bool _contentLoaded;

	public HorizontalAlignment TextHorizontalAlignment
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (HorizontalAlignment)((DependencyObject)this).GetValue(TextHorizontalAlignmentProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(TextHorizontalAlignmentProperty, (object)value);
		}
	}

	public VerticalAlignment TextVerticalAlignment
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (VerticalAlignment)((DependencyObject)this).GetValue(TextVerticalAlignmentProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(TextVerticalAlignmentProperty, (object)value);
		}
	}

	public string ImagesFolderPath
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(ImagesFolderPathProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ImagesFolderPathProperty, (object)value);
		}
	}

	public bool IsArrowVisible
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsArrowVisibleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsArrowVisibleProperty, (object)value);
		}
	}

	public bool HideArrowOnLeave
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(HideArrowOnLeaveProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(HideArrowOnLeaveProperty, (object)value);
		}
	}

	public bool IsAutoPlay
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsAutoPlayProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsAutoPlayProperty, (object)value);
		}
	}

	public int SlideDelay
	{
		get
		{
			return (int)((DependencyObject)this).GetValue(SlideDelayProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SlideDelayProperty, (object)value);
		}
	}

	public SlideAnimationType TransitionType
	{
		get
		{
			return (SlideAnimationType)((DependencyObject)this).GetValue(TransitionTypeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(TransitionTypeProperty, (object)value);
		}
	}

	public SlideShowControl()
	{
		InitializeComponent();
		image1_MouseEnter(null, null);
	}

	internal void AddOrUpdateSlide(SlideShowContext slideContext)
	{
		if (mSlideShowDict.ContainsKey(slideContext.Key))
		{
			mSlideShowDict[slideContext.Key] = slideContext;
		}
		else
		{
			mSlideShowDict.Add((slideContext.Key == 0) ? mSlideShowDict.Count : slideContext.Key, slideContext);
		}
	}

	internal void PlaySlideShow()
	{
		IsAutoPlay = true;
		SlideShowLoop();
		StartImageTransition(_slide + 1);
	}

	internal void StopSlideShow()
	{
		IsAutoPlay = false;
	}

	internal void LoadImagesFromFolder(string folderPath)
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
			string path = Path.Combine(folderPath, "slides.json");
			if (File.Exists(path))
			{
				IEnumerable<SlideShowContext> enumerable = ((JToken)JObject.Parse(File.ReadAllText(path))).ToObject<IEnumerable<SlideShowContext>>();
				if (enumerable != null)
				{
					foreach (SlideShowContext item in enumerable)
					{
						if (!string.IsNullOrEmpty(item.Description))
						{
							item.Description = LocaleStrings.GetLocalizedString(item.Description, "");
						}
						AddOrUpdateSlide(item);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Error while trying to read slides.json from " + folderPath + "." + ex.ToString());
			mSlideShowDict.Clear();
		}
		if (mSlideShowDict.Count == 0)
		{
			FileInfo[] files = new DirectoryInfo(folderPath).GetFiles();
			int num = 0;
			for (int i = 0; i < files.Length; i++)
			{
				if (Enumerable.Contains<string>(ValidImageExtensions, files[i].Extension, StringComparer.InvariantCultureIgnoreCase))
				{
					AddOrUpdateSlide(new SlideShowContext
					{
						Key = num,
						ImageName = files[i].FullName
					});
					num++;
				}
			}
		}
		StartImageTransition(0);
	}

	private void SlideShowLoop(bool forceStart = false)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		if (forceStart && timer != null)
		{
			timer.Enabled = false;
		}
		if (timer != null && !timer.Enabled)
		{
			((Component)(object)timer).Dispose();
		}
		if (IsAutoPlay && mSlideShowDict.Count > 1)
		{
			timer = new Timer
			{
				Interval = SlideDelay * 1000
			};
			timer.Tick += Timer_Tick;
			timer.Start();
		}
	}

	private void Timer_Tick(object sender, EventArgs e)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (timer.Enabled && IsAutoPlay && mSlideShowDict.Count > 1 && sender == timer)
		{
			StartImageTransition(_slide + 1);
		}
		else
		{
			((Timer)sender).Enabled = false;
		}
	}

	private void StartImageTransition(int i)
	{
		if (mSlideShowDict.Count > 0)
		{
			if (_slide == i)
			{
				image1.ImageName = mSlideShowDict[_slide].ImageName;
				SlideshowName.Text = mSlideShowDict[_slide].Description;
				SlideShowLoop();
			}
			else if (i >= mSlideShowDict.Count)
			{
				UnloadImage(0);
			}
			else if (i < 0)
			{
				UnloadImage(mSlideShowDict.Count - 1);
			}
			else
			{
				UnloadImage(i);
			}
		}
	}

	private void UnloadImage(int imageToShow)
	{
		object obj = ((FrameworkElement)this).Resources[(object)string.Format(CultureInfo.InvariantCulture, "{0}Out", new object[1] { TransitionType.ToString() })];
		Storyboard obj2 = ((Storyboard)((obj is Storyboard) ? obj : null)).Clone();
		((Timeline)obj2).Completed += delegate
		{
			image1.ImageName = mSlideShowDict[imageToShow].ImageName;
			LoadImage(imageToShow);
		};
		Storyboard.SetTarget((DependencyObject)(object)obj2, (DependencyObject)(object)SlideshowGrid);
		obj2.Begin();
	}

	private void LoadImage(int imageToShow)
	{
		_slide = imageToShow;
		SlideshowName.Text = mSlideShowDict[imageToShow].Description;
		object obj = ((FrameworkElement)this).Resources[(object)string.Format(CultureInfo.InvariantCulture, "{0}In", new object[1] { TransitionType.ToString() })];
		object obj2 = ((obj is Storyboard) ? obj : null);
		Storyboard.SetTarget((DependencyObject)obj2, (DependencyObject)(object)SlideshowGrid);
		((Storyboard)obj2).Begin();
	}

	private void mPrevBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		SlideShowLoop(forceStart: true);
		StartImageTransition(_slide - 1);
	}

	private void mNextBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		SlideShowLoop(forceStart: true);
		StartImageTransition(_slide + 1);
	}

	private void SlideShowControl_Loaded(object sender, RoutedEventArgs e)
	{
		if (!string.IsNullOrEmpty(ImagesFolderPath))
		{
			LoadImagesFromFolder(ImagesFolderPath);
		}
	}

	private void image1_MouseEnter(object sender, MouseEventArgs e)
	{
		if (!IsArrowVisible || mSlideShowDict.Count < 2)
		{
			image1_MouseLeave(sender, e);
		}
		else if (HideArrowOnLeave)
		{
			if (((UIElement)image1).IsMouseOver)
			{
				((UIElement)mPrevBtn).Visibility = (Visibility)0;
				((UIElement)mNextBtn).Visibility = (Visibility)0;
			}
			else
			{
				image1_MouseLeave(sender, e);
			}
		}
	}

	private void image1_MouseLeave(object sender, MouseEventArgs e)
	{
		if (!IsArrowVisible || mSlideShowDict.Count < 2)
		{
			((UIElement)mPrevBtn).Visibility = (Visibility)1;
			((UIElement)mNextBtn).Visibility = (Visibility)1;
		}
		else if (HideArrowOnLeave && !((UIElement)mPrevBtn).IsMouseOver && !((UIElement)mNextBtn).IsMouseOver && !((UIElement)image1).IsMouseOver)
		{
			((UIElement)mPrevBtn).Visibility = (Visibility)1;
			((UIElement)mNextBtn).Visibility = (Visibility)1;
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (timer != null)
			{
				timer.Tick += Timer_Tick;
				((Component)(object)timer).Dispose();
			}
			disposedValue = true;
		}
	}

	~SlideShowControl()
	{
		try
		{
			Dispose(disposing: false);
		}
		finally
		{
			((object)this).Finalize();
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/slideshowcontrol.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			slideControl = (SlideShowControl)target;
			((FrameworkElement)slideControl).Loaded += new RoutedEventHandler(SlideShowControl_Loaded);
			break;
		case 2:
			SlideshowGrid = (Grid)target;
			break;
		case 3:
			image1 = (CustomPictureBox)target;
			((UIElement)image1).MouseEnter += new MouseEventHandler(image1_MouseEnter);
			((UIElement)image1).MouseLeave += new MouseEventHandler(image1_MouseLeave);
			break;
		case 4:
			mPrevBtn = (CustomPictureBox)target;
			((UIElement)mPrevBtn).MouseLeftButtonUp += new MouseButtonEventHandler(mPrevBtn_MouseLeftButtonUp);
			((UIElement)mPrevBtn).MouseLeave += new MouseEventHandler(image1_MouseLeave);
			break;
		case 5:
			mNextBtn = (CustomPictureBox)target;
			((UIElement)mNextBtn).MouseLeftButtonUp += new MouseButtonEventHandler(mNextBtn_MouseLeftButtonUp);
			((UIElement)mNextBtn).MouseLeave += new MouseEventHandler(image1_MouseLeave);
			break;
		case 6:
			SlideshowName = (TextBlock)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
