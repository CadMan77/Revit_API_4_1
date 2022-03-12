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

namespace Revit_API_3_4
{
    [Transaction(TransactionMode.Manual)]

    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            int wallQty = 0;
            string resultString = String.Empty;
            //string folderPath = "c:/windows/temp/-/";
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";
            string fileName = "Wall-params_(Revit-API_Lab4-2).txt";
            string ffp = folderPath + fileName; // full file path

            try
            {
                //int wallQty2 = new FilteredElementCollector(doc)
                //    .OfClass(typeof(Wall))
                //    .GetElementCount();

                FilteredElementCollector wallCollector = new FilteredElementCollector(doc);
                wallCollector.OfCategory(BuiltInCategory.OST_Walls);

                foreach (Element elem in wallCollector)
                {
                    //if (!(elem is WallType))
                    if (elem is Wall) // exclude Family Symbols (WallType)
                    {
                        wallQty += 1;

                        //Parameter volume = elem.LookupParameter("Volume");
                        //Parameter type = elem.LookupParameter("Type");

                        Parameter volume = elem.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                        Parameter type = elem.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM);

                        //TaskDialog.Show($"Стена#{wallQty}", $"Имя типа: \"{type.AsValueString()}\"{Environment.NewLine}Объем: {volume.AsValueString()}");

                        resultString += type.AsValueString() + "; " + volume.AsValueString() + Environment.NewLine;
                    }
                }

                StreamWriter SW = new StreamWriter(ffp);
                SW.Write(resultString);
                SW.Close();

                TaskDialog.Show("Выполнено", $"Данные о всех ({wallQty}шт.) стенах проекта {doc.Title}.RVT выгружены в файл {fileName}{Environment.NewLine}(см. Рабочий стол)."); //{Environment.NewLine}{wallQty2}");
            }
            catch { }
            return Result.Succeeded;
        }
    }
}