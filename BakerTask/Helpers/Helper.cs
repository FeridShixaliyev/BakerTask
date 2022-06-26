using System.IO;

namespace BakerTask.Helpers
{
    public static class Helper
    {
        public static void DeleteImg(string root,string path,string imageName)
        {
            string fullPath = Path.Combine(root,path,imageName);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
