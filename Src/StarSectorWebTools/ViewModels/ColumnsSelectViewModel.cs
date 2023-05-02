namespace StarSectorWebTools.ViewModels
{
    public class ColumnsSelectViewModel
    {
        public List<ColumnsSelectEntryViewModel> Columns { get; set; }
        public bool IsValidSelect()
        {
            return Columns.Count(t => t.ColumnType == ColumnType.Key) == 1 && Columns.Any(t => t.ColumnType == ColumnType.ToTranslate);
        }
    }

    public class ColumnsSelectEntryViewModel
    {
        public string Name { get; set; }
        public ColumnType ColumnType { get; set; }
        public string GetFriendlyType() => ColumnType switch
        {
            ColumnType.Ignore => "Ничего не делать",
            ColumnType.Key => "Это ключ",
            ColumnType.ToTranslate => "Для перевода",
            _ => ColumnType.ToString()
        };
    }

    public enum ColumnType
    {
        Ignore = 0,
        Key = 1,
        ToTranslate = 2
    }
}
