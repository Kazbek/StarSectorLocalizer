namespace Localizer.Validators
{
    public static class TranslationPathValidator
    {
        public static bool IsValidFolder(string path)
        {
            try
            {
                return File.Exists(Path.Combine(path, "starsector-core", "starfarer.api.jar.translation.json")) && File.Exists(Path.Combine(path, "starsector-core", "starfarer_obf.jar.translation.json"));
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
