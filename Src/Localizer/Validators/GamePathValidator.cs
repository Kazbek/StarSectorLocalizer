namespace Localizer.Validators
{
    public static class GamePathValidator
    {
        public static bool IsValid(string path)
        {
            try
            {
                return File.Exists(Path.Combine(path, "starsector.exe"));
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
