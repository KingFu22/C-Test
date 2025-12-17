using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Test
{
    public class SmartObjectInfo
    {
        public SmartObjectInfo() { }

        public string Name { get; set; }
        public string TemplateName { get; set; }
        public ObservableCollection<SmartObjectProperty> Properties { get; } = new ObservableCollection<SmartObjectProperty>();

        // CSV Export Method
        public static void ExportToCsv(string filePath, IEnumerable<SmartObjectInfo> smartObjects)
        {
            try
            {
                // Step 1: Gather all unique property names
                var allProperties = smartObjects
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
                foreach (var obj in smartObjects)
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
        }
    }
}