using ZwSoft.ZwCAD.Geometry;

namespace Utils
{
    internal static class Extensions
    {
        public static Point3d[] ToArray(this Point3dCollection pts)
        {
            var arr = new Point3d[pts.Count];
            pts.CopyTo(arr, 0);

            return arr;
        }
    }
}
