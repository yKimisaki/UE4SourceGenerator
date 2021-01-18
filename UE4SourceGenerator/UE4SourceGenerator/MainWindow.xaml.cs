﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using UE4SourceGenerator.Model;
using MSAPI = Microsoft.WindowsAPICodePack;

namespace UE4SourceGenerator
{
    public partial class MainWindow : Window
    {
        private MainModel model;

        public MainWindow()
        {
            InitializeComponent();

            model = new MainModel();
            UpdateView();
        }

        private void SelectDirectory(object sender, RoutedEventArgs e)
        {
            var dlg = new MSAPI::Dialogs.CommonOpenFileDialog();

            dlg.IsFolderPicker = true;
            dlg.Title = "Select directory";
            dlg.InitialDirectory = Properties.Settings.Default.LastSelectedDirectory;

            if (dlg.ShowDialog() == MSAPI::Dialogs.CommonFileDialogResult.Ok)
            {
                model.OnSelectedFolder(dlg.FileName);
                UpdateView();
            }
        }

        private void UpdateView()
        {
            GenerateToClipboardButton.IsEnabled = ((string)BaseType.SelectedItem == MainModel.ValueTypeKey);

            Output.Text = model.LastSelectedDirectory;
            if (model.HeaderTemplates.Keys.Any())
            {
                BaseType.ItemsSource = new[] { MainModel.ValueTypeKey }.Concat(model.HeaderTemplates.Keys);
            }
            else
            {
                BaseType.ItemsSource = null;
            }
            ApiName.Text = model.ProjectApiName;
        }

        private void GenerateToFile(object sender, RoutedEventArgs e)
        {
            try
            {
                model.Generate(TypeName.Text, (string)BaseType.SelectedItem, true);
                MessageBox.Show($"Succeeded.");
            }
            catch (SourceGenerateException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch
            {
                throw;
            }
        }

        private void GenerateToClipboard(object sender, RoutedEventArgs e)
        {
            try
            {
                model.Generate(TypeName.Text, (string)BaseType.SelectedItem, false);
                MessageBox.Show($"Succeeded.");
            }
            catch (SourceGenerateException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch
            {
                throw;
            }
        }

        private void BaseType_Selected(object sender, RoutedEventArgs e)
        {
            UpdateView();
        }
    }
}