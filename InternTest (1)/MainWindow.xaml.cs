using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Test
{
    // -- WPF Part --
    // 1. Execute the loading command on clicking the "Load" button
    // 2. Execute the export command on clicking the "Export" button
    // 3. Change the text of the "Load" Button to "Loading" while the data is loaded
    // 4. Display the loaded data in the left ListBox.
    // 5. On Selecting an instance in this list, display its "Properties" in the second ListBox
    // 6. For the first ListBox, display the name and the template name of each entity. The name shall be in bold.
    // 7. For the second ListBox, display the name, the value and the type of each entry. The name shall be in bold.
    // 8. Disable the Export button while data is still loading

    // -- Data handling part --
    // Use the loaded data ("SmartObjects") and export them as a CSV:
    // 1. For each row, the data of a SmartObject shall be displayed.
    // 2. The first column shall be the "Name" of the SmartObject
    // 3. The second column shall be the "TemplateName" of the SmartObject
    // 4. The following rows shall be aggregated by the different released properties of the SmartObjects
    //    i.e. one column per unique property. Properties which do not exist for a SmartObject shall be left empty.
    // 5. Write the result to a file (can be hardcoded)
    // Example output:
    //    Name,Template,String length, Initial value,Min.set value,
    //    InstanceA,SOT1,5,Other value,,
    //    InstanceB,SOT2,,,5,
    //    InstanceC,SOT1,5,Value,,


    namespace Test
    {
        public partial class MainWindow : Window, INotifyPropertyChanged
        {
            #region Test Data
            private readonly SmartObjectInfo[] _testItems = new[]
            {
            new SmartObjectInfo
            {
                Name = "Valve1",
                TemplateName = "Valve",
                Properties =
                {
                    new SmartObjectProperty { Name = "Initial Value", Value = 123, Type = typeof(int) }
                }
            },
            new SmartObjectInfo
            {
                Name = "Valve2",
                TemplateName = "Valve",
                Properties =
                {
                    new SmartObjectProperty { Name = "Initial Value", Value = 456.5d, Type = typeof(double) },
                    new SmartObjectProperty { Name = "StartUpFunction", Value = "NewWindowFunc", Type = typeof(string) }
                }
            },
            new SmartObjectInfo
            {
                Name = "Diesel",
                TemplateName = "Motor",
                Properties =
                {
                    new SmartObjectProperty { Name = "FrameName", Value = "Frame1", Type = typeof(string) }
                }
            },
        };
            #endregion

            public MainWindow()
            {
                LoadSmartObjectInfoCommand = new RelayCommand(DoLoad, CanLoad);
                ExportCommand = new RelayCommand(DoExport, CanExport);
          
            }


            #region Properties Bound to UI
            private bool _isLoading;
            public bool IsLoading
            {
                get => _isLoading;
                set
                {
                    if (value == _isLoading) return;
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }

            private ObservableCollection<SmartObjectInfo> _smartObjects = new ObservableCollection<SmartObjectInfo>();
            public ObservableCollection<SmartObjectInfo> SmartObjects
            {
                get => _smartObjects;
                set
                {
                    if (_smartObjects == value) return;
                    _smartObjects = value;
                    OnPropertyChanged();
                }
            }

            private SmartObjectInfo _selectedSmartObject;
            public SmartObjectInfo SelectedSmartObject
            {
                get => _selectedSmartObject;
                set
                {
                    if (_selectedSmartObject == value) return;
                    _selectedSmartObject = value;
                    OnPropertyChanged();
                }
            }
            #endregion

            #region Commands
            public ICommand LoadSmartObjectInfoCommand { get; }

            private void DoLoad(object parameter)
            {
                new Thread(() =>
                {
                    IsLoading = true;
                    OnPropertyChanged(nameof(IsLoading));  // Update UI to reflect loading state

                    Dispatcher.Invoke(() => SmartObjects.Clear());

                    foreach (var info in _testItems)
                    {
                        Dispatcher.Invoke(() => SmartObjects.Add(info));
                    }

                    IsLoading = false;
                    OnPropertyChanged(nameof(IsLoading));  // Update UI after loading completes
                }).Start();
            }

            private bool CanLoad(object parameter) => !_isLoading;

            public ICommand ExportCommand { get; }

            private void DoExport(object parameter)
            {
                new Thread(() =>
                {
                    string filePath = "exported_data.csv";

                    try
                    {
                        // Step 1: Gather all unique property names
                        var allProperties = SmartObjects
                            .SelectMany(obj => obj.Properties.Select(prop => prop.Name))
                            .Distinct()
                            .ToList();

                        // Step 2: Build the CSV header
                        StringBuilder csvContent = new StringBuilder();
                        csvContent.Append("Name,TemplateName");

                        foreach (var property in allProperties)
                        {
                            csvContent.Append($",{property}");
                        }
                        csvContent.AppendLine();

                        // Step 3: Add data rows
                        foreach (var obj in SmartObjects)
                        {
                            StringBuilder row = new StringBuilder();
                            row.Append($"{obj.Name},{obj.TemplateName}");

                            foreach (var propName in allProperties)
                            {
                                var property = obj.Properties.FirstOrDefault(p => p.Name == propName);
                                row.Append(property != null ? $",{property.Value}" : ",");
                            }
                            csvContent.AppendLine(row.ToString());
                        }

                        // Step 4: Write to CSV file
                        File.WriteAllText(filePath, csvContent.ToString());

                        // Step 5: Notify user
                        MessageBox.Show($"Export completed!\nSaved to: {filePath}", "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }).Start();
            }

            private bool CanExport(object parameter) => !_isLoading;
            #endregion

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string propertyName = "")
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            #endregion
        }
    }
}
