using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddRooms
{
    [Transaction(TransactionMode.Manual)]
    public class AddRoomsPlugin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            List<Level> listLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();

            Transaction ts = new Transaction(doc, "Добавление помещений");
            ts.Start();
            ICollection<ElementId> rooms;
            foreach (Level level in listLevel)
            {
                rooms = doc.Create.NewRooms2(level);
            }
            
            List<ElementId> roomIds = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .ToElementIds() as List<ElementId>;
            
            foreach (ElementId roomId in roomIds)
            {
                Element e = doc.GetElement(roomId);
                Room r = e as Room;
                string rName = r.Level.Name.Substring(6);
                r.Name = $"{rName}_{r.Number}";
               
                doc.Create.NewRoomTag(new LinkElementId(roomId), new UV(0, 0), null);
            }

            ts.Commit();

            return Result.Succeeded;
        }
    }
}
