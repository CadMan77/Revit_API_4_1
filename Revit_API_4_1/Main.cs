//Выведите в текстовый файл с разделением данных (разделитель может быть любой) 
//следующие значения всех стен: имя типа стены, объём стены.

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Revit_API_4_1
{
    [Transaction(TransactionMode.Manual)]

    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            string sepChar = ";"; //символ-разделитель
            string wallInfo = String.Empty;
            wallInfo = "Имя типа" + sepChar + "Объем" + (Environment.NewLine);

            try
            {
                //FilteredElementCollector walls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls);                              
                var walls = new FilteredElementCollector(doc) // var ?? List<Wall>
                    .OfCategory(BuiltInCategory.OST_Walls)
                    .WhereElementIsNotElementType()
                    .Cast<Wall>()
                    .ToList();

                foreach (Element elem in walls)
                {
                    //if (!(elem is WallType))
                    //if (elem is Wall) // exclude Family Symbols (WallType)
                    //{

                        //Parameter type = elem.LookupParameter("Type");
                        //Parameter volume = elem.LookupParameter("Volume");

                        Parameter type = elem.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM);
                        Parameter volume = elem.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);

                        //TaskDialog.Show($"Стена#{walls.Count}", $"Имя типа: \"{type.AsValueString()}\"{Environment.NewLine}Объем: {volume.AsValueString()}");

                        wallInfo += type.AsValueString() + sepChar + volume.AsValueString() + Environment.NewLine;
                    //}
                }

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                //string fileName = "WallInfo_(Revit-API_Lab4-1).TXT";
                string fileName = "WallInfo_(Revit-API_Lab4-1).CSV";
                //string exportFilePath = Path.Combine(desktopPath, fileName);

                var saveDialog = new SaveFileDialog
                {
                    OverwritePrompt = true,
                    InitialDirectory = desktopPath,
                    Filter = "All files (*.*)|*.*",
                    FileName = fileName,
                    DefaultExt = ".csv"
                };

                string exportFilePath = string.Empty;
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    exportFilePath = saveDialog.FileName;
                }

                if (string.IsNullOrEmpty(exportFilePath))
                    return Result.Cancelled;

                //File.WriteAllText(exportFilePath, wallInfo);
                File.WriteAllText(exportFilePath, wallInfo, Encoding.UTF8); // Encoding.GetEncoding(1251));

                TaskDialog.Show("Выполнено", $"Данные о всех ({walls.Count()}шт.) стенах проекта {doc.Title}.RVT выгружены в файл {fileName}{Environment.NewLine}.");
            }
            catch 
            {
                TaskDialog.Show("Прервано", "Ошибка");
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}