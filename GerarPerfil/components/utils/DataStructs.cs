using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwSoft.ZwCAD.Geometry;

namespace Utils
{
    public struct Position
    {
        public Point3d StartPoint { get; }
        public Point3d EndPoint { get; }

        Position(Point3d startPoint, Point3d endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }
    }
    
}
