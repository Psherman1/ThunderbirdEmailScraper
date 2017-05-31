using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace EmailScraper
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void OpenFile()
		{
			var dlg = new OpenFileDialog();
			bool? res = dlg.ShowDialog();
			if (res.HasValue && res.Value)
				ParseFile(dlg.FileName);
		}

		private void ParseFile(string filepath)
		{
			try
			{
				if (File.Exists(filepath) == false)
				{
					MessageBox.Show(this, "File does not exist.", "Error", MessageBoxButton.OK);
				}
				else
				{
					var emails = new List<string>();
					using (var fs = new FileStream(filepath, FileMode.Open))
					using (var reader = new StreamReader(fs))
					{
						string str;
						while (reader.EndOfStream == false && (str = reader.ReadLine()) != null)
						{
							if (str.StartsWith("Reply-To:", StringComparison.OrdinalIgnoreCase) == false)
								continue;

							string newStr;
							if (str.EndsWith(">"))
							{
								var ind = str.IndexOf('<');
								newStr = str.Substring(ind + 1, str.Length - 2 - ind);
							}
							else
							{
								newStr = str.Substring(10);
							}

							if (emails.Contains(newStr) == false)
								emails.Add(newStr);
						}
					}

					var dlg = new SaveFileDialog
					{
						Title = "Save Emails To File",
						FileName = "emails.txt"
					};
					bool? res = dlg.ShowDialog();
					if (res.HasValue && res.Value)
					{
						using (var fs = new FileStream(dlg.FileName, FileMode.Create))
						using (var writer = new StreamWriter(fs))
						{
							for (int i = 0; i < emails.Count; i++)
							{
								writer.Write("{0}{1}", emails[i], (i < emails.Count - 1) ? ", " : "");
							}
						}
					}
					MessageBox.Show(this, string.Format("Successfully wrote {0} emails to {1}", emails.Count, Path.GetFileName(dlg.FileName)), "Succes", MessageBoxButton.OK);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Error: " + ex.Message, "Error", MessageBoxButton.OK);
			}
		}

		private void OpenButton_OnClick(object sender, RoutedEventArgs e)
		{
			OpenFile();
		}
	}
}