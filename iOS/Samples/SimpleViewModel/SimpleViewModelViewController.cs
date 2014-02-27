﻿using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Threading.Tasks;
using System.Threading;
using SimpleViewModel.Core;

namespace SimpleViewModel
{
	public partial class SimpleViewModelViewController : UIViewController
	{
		private readonly MyViewModel viewModel = new MyViewModel();
		private CancellationTokenSource tokenSource;

		public SimpleViewModelViewController () : base ("SimpleViewModelViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
			this.label.Text = viewModel.Label;
			this.button.SetTitle(viewModel.ButtonText, UIControlState.Normal);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear (animated);

			viewModel.PropertyChanged += HandlePropertyChanged;
			button.TouchUpInside += HandleTouchUpInside;
		}

		public override void ViewWillDisappear(bool animated)
		{
			viewModel.PropertyChanged -= HandlePropertyChanged;
			button.TouchUpInside -= HandleTouchUpInside;
			this.CancelTask();
			base.ViewWillDisappear (animated);
		}

		void HandlePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ButtonText")
			{
				this.button.SetTitle (viewModel.ButtonText, UIControlState.Normal);
			}
			else
			{
				this.label.Text = viewModel.Label;
			}
		}

		private void HandleTouchUpInside (object sender, EventArgs e)
		{
			if (this.tokenSource == null)
			{
				this.tokenSource = new CancellationTokenSource ();
				this.viewModel.Update (this.tokenSource.Token).ContinueWith (t => this.tokenSource = null);
			}
			else
			{
				CancelTask();
			}
		}

		private void CancelTask()
		{
			if (this.tokenSource == null)
			{
				return;
			}

			this.tokenSource.Cancel ();
		}
	}
}

